using System;
using System.Drawing;

namespace QuadrilateralCorrectionEffect
{
    internal static class PerspectiveWarpUtil
    {
        #region Perspective Warp Helpers
        public static BitmapRegionUtil.BitmapBgra32Data PerspectiveWarp(
            BitmapRegionUtil.BitmapBgra32Data source,
            Point topLeft,
            Point topRight,
            Point bottomRight,
            Point bottomLeft,
            bool autoDims,
            int outputWidth,
            int outputHeight,
            ResamplingMode resamplingMode,
            CropOutsideMode cropOutsideMode,
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

                double correctedWidth = Math.Max(topWidth, bottomWidth) + 1.0;
                double correctedHeight = Math.Max(leftHeight, rightHeight) + 1.0;

                outputWidth = Math.Max(1, (int)Math.Ceiling(correctedWidth));
                outputHeight = Math.Max(1, (int)Math.Ceiling(correctedHeight));
            }

            preserveOutsideQuadrilateralSize = new Size(outputWidth, outputHeight);

            PointF[] dst =
            {
                new PointF(0, 0),
                new PointF(outputWidth - 1, 0),
                new PointF(outputWidth - 1, outputHeight - 1),
                new PointF(0, outputHeight - 1)
            };

            double minX = 0;
            double minY = 0;
            int destinationWidth = outputWidth;
            int destinationHeight = outputHeight;

            if (cropOutsideMode == CropOutsideMode.Crop)
            {
                preserveOutsideOffset = Point.Empty;
            }
            else
            {
                // source -> corrected rectangle
                double[,] h = ComputeHomography(src, dst);

                PointF corner1 = ApplyHomography(h, 0, 0);
                PointF corner2 = ApplyHomography(h, source.Width - 1, 0);
                PointF corner3 = ApplyHomography(h, source.Width - 1, source.Height - 1);
                PointF corner4 = ApplyHomography(h, 0, source.Height - 1);

                minX = Math.Min(Math.Min(corner1.X, corner2.X), Math.Min(corner3.X, corner4.X));
                minY = Math.Min(Math.Min(corner1.Y, corner2.Y), Math.Min(corner3.Y, corner4.Y));
                double maxX = Math.Max(Math.Max(corner1.X, corner2.X), Math.Max(corner3.X, corner4.X));
                double maxY = Math.Max(Math.Max(corner1.Y, corner2.Y), Math.Max(corner3.Y, corner4.Y));

                minX = Math.Floor(minX);
                minY = Math.Floor(minY);
                maxX = Math.Ceiling(maxX);
                maxY = Math.Ceiling(maxY);

                preserveOutsideOffset = new Point((int)minX, (int)minY);

                destinationWidth = Math.Max(1, (int)(maxX - minX + 1));
                destinationHeight = Math.Max(1, (int)(maxY - minY + 1));
            }

            BitmapRegionUtil.BitmapBgra32Data destination = BitmapRegionUtil.CreateBgra32Data(destinationWidth, destinationHeight);

            // corrected rectangle -> source
            double[,] hInverse = ComputeHomography(dst, src);
            byte[] sourceBuffer = source.Buffer;
            byte[] destinationBuffer = destination.Buffer;

            int sourceStride = source.StrideAbs;
            int destinationStride = destination.StrideAbs;
            int sourceWidth = source.Width;
            int sourceHeight = source.Height;
            int destinationWidthLocal = destinationWidth;
            int destinationHeightLocal = destinationHeight;

            // h00 through h22 are the nine coefficients of hInverse,
            // a 3x3 inverse perspective transformation matrix.
            // They are used to transform each corrected coordinate
            // (correctedX, correctedY) in the output image into the floating-point
            // sampling coordinate (sourceX, sourceY) in the original image.

            // Compute the numerator for sourceX.
            double h00 = hInverse[0, 0];
            double h01 = hInverse[0, 1];
            double h02 = hInverse[0, 2];
            // Compute the numerator for sourceY.
            double h10 = hInverse[1, 0];
            double h11 = hInverse[1, 1];
            double h12 = hInverse[1, 2];
            // Compute the perspective denominator.
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

                    byte a;
                    byte r;
                    byte g;
                    byte b;

