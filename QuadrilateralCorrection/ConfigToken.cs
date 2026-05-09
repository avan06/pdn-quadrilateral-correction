using System.Drawing;
using PaintDotNet.Effects;

namespace QuadrilateralCorrectionEffect
{
    internal enum ResamplingMode
    {
        NearestNeighbor = 0,
        Bilinear = 1,
        Bicubic = 2,
        Lanczos3 = 3,
        HighQualitySupersampling = 4
    }

    internal enum CropOutsideMode
    {
        Crop = 0,
        Preserve = 1,
        Repeat = 2,
        Mirror = 3
    }

    internal class QuadrilateralCorrectionConfigToken : EffectConfigToken
    {
        internal QuadrilateralCorrectionConfigToken()
        {
            TopLeft = new Point(0, 0);
            TopRight = new Point(int.MaxValue, 0);
            BottomRight = new Point(int.MaxValue, int.MaxValue);
            BottomLeft = new Point(0, int.MaxValue);
            AutoDims = true;
            Width = int.MaxValue;
            Height = int.MaxValue;
            Center = true;
            ResamplingMode = ResamplingMode.Bilinear;
            CropOutsideMode = CropOutsideMode.Crop; // Remove content outside the quadrilateral by default
        }

        private QuadrilateralCorrectionConfigToken(QuadrilateralCorrectionConfigToken copyMe)
        {
            TopLeft = copyMe.TopLeft;
            TopRight = copyMe.TopRight;
            BottomRight = copyMe.BottomRight;
            BottomLeft = copyMe.BottomLeft;
            AutoDims = copyMe.AutoDims;
            Width = copyMe.Width;
            Height = copyMe.Height;
            Center = copyMe.Center;
            ResamplingMode = copyMe.ResamplingMode;
            CropOutsideMode = copyMe.CropOutsideMode;
        }

        public override object Clone()
        {
            return new QuadrilateralCorrectionConfigToken(this);
        }

        internal Point TopLeft { get; set; }
        internal Point TopRight { get; set; }
        internal Point BottomRight { get; set; }
        internal Point BottomLeft { get; set; }
        internal bool AutoDims { get; set; }
        internal int Width { get; set; }
        internal int Height { get; set; }
        internal ResamplingMode ResamplingMode { get; set; }
        internal CropOutsideMode CropOutsideMode { get; set; }
        internal bool Center { get; set; }
    }
}