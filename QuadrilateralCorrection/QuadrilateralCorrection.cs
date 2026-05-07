using System;
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;
using AForge.Imaging.Filters;
using PaintDotNet;
using PaintDotNet.Effects;
using PaintDotNet.Imaging;
using PaintDotNet.Rendering;

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
    internal class QuadrilateralCorrectionEffectPlugin : BitmapEffect<QuadrilateralCorrectionConfigToken>
    {
        private static readonly Image StaticIcon = new Bitmap(typeof(QuadrilateralCorrectionEffectPlugin), "Icon.png");

        public QuadrilateralCorrectionEffectPlugin()
            : base("Quadrilateral Correction", StaticIcon, "Tools", BitmapEffectOptions.Create() with { IsConfigurable = true })
        {
        }

        protected override IEffectConfigForm OnCreateConfigForm()
        {
            return new QuadrilateralCorrectionConfigDialog();
        }

        protected override void OnInitializeRenderInfo(IBitmapEffectRenderInfo renderInfo)
        {
            base.OnInitializeRenderInfo(renderInfo);
        }

        protected override void OnSetToken(QuadrilateralCorrectionConfigToken newToken)
        {
            base.OnSetToken(newToken);

            if (newToken == null) return;

            topLeft = newToken.TopLeft;
            topRight = newToken.TopRight;
            bottomRight = newToken.BottomRight;
            bottomLeft = newToken.BottomLeft;
            autoDims = newToken.AutoDims;
            width = newToken.Width;
            height = newToken.Height;
            center = newToken.Center;
            cropOutsideQuadrilateral = newToken.CropOutsideQuadrilateral;

            var sourceQuadrilateral = new List<AForge.IntPoint>
            {
                new AForge.IntPoint(topLeft.X, topLeft.Y),
                new AForge.IntPoint(topRight.X, topRight.Y),
                new AForge.IntPoint(bottomRight.X, bottomRight.Y),
                new AForge.IntPoint(bottomLeft.X, bottomLeft.Y)
            };

            // Use Environment.Selection to get the selection bounds
            RectInt32 renderBounds = Environment.Selection.RenderBounds;
            Rectangle selection = new Rectangle(renderBounds.X, renderBounds.Y, renderBounds.Width, renderBounds.Height);

            Bitmap quadTransOutput;
            Point preserveOutsideOffset = Point.Empty;
            Size preserveOutsideSize = Size.Empty;

            try
            {
                // Read the image using the PDNv5 API and convert it to a GDI+ Bitmap for the existing logic
                IEffectInputBitmap<ColorBgra32> sourceBitmap = Environment.GetSourceBitmapBgra32();
                using IBitmapLock<ColorBgra32> sourceLock = sourceBitmap.Lock(new RectInt32(0, 0, sourceBitmap.Size));
                RegionPtr<ColorBgra32> sourceRegion = sourceLock.AsRegionPtr();

                using Bitmap tempBmp =
                    BitmapRegionUtil.CreateBitmapFromSourceRegion(
                        sourceRegion,
                        sourceBitmap.Size.Width,
                        sourceBitmap.Size.Height);

                using Bitmap srcImage = new Bitmap(tempBmp);

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
                    quadTransOutput = PerspectiveWarpUtil.PerspectiveWarpPreserveOutside(
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

                // Use Environment.Document.Size for the canvas size
                offSet = PerspectiveWarpUtil.FitOffsetInsideCanvas(
                    offSet,
                    quadTransOutput.Size,
                    new Size(Environment.Document.Size.Width, Environment.Document.Size.Height));
            }

            Bitmap alignedImage = new Bitmap(Environment.Document.Size.Width, Environment.Document.Size.Height);
            using (Graphics graphics = Graphics.FromImage(alignedImage))
            {
                graphics.DrawImage(quadTransOutput, offSet);
            }
            quadTransOutput.Dispose();

            Bitmap newQuadrilateralSurface = new Bitmap(alignedImage);
            BitmapRegionUtil.BitmapBgra32Data newQuadrilateralSurfaceData =
                BitmapRegionUtil.CreateBgra32DataFromBitmap(newQuadrilateralSurface);

            // Update the preprocessed full-size image
            if (quadrilateralSurface != null)
            {
                quadrilateralSurface.Dispose();
            }

            quadrilateralSurface = newQuadrilateralSurface;
            quadrilateralSurfaceData = newQuadrilateralSurfaceData;

            alignedImage.Dispose();
        }

        // In PDNv5, OnRender uses concurrent tile rendering, so only the corresponding slice from the precomputed quadrilateralSurface needs to be copied
        protected override void OnRender(IBitmapEffectOutput output)
        {
            BitmapRegionUtil.BitmapBgra32Data surfaceData = quadrilateralSurfaceData;

            if (surfaceData == null)
            {
                return;
            }

            RectInt32 outputBounds = output.Bounds;

            using IBitmapLock<ColorBgra32> outputLock = output.LockBgra32();
            RegionPtr<ColorBgra32> outputSubRegion = outputLock.AsRegionPtr();
            RegionPtrOffsetView<ColorBgra32> outputRegion =
                outputSubRegion.OffsetView(-outputBounds.Location);

            for (int y = outputBounds.Top; y < outputBounds.Bottom; y++)
            {
                if (IsCancelRequested)
                {
                    return;
                }

                if (y < 0 || y >= surfaceData.Height)
                {
                    continue;
                }

                int sourceY = surfaceData.StridePositive
                    ? y
                    : surfaceData.Height - 1 - y;

                int rowOffset = sourceY * surfaceData.StrideAbs;

                for (int x = outputBounds.Left; x < outputBounds.Right; x++)
                {
                    if (x < 0 || x >= surfaceData.Width)
                    {
                        continue;
                    }

                    int offset = rowOffset + (x * 4);

                    outputRegion[x, y] = new ColorBgra32
                    {
                        B = surfaceData.Buffer[offset + 0],
                        G = surfaceData.Buffer[offset + 1],
                        R = surfaceData.Buffer[offset + 2],
                        A = surfaceData.Buffer[offset + 3]
                    };
                }
            }
        }

        // Clean up the generated resources when the Effect is disposed
        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                if (quadrilateralSurface != null)
                {
                    quadrilateralSurface.Dispose();
                    quadrilateralSurface = null;
                }

                quadrilateralSurfaceData = null;
            }

            base.OnDispose(disposing);
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

        // In PDNv5, Surface has been replaced with Bitmap
        private Bitmap quadrilateralSurface;

        private BitmapRegionUtil.BitmapBgra32Data quadrilateralSurfaceData;
    }
}
