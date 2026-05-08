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
            public readonly byte[] Buffer;   // BGRA, packed
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

        public static BitmapBgra32Data CreateBgra32Data(int width, int height)
        {
            int stride = checked(width * 4);
            return new BitmapBgra32Data(new byte[checked(stride * height)], width, height, stride, true);
        }

        public static BitmapBgra32Data CreateBgra32DataFromSourceRegion(
            RegionPtr<ColorBgra32> sourceRegion,
            int width,
            int height)
        {
            return CreateBgra32DataFromSourceRegion(sourceRegion, 0, 0, width, height);
        }

        public static BitmapBgra32Data CreateBgra32DataFromSourceRegion(
            RegionPtr<ColorBgra32> sourceRegion,
            int sourceLeft,
            int sourceTop,
            int width,
            int height)
        {
            int stride = checked(width * 4);
            byte[] buffer = new byte[checked(stride * height)];

            for (int y = 0; y < height; y++)
            {
                int rowOffset = y * stride;

                for (int x = 0; x < width; x++)
                {
                    ColorBgra32 pixel = sourceRegion[sourceLeft + x, sourceTop + y];
                    int offset = rowOffset + (x * 4);

                    buffer[offset + 0] = pixel.B;
                    buffer[offset + 1] = pixel.G;
                    buffer[offset + 2] = pixel.R;
                    buffer[offset + 3] = pixel.A;
                }
            }

            return new BitmapBgra32Data(buffer, width, height, stride, true);
        }

        public static void DrawBgra32Data(
            BitmapBgra32Data source,
            BitmapBgra32Data destination,
            Point offSet)
        {
            int startX = Math.Max(0, offSet.X);
            int startY = Math.Max(0, offSet.Y);
            int endX = Math.Min(destination.Width, offSet.X + source.Width);
            int endY = Math.Min(destination.Height, offSet.Y + source.Height);

            for (int y = startY; y < endY; y++)
            {
                int sourceY = y - offSet.Y;
                int sourceBufferY = source.StridePositive ? sourceY : source.Height - 1 - sourceY;
                int destinationBufferY = destination.StridePositive ? y : destination.Height - 1 - y;

                int sourceRowOffset = sourceBufferY * source.StrideAbs;
                int destinationRowOffset = destinationBufferY * destination.StrideAbs;

                for (int x = startX; x < endX; x++)
                {
                    int sourceX = x - offSet.X;
                    int sourceOffset = sourceRowOffset + (sourceX * 4);
                    int destinationOffset = destinationRowOffset + (x * 4);

                    destination.Buffer[destinationOffset + 0] = source.Buffer[sourceOffset + 0];
                    destination.Buffer[destinationOffset + 1] = source.Buffer[sourceOffset + 1];
                    destination.Buffer[destinationOffset + 2] = source.Buffer[sourceOffset + 2];
                    destination.Buffer[destinationOffset + 3] = source.Buffer[sourceOffset + 3];
                }
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
