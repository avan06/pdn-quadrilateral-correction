using PaintDotNet;
using PaintDotNet.Imaging;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace QuadrilateralCorrectionEffect
{
    internal static class BitmapRegionUtil
    {
        internal sealed class BitmapBgra32Data
        {
            public readonly byte[] Buffer;
            public readonly int Width;
            public readonly int Height;
            public readonly int StrideAbs;
            public readonly bool StridePositive;

            public BitmapBgra32Data(
                byte[] buffer,
                int width,
                int height,
                int strideAbs,
                bool stridePositive)
            {
                Buffer = buffer;
                Width = width;
                Height = height;
                StrideAbs = strideAbs;
                StridePositive = stridePositive;
            }
        }

        public static BitmapBgra32Data CreateBgra32DataFromBitmap(Bitmap bitmap)
        {
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            BitmapData bitmapData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                int stride = bitmapData.Stride;
                int strideAbs = Math.Abs(stride);
                byte[] buffer = new byte[strideAbs * bitmap.Height];

                Marshal.Copy(bitmapData.Scan0, buffer, 0, buffer.Length);

                return new BitmapBgra32Data(
                    buffer,
                    bitmap.Width,
                    bitmap.Height,
                    strideAbs,
                    stride > 0);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        public static Bitmap CreateBitmapFromSourceRegion(
            RegionPtr<ColorBgra32> sourceRegion,
            int width,
            int height)
        {
            return CreateBitmapFromSourceRegion(
                sourceRegion,
                0,
                0,
                width,
                height);
        }

        public static Bitmap CreateBitmapFromSourceRegion(
            RegionPtr<ColorBgra32> sourceRegion,
            int sourceLeft,
            int sourceTop,
            int width,
            int height)
        {
            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            return CreateBitmapFromSourceRegion(bitmap, bitmapData, sourceRegion, sourceLeft, sourceTop, width, height);
        }

        private static Bitmap CreateBitmapFromSourceRegion(
            Bitmap bitmap,
            BitmapData bitmapData,
            RegionPtr<ColorBgra32> sourceRegion,
            int sourceLeft,
            int sourceTop,
            int width,
            int height)
        {
            try
            {
                int stride = bitmapData.Stride;
                int strideAbs = Math.Abs(stride);
                byte[] buffer = new byte[strideAbs * height];

                for (int y = 0; y < height; y++)
                {
                    int targetY = stride > 0 ? y : height - 1 - y;
                    int rowOffset = targetY * strideAbs;

                    for (int x = 0; x < width; x++)
                    {
                        ColorBgra32 pixel = sourceRegion[sourceLeft + x, sourceTop + y];

                        int offset = rowOffset + (x * 4);

                        /*
                         * PixelFormat.Format32bppArgb 在記憶體中實際順序是 BGRA。
                         */
                        buffer[offset + 0] = pixel.B;
                        buffer[offset + 1] = pixel.G;
                        buffer[offset + 2] = pixel.R;
                        buffer[offset + 3] = pixel.A;
                    }
                }

                Marshal.Copy(
                    buffer,
                    0,
                    bitmapData.Scan0,
                    buffer.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            return bitmap;
        }
    }
}