                    if (resamplingMode == ResamplingMode.HighQualitySupersampling)
                    {
                        SampleSupersampling2x2(
                            sourceBuffer,
                            sourceStride,
                            sourceWidth,
                            sourceHeight,
                            correctedX,
                            correctedY,
                            h00,
                            h01,
                            h02,
                            h10,
                            h11,
                            h12,
                            h20,
                            h21,
                            h22,
                            cropOutsideMode,
                            out a,
                            out r,
                            out g,
                            out b);
                    }
                    else
                    {
                        if (cropOutsideMode == CropOutsideMode.Repeat || cropOutsideMode == CropOutsideMode.Mirror)
                        {
                            ApplyCropOutsideMode(
                                cropOutsideMode,
                                sourceWidth,
                                sourceHeight,
                                ref sourceX,
                                ref sourceY);
                        }

                        SampleWithMode(
                            sourceBuffer,
                            sourceStride,
                            sourceWidth,
                            sourceHeight,
                            sourceX,
                            sourceY,
                            resamplingMode,
                            out a,
                            out r,
                            out g,
                            out b);
                    }

                    int destinationIndex = y * destinationStride + x * 4;

                    destinationBuffer[destinationIndex] = b;
                    destinationBuffer[destinationIndex + 1] = g;
                    destinationBuffer[destinationIndex + 2] = r;
                    destinationBuffer[destinationIndex + 3] = a;
                }
            });

            return destination;
        }

        public static double Distance(Point p1, Point p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double[,] ComputeHomography(PointF[] src, PointF[] dst)
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

        public static PointF ApplyHomography(double[,] h, double x, double y)
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

        public static double[] SolveLinearSystem(double[,] a, double[] b)
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
        #endregion

        #region Sample
        private enum KernelBoundsMode
        {
            // Preserve the original Bilinear behavior:
            // return transparent when x >= sourceWidth - 1 or y >= sourceHeight - 1.
            RequireBilinearFootprint,

            // Preserve the original Bicubic / Lanczos3 behavior:
            // allow coordinates within the source image, and clamp neighboring samples
            // that fall outside the kernel footprint by using ClampInt.
            ClampKernelFootprint
        }

        public static void SampleWithMode(
            byte[] sourceBuffer,
            int sourceStride,
            int sourceWidth,
            int sourceHeight,
            double x,
            double y,
            ResamplingMode resamplingMode,
            out byte a,
            out byte r,
            out byte g,
            out byte b)
        {
            switch (resamplingMode)
            {
                case ResamplingMode.NearestNeighbor:
                    SampleNearestNeighbor(
                        sourceBuffer,
                        sourceStride,
                        sourceWidth,
                        sourceHeight,
                        x,
                        y,
                        out a,
                        out r,
                        out g,
                        out b);
                    break;

                case ResamplingMode.Bicubic:
                    SampleSeparableKernel(
                        sourceBuffer,
                        sourceStride,
                        sourceWidth,
                        sourceHeight,
                        x,
                        y,
                        2,
                        CubicWeight,
                        KernelBoundsMode.ClampKernelFootprint,
                        out a,
                        out r,
                        out g,
                        out b);
                    break;

                case ResamplingMode.Lanczos3:
                    SampleSeparableKernel(
                        sourceBuffer,
                        sourceStride,
                        sourceWidth,
                        sourceHeight,
                        x,
                        y,
                        3,
                        Lanczos3Weight,
                        KernelBoundsMode.ClampKernelFootprint,
                        out a,
                        out r,
                        out g,
                        out b);
                    break;

                case ResamplingMode.Bilinear:
                default:
                    SampleSeparableKernel(
                        sourceBuffer,
                        sourceStride,
                        sourceWidth,
                        sourceHeight,
                        x,
                        y,
                        1, //Bilinear
                        LinearWeight,
                        KernelBoundsMode.ClampKernelFootprint,
                        out a,
                        out r,
                        out g,
                        out b);
                    break;
            }
        }

        private static void SampleNearestNeighbor(
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
                return;

            if (!double.IsFinite(x) || !double.IsFinite(y))
                return;

            int ix = (int)Math.Round(x);
            int iy = (int)Math.Round(y);

            if (ix < 0 || iy < 0 || ix >= sourceWidth || iy >= sourceHeight)
                return;

            int p = iy * sourceStride + ix * 4;

            b = sourceBuffer[p];
            g = sourceBuffer[p + 1];
            r = sourceBuffer[p + 2];
            a = sourceBuffer[p + 3];
        }

        private static void SampleSeparableKernel(
            byte[] sourceBuffer,
            int sourceStride,
            int sourceWidth,
            int sourceHeight,
            double x,
            double y,
            int radius,
            WeightFunction weightFunction,
            KernelBoundsMode boundsMode,
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
                return;

            if (!double.IsFinite(x) || !double.IsFinite(y))
                return;

            if (sourceWidth <= 0 || sourceHeight <= 0)
                return;

            switch (boundsMode)
            {
                case KernelBoundsMode.RequireBilinearFootprint:
                    // Preserve the original SampleBilinear boundary behavior:
                    // return transparent directly when x >= sourceWidth - 1
                    // or y >= sourceHeight - 1.
                    if (x < 0 || y < 0 || x >= sourceWidth - 1 || y >= sourceHeight - 1)
                        return;
                    break;

                case KernelBoundsMode.ClampKernelFootprint:
                    // Preserve the original SampleKernel boundary behavior:
                    // as long as the center coordinate is inside the image, use ClampInt
                    // for kernel samples that fall outside the image bounds.
                    if (x < 0 || y < 0 || x >= sourceWidth || y >= sourceHeight)
                        return;
                    break;
            }

            int centerX = (int)Math.Floor(x);
            int centerY = (int)Math.Floor(y);

            double sumA = 0.0;
            double sumR = 0.0;
            double sumG = 0.0;
            double sumB = 0.0;
            double sumWeight = 0.0;

            int startX = centerX - radius + 1;
            int endX = centerX + radius;
            int startY = centerY - radius + 1;
            int endY = centerY + radius;

            for (int yy = startY; yy <= endY; yy++)
            {
                double wy = weightFunction(y - yy);

                if (wy == 0.0)
                    continue;

                int sampleY = ClampInt(yy, 0, sourceHeight - 1);

                for (int xx = startX; xx <= endX; xx++)
                {
                    double wx = weightFunction(x - xx);

                    if (wx == 0.0)
                        continue;

                    double weight = wx * wy;

                    if (weight == 0.0)
                        continue;

                    int sampleX = ClampInt(xx, 0, sourceWidth - 1);
                    int p = sampleY * sourceStride + sampleX * 4;

                    sumB += sourceBuffer[p] * weight;
                    sumG += sourceBuffer[p + 1] * weight;
                    sumR += sourceBuffer[p + 2] * weight;
                    sumA += sourceBuffer[p + 3] * weight;
                    sumWeight += weight;
                }
            }

            if (Math.Abs(sumWeight) < 1e-12)
                return;

            b = (byte)ClampToByte(sumB / sumWeight);
            g = (byte)ClampToByte(sumG / sumWeight);
            r = (byte)ClampToByte(sumR / sumWeight);
            a = (byte)ClampToByte(sumA / sumWeight);
        }

        private static void SampleSupersampling2x2(
            byte[] sourceBuffer,
            int sourceStride,
            int sourceWidth,
            int sourceHeight,
            double correctedX,
            double correctedY,
            double h00,
            double h01,
            double h02,
            double h10,
            double h11,
            double h12,
            double h20,
            double h21,
            double h22,
            CropOutsideMode cropOutsideMode,
            out byte a,
            out byte r,
            out byte g,
            out byte b)
        {
            double sumA = 0.0;
            double sumPremulR = 0.0;
            double sumPremulG = 0.0;
            double sumPremulB = 0.0;

            for (int oy = 0; oy < 2; oy++)
            {
                double offsetY = oy == 0 ? -0.25 : 0.25;

                for (int ox = 0; ox < 2; ox++)
                {
                    double offsetX = ox == 0 ? -0.25 : 0.25;

                    SampleSupersamplingSubPixel(
                        sourceBuffer,
                        sourceStride,
                        sourceWidth,
                        sourceHeight,
                        correctedX + offsetX,
                        correctedY + offsetY,
                        h00,
                        h01,
                        h02,
                        h10,
                        h11,
                        h12,
                        h20,
                        h21,
                        h22,
                        cropOutsideMode,
                        ref sumA,
                        ref sumPremulR,
                        ref sumPremulG,
                        ref sumPremulB);
                }
            }

            a = (byte)ClampToByte(sumA / 4.0);

            if (sumA <= 0.0)
            {
                r = 0;
                g = 0;
                b = 0;
            }
            else
            {
                r = (byte)ClampToByte(sumPremulR * 255.0 / sumA);
                g = (byte)ClampToByte(sumPremulG * 255.0 / sumA);
                b = (byte)ClampToByte(sumPremulB * 255.0 / sumA);
            }
        }

        private static void SampleSupersamplingSubPixel(
            byte[] sourceBuffer,
            int sourceStride,
            int sourceWidth,
            int sourceHeight,
            double correctedX,
            double correctedY,
            double h00,
            double h01,
            double h02,
            double h10,
            double h11,
            double h12,
            double h20,
            double h21,
            double h22,
            CropOutsideMode cropOutsideMode,
            ref double sumA,
            ref double sumPremulR,
            ref double sumPremulG,
            ref double sumPremulB)
        {
            double denominator = h20 * correctedX + h21 * correctedY + h22;

            if (Math.Abs(denominator) < 1e-12)
                return;

            double sourceX = (h00 * correctedX + h01 * correctedY + h02) / denominator;
            double sourceY = (h10 * correctedX + h11 * correctedY + h12) / denominator;

            if (!double.IsFinite(sourceX) || !double.IsFinite(sourceY))
                return;


            if (cropOutsideMode == CropOutsideMode.Repeat || cropOutsideMode == CropOutsideMode.Mirror)
            {
                ApplyCropOutsideMode(
                cropOutsideMode,
                sourceWidth,
                sourceHeight,
                ref sourceX,
                ref sourceY);
            }
            else
            {
                sourceX = Math.Max(0.0, Math.Min(sourceWidth - 1, sourceX));
                sourceY = Math.Max(0.0, Math.Min(sourceHeight - 1, sourceY));
            }

            SampleSeparableKernel(
                sourceBuffer,
                sourceStride,
                sourceWidth,
                sourceHeight,
                sourceX,
                sourceY,
                1, //Bilinear
                LinearWeight,
                KernelBoundsMode.ClampKernelFootprint,
                out byte sampleA,
                out byte sampleR,
                out byte sampleG,
                out byte sampleB);

            double alpha = sampleA / 255.0;

            sumA += sampleA;
            sumPremulR += sampleR * alpha;
            sumPremulG += sampleG * alpha;
            sumPremulB += sampleB * alpha;
        }

        #region CropOutsideMode
        private static void ApplyCropOutsideMode(
            CropOutsideMode cropOutsideMode,
            int sourceWidth,
            int sourceHeight,
            ref double sourceX,
            ref double sourceY)
        {
            if (sourceWidth <= 0 || sourceHeight <= 0)
                return;

            switch (cropOutsideMode)
            {
                case CropOutsideMode.Repeat:
                    sourceX = RepeatCoordinate(sourceX, sourceWidth);
                    sourceY = RepeatCoordinate(sourceY, sourceHeight);
                    break;

                case CropOutsideMode.Mirror:
                    sourceX = MirrorCoordinate(sourceX, sourceWidth);
                    sourceY = MirrorCoordinate(sourceY, sourceHeight);
                    break;
            }
        }

        private static double RepeatCoordinate(double value, int length)
        {
            if (length <= 1)
                return 0;

            double result = value % length;
            if (result < 0)
                result += length;

            return result;
        }

        private static double MirrorCoordinate(double value, int length)
        {
            if (length <= 1)
                return 0;

            double period = (length - 1) * 2.0;
            double result = value % period;

            if (result < 0)
                result += period;

            if (result > length - 1)
                result = period - result;

            return result;
        }
        #endregion


        private delegate double WeightFunction(double distance);

        private static double CubicWeight(double x)
        {
            // Catmull-Rom bicubic, a = -0.5
            const double a = -0.5;

            x = Math.Abs(x);

            if (x <= 1)
                return ((a + 2) * x * x * x) - ((a + 3) * x * x) + 1;

            if (x < 2)
                return (a * x * x * x) - (5 * a * x * x) + (8 * a * x) - (4 * a);

            return 0;
        }

        private static double Lanczos3Weight(double x)
        {
            x = Math.Abs(x);

            if (x < 1e-12)
                return 1;

            if (x >= 3)
                return 0;

            return Sinc(x) * Sinc(x / 3);
        }

        private static double LinearWeight(double distance)
        {
            distance = Math.Abs(distance);
            return distance < 1.0 ? 1.0 - distance : 0.0;
        }

        private static double Sinc(double x)
        {
            double pix = Math.PI * x;

            if (Math.Abs(pix) < 1e-12)
                return 1;

            return Math.Sin(pix) / pix;
        }

        private static int ClampInt(int value, int min, int max)
        {
            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
        }

        public static int ClampToByte(double value)
        {
            if (value < 0)
                return 0;

            if (value > 255)
                return 255;

            return (int)Math.Round(value);
        }
        #endregion
    }
}
