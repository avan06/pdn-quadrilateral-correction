using PaintDotNet;
using PaintDotNet.Imaging;
using System;
using System.Drawing;

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

        public static void DrawBgra32Data(
            BitmapBgra32Data source,
            BitmapBgra32Data destination,
            Point offSet,
            CropOutsideMode cropOutsideMode)
        {
            if (cropOutsideMode != CropOutsideMode.Repeat &&
                cropOutsideMode != CropOutsideMode.Mirror)
            {
                DrawBgra32Data(source, destination, offSet);
                return;
            }

            if (source == null || destination == null)
                return;

            if (source.Width <= 0 || source.Height <= 0 ||
                destination.Width <= 0 || destination.Height <= 0)
                return;

            for (int y = 0; y < destination.Height; y++)
            {
                int sourceY = y - offSet.Y;

                if (cropOutsideMode == CropOutsideMode.Repeat)
                    sourceY = RepeatCoordinate(sourceY, source.Height);
                else
                    sourceY = MirrorCoordinate(sourceY, source.Height);

                int sourceRowY = source.StridePositive
                    ? sourceY
                    : source.Height - 1 - sourceY;

                int destinationRowY = destination.StridePositive
                    ? y
                    : destination.Height - 1 - y;

                int sourceRowOffset = sourceRowY * source.StrideAbs;
                int destinationRowOffset = destinationRowY * destination.StrideAbs;

                for (int x = 0; x < destination.Width; x++)
                {
                    int sourceX = x - offSet.X;

                    if (cropOutsideMode == CropOutsideMode.Repeat)
                        sourceX = RepeatCoordinate(sourceX, source.Width);
                    else
                        sourceX = MirrorCoordinate(sourceX, source.Width);

                    int sourceOffset = sourceRowOffset + sourceX * 4;
                    int destinationOffset = destinationRowOffset + x * 4;

                    destination.Buffer[destinationOffset + 0] = source.Buffer[sourceOffset + 0];
                    destination.Buffer[destinationOffset + 1] = source.Buffer[sourceOffset + 1];
                    destination.Buffer[destinationOffset + 2] = source.Buffer[sourceOffset + 2];
                    destination.Buffer[destinationOffset + 3] = source.Buffer[sourceOffset + 3];
                }
            }
        }

        private static int RepeatCoordinate(int value, int length)
        {
            if (length <= 1)
                return 0;

            int result = value % length;

            if (result < 0)
                result += length;

            return result;
        }

        private static int MirrorCoordinate(int value, int length)
        {
            if (length <= 1)
                return 0;

            int period = (length - 1) * 2;
            int result = value % period;

            if (result < 0)
                result += period;

            if (result > length - 1)
                result = period - result;

            return result;
        }
    }
}
