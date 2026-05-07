using System;
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;
using PaintDotNet;
using PaintDotNet.Effects;
using AForge;
using AForge.Imaging.Filters;
using Point = System.Drawing.Point;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace QuadrilateralCorrectionEffect
{
    public class PluginSupportInfo : IPluginSupportInfo
    {
        public string Author => base.GetType().Assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
        public string Copyright => base.GetType().Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
        public string DisplayName => base.GetType().Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
        public Version Version => base.GetType().Assembly.GetName().Version;
        public Uri WebsiteUri => new Uri("https://forums.getpaint.net/topic/110247-quadrilateral-correction/");
    }

    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "Quadrilateral Correction")]
    internal class QuadrilateralCorrectionEffectPlugin : Effect<QuadrilateralCorrectionConfigToken>
    {
        private static readonly Image StaticIcon = new Bitmap(typeof(QuadrilateralCorrectionEffectPlugin), "Icon.png");

        [Obsolete("The classic effect system has been deprecated. Please move over to the modern replacements: BitmapEffect for CPU rendering, and GpuEffect for GPU rendering via Direct2D.", false)]
        public QuadrilateralCorrectionEffectPlugin()
            : base("Quadrilateral Correction", StaticIcon, "Tools", new EffectOptions { Flags = EffectFlags.Configurable })
        {
        }

        public override EffectConfigDialog CreateConfigDialog()
        {
            return new QuadrilateralCorrectionConfigDialog();
        }

        protected override void OnSetRenderInfo(QuadrilateralCorrectionConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            topLeft = newToken.TopLeft;
            topRight = newToken.TopRight;
            bottomRight = newToken.BottomRight;
            bottomLeft = newToken.BottomLeft;
            autoDims = newToken.AutoDims;
            width = newToken.Width;
            height = newToken.Height;
            center = newToken.Center;
            cropOutsideQuadrilateral = newToken.CropOutsideQuadrilateral;
            var sourceQuadrilateral = new List<IntPoint>
            {
                new IntPoint(topLeft.X, topLeft.Y),
                new IntPoint(topRight.X, topRight.Y),
                new IntPoint(bottomRight.X, bottomRight.Y),
                new IntPoint(bottomLeft.X, bottomLeft.Y)
            };

            Rectangle selection = EnvironmentParameters.SelectionBounds;
            PdnRegion exactSelection = EnvironmentParameters.GetSelectionAsPdnRegion();

            Bitmap quadTransOutput;
            Point preserveOutsideOffset = Point.Empty;
            Size preserveOutsideSize = Size.Empty;

            try
            {
                using Bitmap srcImage = srcArgs.Surface.CreateAliasedBitmap(selection);
                if (cropOutsideQuadrilateral)
                {
                    // create filter: remove content outside the quadrilateral
                    QuadrilateralTransformation quadTrans = new QuadrilateralTransformation
                    {
                        SourceQuadrilateral = sourceQuadrilateral,
                        AutomaticSizeCalculaton = autoDims,
                        NewWidth = width,
                        NewHeight = height
                    };
                    quadTransOutput = quadTrans.Apply(srcImage);
                }
                else
                {
                    // Preserve content outside the quadrilateral and apply perspective warp to the entire image
                    quadTransOutput = PerspectiveWarpPreserveOutside(
                        srcImage,
                        topLeft,
                        topRight,
                        bottomRight,
                        bottomLeft,
                        autoDims,
                        width,
                        height,
                        out preserveOutsideOffset,
                        out preserveOutsideSize);
                }

            }
            catch
            {
                quadTransOutput = new Bitmap(1, 1);
                preserveOutsideSize = quadTransOutput.Size;
            }

            int outputWidth = cropOutsideQuadrilateral ? quadTransOutput.Width : preserveOutsideSize.Width;
            int outputHeight = cropOutsideQuadrilateral ? quadTransOutput.Height : preserveOutsideSize.Height;

            Point offSet = new Point
            {
                X = selection.X + (center ? (selection.Width - outputWidth) / 2 : 0),
                Y = selection.Y + (center ? (selection.Height - outputHeight) / 2 : 0)
            };

            if (!cropOutsideQuadrilateral)
            {
                offSet.X += preserveOutsideOffset.X;
                offSet.Y += preserveOutsideOffset.Y;

                offSet = FitOffsetInsideCanvas(
                    offSet,
                    quadTransOutput.Size,
                    srcArgs.Surface.Size);
            }

            Bitmap alignedImage = new Bitmap(srcArgs.Surface.Width, srcArgs.Surface.Height);
            using (Graphics graphics = Graphics.FromImage(alignedImage))
            {
                graphics.DrawImage(quadTransOutput, offSet);
            }
            quadTransOutput.Dispose();

            if (quadrilateralSurface == null)
            {
                quadrilateralSurface = new Surface(srcArgs.Surface.Size);
            }

            quadrilateralSurface = Surface.CopyFromBitmap(alignedImage);
            alignedImage.Dispose();

            dstArgs.Surface.Fill(exactSelection, Color.Transparent);
            dstArgs.Surface.CopySurface(quadrilateralSurface, exactSelection);

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        protected override void OnRender(Rectangle[] renderRects, int startIndex, int length)
        {
            return;
        }

        private Point topLeft;
        private Point topRight;
        private Point bottomRight;
        private Point bottomLeft;
        private bool autoDims;
        private int width;
        private int height;
        private bool center;
        private bool cropOutsideQuadrilateral;

        private Surface quadrilateralSurface;

        #region Perspective Warp Helpers
        private static Bitmap PerspectiveWarpPreserveOutside(
            Bitmap source,
            Point topLeft,
            Point topRight,
            Point bottomRight,
            Point bottomLeft,
            bool autoDims,
            int outputWidth,
            int outputHeight,
            out Point preserveOutsideOffset,
            out Size preserveOutsideQuadrilateralSize)
        {
            PointF[] src =
            {
                topLeft,
                topRight,
                bottomRight,
                bottomLeft
            };

            if (autoDims || outputWidth == int.MaxValue || outputHeight == int.MaxValue || outputWidth <= 0 || outputHeight <= 0)
            {
                double topWidth = Distance(topLeft, topRight);
                double bottomWidth = Distance(bottomLeft, bottomRight);
                double leftHeight = Distance(topLeft, bottomLeft);
                double rightHeight = Distance(topRight, bottomRight);

                outputWidth = Math.Max(1, (int)Math.Ceiling(Math.Max(topWidth, bottomWidth)));
                outputHeight = Math.Max(1, (int)Math.Ceiling(Math.Max(leftHeight, rightHeight)));
            }

            preserveOutsideQuadrilateralSize = new Size(outputWidth, outputHeight);

            PointF[] dst =
            {
                new PointF(0, 0),
                new PointF(outputWidth - 1, 0),
                new PointF(outputWidth - 1, outputHeight - 1),
                new PointF(0, outputHeight - 1)
            };

            // source -> corrected rectangle
            double[,] h = ComputeHomography(src, dst);

            PointF corner1 = ApplyHomography(h, 0, 0);
            PointF corner2 = ApplyHomography(h, source.Width - 1, 0);
            PointF corner3 = ApplyHomography(h, source.Width - 1, source.Height - 1);
            PointF corner4 = ApplyHomography(h, 0, source.Height - 1);

            double minX = Math.Min(Math.Min(corner1.X, corner2.X), Math.Min(corner3.X, corner4.X));
            double minY = Math.Min(Math.Min(corner1.Y, corner2.Y), Math.Min(corner3.Y, corner4.Y));
            double maxX = Math.Max(Math.Max(corner1.X, corner2.X), Math.Max(corner3.X, corner4.X));
            double maxY = Math.Max(Math.Max(corner1.Y, corner2.Y), Math.Max(corner3.Y, corner4.Y));

            minX = Math.Floor(minX);
            minY = Math.Floor(minY);
            maxX = Math.Ceiling(maxX);
            maxY = Math.Ceiling(maxY);

            preserveOutsideOffset = new Point((int)minX, (int)minY);

            int destinationWidth = Math.Max(1, (int)(maxX - minX + 1));
            int destinationHeight = Math.Max(1, (int)(maxY - minY + 1));

            Bitmap destination = new Bitmap(destinationWidth, destinationHeight, PixelFormat.Format32bppPArgb);

            // corrected rectangle -> source
            double[,] hInverse = ComputeHomography(dst, src);

            Bitmap source32 = source;

            if (source.PixelFormat != PixelFormat.Format32bppPArgb &&
                source.PixelFormat != PixelFormat.Format32bppArgb)
            {
                source32 = source.Clone(
                    new Rectangle(0, 0, source.Width, source.Height),
                    PixelFormat.Format32bppPArgb);
            }

            BitmapData sourceData = null;
            BitmapData destinationData = null;

            try
            {
                Rectangle sourceRect = new Rectangle(0, 0, source32.Width, source32.Height);
                Rectangle destinationRect = new Rectangle(0, 0, destination.Width, destination.Height);

                sourceData = source32.LockBits(sourceRect, ImageLockMode.ReadOnly, source32.PixelFormat);
                destinationData = destination.LockBits(destinationRect, ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);

                int sourceStride = Math.Abs(sourceData.Stride);
                int destinationStride = Math.Abs(destinationData.Stride);

                byte[] sourceBuffer = new byte[sourceStride * source32.Height];
                byte[] destinationBuffer = new byte[destinationStride * destination.Height];

                Marshal.Copy(sourceData.Scan0, sourceBuffer, 0, sourceBuffer.Length);

                int sourceWidth = source32.Width;
                int sourceHeight = source32.Height;
                int destinationWidthLocal = destinationWidth;
                int destinationHeightLocal = destinationHeight;

                double h00 = hInverse[0, 0];
                double h01 = hInverse[0, 1];
                double h02 = hInverse[0, 2];
                double h10 = hInverse[1, 0];
                double h11 = hInverse[1, 1];
                double h12 = hInverse[1, 2];
                double h20 = hInverse[2, 0];
                double h21 = hInverse[2, 1];
                double h22 = hInverse[2, 2];

                System.Threading.Tasks.Parallel.For(0, destinationHeightLocal, y =>
                {
                    double correctedY = y + minY;

                    for (int x = 0; x < destinationWidthLocal; x++)
                    {
                        double correctedX = x + minX;

                        double denominator = h20 * correctedX + h21 * correctedY + h22;

                        if (Math.Abs(denominator) < 1e-12)
                        {
                            continue;
                        }

                        double sourceX = (h00 * correctedX + h01 * correctedY + h02) / denominator;
                        double sourceY = (h10 * correctedX + h11 * correctedY + h12) / denominator;

                        SampleBilinear(
                            sourceBuffer,
                            sourceStride,
                            sourceWidth,
                            sourceHeight,
                            sourceX,
                            sourceY,
                            out byte a,
                            out byte r,
                            out byte g,
                            out byte b);

                        int destinationIndex = y * destinationStride + x * 4;

                        destinationBuffer[destinationIndex] = b;
                        destinationBuffer[destinationIndex + 1] = g;
                        destinationBuffer[destinationIndex + 2] = r;
                        destinationBuffer[destinationIndex + 3] = a;
                    }
                });

                Marshal.Copy(destinationBuffer, 0, destinationData.Scan0, destinationBuffer.Length);
            }
            finally
            {
                if (destinationData != null)
                {
                    destination.UnlockBits(destinationData);
                }

                if (sourceData != null)
                {
                    source32.UnlockBits(sourceData);
                }

                if (!ReferenceEquals(source32, source))
                {
                    source32.Dispose();
                }
            }

            return destination;
        }

        private static double Distance(Point p1, Point p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        private static double[,] ComputeHomography(PointF[] src, PointF[] dst)
        {
            double[,] a = new double[8, 8];
            double[] b = new double[8];

            for (int i = 0; i < 4; i++)
            {
                double x = src[i].X;
                double y = src[i].Y;
                double u = dst[i].X;
                double v = dst[i].Y;

                int r = i * 2;

                a[r, 0] = x;
                a[r, 1] = y;
                a[r, 2] = 1;
                a[r, 3] = 0;
                a[r, 4] = 0;
                a[r, 5] = 0;
                a[r, 6] = -u * x;
                a[r, 7] = -u * y;
                b[r] = u;

                a[r + 1, 0] = 0;
                a[r + 1, 1] = 0;
                a[r + 1, 2] = 0;
                a[r + 1, 3] = x;
                a[r + 1, 4] = y;
                a[r + 1, 5] = 1;
                a[r + 1, 6] = -v * x;
                a[r + 1, 7] = -v * y;
                b[r + 1] = v;
            }

            double[] h = SolveLinearSystem(a, b);

            return new double[,]
            {
                { h[0], h[1], h[2] },
                { h[3], h[4], h[5] },
                { h[6], h[7], 1.0 }
            };
        }

        private static PointF ApplyHomography(double[,] h, double x, double y)
        {
            double denominator = h[2, 0] * x + h[2, 1] * y + h[2, 2];

            if (Math.Abs(denominator) < 1e-12)
            {
                return new PointF(float.NaN, float.NaN);
            }

            double sourceX = (h[0, 0] * x + h[0, 1] * y + h[0, 2]) / denominator;
            double sourceY = (h[1, 0] * x + h[1, 1] * y + h[1, 2]) / denominator;

            return new PointF((float)sourceX, (float)sourceY);
        }

        private static void SampleBilinear(
            byte[] sourceBuffer,
            int sourceStride,
            int sourceWidth,
            int sourceHeight,
            double x,
            double y,
            out byte a,
            out byte r,
            out byte g,
            out byte b)
        {
            a = 0;
            r = 0;
            g = 0;
            b = 0;

            if (double.IsNaN(x) || double.IsNaN(y))
            {
                return;
            }

            if (x < 0 || y < 0 || x >= sourceWidth - 1 || y >= sourceHeight - 1)
            {
                return;
            }

            int x0 = (int)Math.Floor(x);
            int y0 = (int)Math.Floor(y);
            int x1 = x0 + 1;
            int y1 = y0 + 1;

            double dx = x - x0;
            double dy = y - y0;

            double wx00 = (1 - dx) * (1 - dy);
            double wx10 = dx * (1 - dy);
            double wx01 = (1 - dx) * dy;
            double wx11 = dx * dy;

            int p00 = y0 * sourceStride + x0 * 4;
            int p10 = y0 * sourceStride + x1 * 4;
            int p01 = y1 * sourceStride + x0 * 4;
            int p11 = y1 * sourceStride + x1 * 4;

            b = (byte)ClampToByte(
                sourceBuffer[p00] * wx00 +
                sourceBuffer[p10] * wx10 +
                sourceBuffer[p01] * wx01 +
                sourceBuffer[p11] * wx11);

            g = (byte)ClampToByte(
                sourceBuffer[p00 + 1] * wx00 +
                sourceBuffer[p10 + 1] * wx10 +
                sourceBuffer[p01 + 1] * wx01 +
                sourceBuffer[p11 + 1] * wx11);

            r = (byte)ClampToByte(
                sourceBuffer[p00 + 2] * wx00 +
                sourceBuffer[p10 + 2] * wx10 +
                sourceBuffer[p01 + 2] * wx01 +
                sourceBuffer[p11 + 2] * wx11);

            a = (byte)ClampToByte(
                sourceBuffer[p00 + 3] * wx00 +
                sourceBuffer[p10 + 3] * wx10 +
                sourceBuffer[p01 + 3] * wx01 +
                sourceBuffer[p11 + 3] * wx11);
        }

        private static int ClampToByte(double value)
        {
            if (value < 0)
            {
                return 0;
            }

            if (value > 255)
            {
                return 255;
            }

            return (int)Math.Round(value);
        }

        private static double[] SolveLinearSystem(double[,] a, double[] b)
        {
            int n = b.Length;

            for (int i = 0; i < n; i++)
            {
                int maxRow = i;
                double maxValue = Math.Abs(a[i, i]);

                for (int row = i + 1; row < n; row++)
                {
                    double value = Math.Abs(a[row, i]);

                    if (value > maxValue)
                    {
                        maxValue = value;
                        maxRow = row;
                    }
                }

                if (maxValue < 1e-12)
                {
                    throw new InvalidOperationException("Homography matrix is singular.");
                }

                if (maxRow != i)
                {
                    for (int col = i; col < n; col++)
                    {
                        double temp = a[i, col];
                        a[i, col] = a[maxRow, col];
                        a[maxRow, col] = temp;
                    }

                    double tempB = b[i];
                    b[i] = b[maxRow];
                    b[maxRow] = tempB;
                }

                double pivot = a[i, i];

                for (int col = i; col < n; col++)
                {
                    a[i, col] /= pivot;
                }

                b[i] /= pivot;

                for (int row = 0; row < n; row++)
                {
                    if (row == i)
                    {
                        continue;
                    }

                    double factor = a[row, i];

                    for (int col = i; col < n; col++)
                    {
                        a[row, col] -= factor * a[i, col];
                    }

                    b[row] -= factor * b[i];
                }
            }

            return b;
        }

        private static Point FitOffsetInsideCanvas(Point offSet, Size imageSize, Size canvasSize)
        {
            Rectangle canvasBounds = new Rectangle(0, 0, canvasSize.Width, canvasSize.Height);
            Rectangle outputBounds = new Rectangle(offSet, imageSize);

            if (outputBounds.Width <= canvasBounds.Width)
            {
                if (outputBounds.Left < canvasBounds.Left)
                {
                    offSet.X += canvasBounds.Left - outputBounds.Left;
                }
                else if (outputBounds.Right > canvasBounds.Right)
                {
                    offSet.X -= outputBounds.Right - canvasBounds.Right;
                }
            }

            if (outputBounds.Height <= canvasBounds.Height)
            {
                if (outputBounds.Top < canvasBounds.Top)
                {
                    offSet.Y += canvasBounds.Top - outputBounds.Top;
                }
                else if (outputBounds.Bottom > canvasBounds.Bottom)
                {
                    offSet.Y -= outputBounds.Bottom - canvasBounds.Bottom;
                }
            }

            return offSet;
        }
        #endregion
    }
}
