using AForge.Video.DirectShow.Internals;
using AForge.Video;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace HadwareRemoteControl
{
    public static class FormatsDecoder
    {
        [DllImport( "ntdll.dll", CallingConvention = CallingConvention.Cdecl )]
        public static unsafe extern int memcpy(
            byte* dst,
            byte* src,
            int count );

        public static Bitmap NV12ToBitmap(byte[] buffer, int width, int height)
        {
            // Lock the buffer data for reading
            unsafe
            {
                //byte* nv12Ptr = (byte *)buffer [0];

                int frameSize = width * height;
                int chromaHeight = height / 2;

                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

                BitmapData bmpData = bmp.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format24bppRgb
                );

                byte* rgbPtr = (byte*)bmpData.Scan0;

                for (int y = 0; y < height; y++)
                {
                    int yStride = (y * bmpData.Stride);
                    int y2Width = (y / 2) * width;
                    int yWidth = y * width;
                    int frameSizeY2Width = frameSize + y2Width;
                    for (int x = 0; x < width; x++)
                    {
                        int yIndex = yWidth + x;

                        int uvIndex = frameSizeY2Width + (x & ~1);

                        int Y = buffer[yIndex];
                        int U = buffer[uvIndex + 0] - 128;
                        int V = buffer[uvIndex + 1] - 128;

                        int C = Y - 16;
                        int D = U;
                        int E = V;

                        int R = (298 * C + 409 * E + 128) >> 8;
                        int G = (298 * C - 100 * D - 208 * E + 128) >> 8;
                        int B = (298 * C + 516 * D + 128) >> 8;

                        R = R > 255 ? 255 : (R < 0 ? 0 : R);
                        G = G > 255 ? 255 : (G < 0 ? 0 : G);
                        B = B > 255 ? 255 : (B < 0 ? 0 : B);

                        int pixelIndex = yStride + (x * 3);

                        rgbPtr[pixelIndex] = (byte)B;
                        rgbPtr[pixelIndex + 1] = (byte)G;
                        rgbPtr[pixelIndex + 2] = (byte)R;
                    }
                }

                bmp.UnlockBits(bmpData);

                return bmp;
            }
        }

        public static Bitmap ConvertYUYVToBitmap(byte[] buffer, int width, int height)
        {
            int bufferLen = buffer.Length;
            if (bufferLen < width * height * 2)
            {
                throw new ArgumentException("Buffer length is too short for the given dimensions.");
            }

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int stride = bitmapData.Stride;
            IntPtr scan0 = bitmapData.Scan0;
            int i = 0;

            unsafe
            {
                byte* rgbPtr = (byte*)scan0.ToPointer();
                //byte* yuyvPtr = (byte *)buffer [0];
                //int yuyvIndex = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x += 2)
                    {
                        int y0 = buffer[i];
                        int u = buffer[i + 1];
                        int y1 = buffer[i + 2];
                        int v = buffer[i + 3];
                        i += 4;

                        YuvToRgb(y0, u, v, out byte r0, out byte g0, out byte b0);
                        YuvToRgb(y1, u, v, out byte r1, out byte g1, out byte b1);

                        int index = y * stride + x * 3;
                        rgbPtr[index] = b0;
                        rgbPtr[index + 1] = g0;
                        rgbPtr[index + 2] = r0;
                        if (x + 1 < width)
                        {
                            rgbPtr[index + 3] = b1;
                            rgbPtr[index + 4] = g1;
                            rgbPtr[index + 5] = r1;
                        }
                    }
                }
            }

            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        private static void YuvToRgb(int y, int u, int v, out byte r, out byte g, out byte b)
        {
            int c = y - 16;
            int d = u - 128;
            int e = v - 128;

            int clip(int val)
            {
                return (val < 0) ? 0 : (val > 255) ? 255 : val;
            }

            r = (byte)clip((298 * c + 409 * e + 128) >> 8);
            g = (byte)clip((298 * c - 100 * d - 208 * e + 128) >> 8);
            b = (byte)clip((298 * c + 516 * d + 128) >> 8);
        }


        public static Bitmap ConvertYUY2ToBitmap(byte[] buffer, int width, int height)
        {
            int bufferLen = buffer.Length;
            if (bufferLen != width * height * 2)
                throw new ArgumentException("Invalid buffer length for given width and height");

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            // Precompute some constant factors
            /*
            const float yFactor = 1.164f;
            const float uFactorB = 2.018f;
            const float vFactorR = 1.596f;
            const float uFactorG = -0.391f;
            const float vFactorG = -0.813f;
            */
            // Constants for YUV to RGB conversion in fixed-point arithmetic
            /*
            const int yFactor = 1192; // 1.164 x 1024
            const int uFactorG = -201; // -0.391 x 1024
            const int vFactorG = -400; // -0.813 x 1024
            const int uFactorB = 2066; // 2.018 x 1024
            const int vFactorR = 1634; // 1.596 x 1024
            */

            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height),
                                                    ImageLockMode.WriteOnly,
                                                    bitmap.PixelFormat);

            unsafe
            {
                //byte* pYUY2 = buffer [0];
                int iYUY2 = 0;
                byte* pBmp = (byte*)bmpData.Scan0;

                for (int y = 0; y < height; y++)
                {
                    int yWidth = (y * width);
                    int idx = yWidth * 3;
                    for (int x = 0; x < width; x += 2)
                    {
                        /*
                        int y0 = pYUY2[0];
                        int u = pYUY2[1] - 128;
                        int y1 = pYUY2[2];
                        int v = pYUY2[3] - 128;
                        //*/
                        //*
                        int y0 = buffer[iYUY2++];
                        int u = buffer[iYUY2++] - 128;
                        int y1 = buffer[iYUY2++];
                        int v = buffer[iYUY2++] - 128;
                        //*/

                        // Precompute common terms
                        //float uFactor = uFactorG * u;
                        //float vFactor = vFactorG * v;

                        // YUV to RGB for first pixel
                        int c0 = y0 - 16;
                        int c1 = y1 - 16;
                        int d = u;
                        int e = v;
                        /*
                        int c = y0 - 16 * 1024;
                        int ri1 = (c + vFactorR * v) >> 10;
                        int gi1 = (c + uFactorG * u + vFactorG * v) >> 10;
                        int bi1 = (c + uFactorB * u) >> 10;
                        */
                        int ri1 = (298 * c0 + 409 * e + 128) >> 8;
                        int gi1 = (298 * c0 - 100 * d - 208 * e + 128) >> 8;
                        int bi1 = (298 * c0 + 516 * d + 128) >> 8;
                        /*
                        int ri1 = (int)(yFactor * y0 + vFactorR * v);
                        int gi1 = (int)(yFactor * y0 + uFactor + vFactor);
                        int bi1 = (int)(yFactor * y0 + uFactorB * u);
                        */

                        byte r1 = (byte)(ri1 < 0 ? 0 : (ri1 > 255 ? 255 : ri1)); //Math.Min(255, Math.Max(0, yFactor * y0 + vFactorR * v));
                        byte g1 = (byte)(gi1 < 0 ? 0 : (gi1 > 255 ? 255 : gi1));//Math.Min(255, Math.Max(0, yFactor * y0 + uFactor + vFactor));
                        byte b1 = (byte)(bi1 < 0 ? 0 : (bi1 > 255 ? 255 : bi1));//Math.Min(255, Math.Max(0, yFactor * y0 + uFactorB * u));

                        // Set pixel (x)
                        /*
                        int idx1 = (yWidth + x) * 3;
                        pBmp[idx1] = b1;
                        pBmp[idx1 + 1] = g1;
                        pBmp[idx1 + 2] = r1;
                        */
                        pBmp[idx++] = b1;
                        pBmp[idx++] = g1;
                        pBmp[idx++] = r1;


                        /*
                        int c = y1 - 16 * 1024;
                        int ri2 = (c + vFactorR * v) >> 10;
                        int gi2 = (c + uFactorG * u + vFactorG * v) >> 10;
                        int bi2 = (c + uFactorB * u) >> 10;
                        */
                        /*
                        int ri2 = (int)(yFactor * y1 + vFactorR * v);
                        int gi2 = (int)(yFactor * y1 + uFactor + vFactor);
                        int bi2 = (int)(yFactor * y1 + uFactorB * u);
                        */
                        int ri2 = (298 * c1 + 409 * e + 128) >> 8;
                        int gi2 = (298 * c1 - 100 * d - 208 * e + 128) >> 8;
                        int bi2 = (298 * c1 + 516 * d + 128) >> 8;


                        // YUV to RGB for second pixel
                        byte r2 = (byte)(ri2 < 0 ? 0 : (ri2 > 255 ? 255 : ri2));//Math.Min(255, Math.Max(0, yFactor * y1 + vFactorR * v));
                        byte g2 = (byte)(gi2 < 0 ? 0 : (gi2 > 255 ? 255 : gi2));//Math.Min(255, Math.Max(0, yFactor * y1 + uFactor + vFactor));
                        byte b2 = (byte)(bi2 < 0 ? 0 : (bi2 > 255 ? 255 : bi2));//Math.Min(255, Math.Max(0, yFactor * y1 + uFactorB * u));

                        // Set pixel (x+1)
                        /*
                        int idx2 = (yWidth + x + 1) * 3;
                        pBmp[idx2] = b2;
                        pBmp[idx2 + 1] = g2;
                        pBmp[idx2 + 2] = r2;
                        */
                        pBmp[idx++] = b2;
                        pBmp[idx++] = g2;
                        pBmp[idx++] = r2;
                            
                        // Move to the next group of 4 bytes
                        //pYUY2 += 4;
                    }
                }
            }

            bitmap.UnlockBits(bmpData);

            return bitmap;
        }

        public static Bitmap ConvertI420ToBitmap(byte[] buffer, int width, int height)
        {
            int frameSize = width * height;
            int chromaSize = frameSize / 4;

            // Allocate arrays for Y, U, and V planes
            byte[] yPlane = new byte[frameSize];
            byte[] uPlane = new byte[chromaSize];
            byte[] vPlane = new byte[chromaSize];

            // Copy the Y plane
            buffer.CopyTo(yPlane, 0);
            //Marshal.Copy(buffer, yPlane, 0, frameSize);
            // Copy the U plane
            Array.Copy(buffer, frameSize, uPlane, 0, chromaSize);
            //Marshal.Copy(new IntPtr(buffer.ToInt64() + frameSize), uPlane, 0, chromaSize);
            // Copy the V plane
            Array.Copy(buffer, frameSize + chromaSize, vPlane, 0, chromaSize);
            //Marshal.Copy(new IntPtr(buffer.ToInt64() + frameSize + chromaSize), vPlane, 0, chromaSize);

            // Create a byte array to hold the RGB data
            byte[] rgbData = new byte[frameSize * 3];

            // Convert I420 to RGB
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int yIndex = y * width + x;
                    int uIndex = (y / 2) * (width / 2) + (x / 2);
                    int vIndex = uIndex;

                    int Y = yPlane[yIndex];
                    int U = uPlane[uIndex] - 128;
                    int V = vPlane[vIndex] - 128;

                    // Convert YUV to RGB
                    int R = (int)(Y + 1.402 * V);
                    int G = (int)(Y - 0.344136 * U - 0.714136 * V);
                    int B = (int)(Y + 1.772 * U);

                    // Clamp values to byte range
                    R = Math.Max(0, Math.Min(255, R));
                    G = Math.Max(0, Math.Min(255, G));
                    B = Math.Max(0, Math.Min(255, B));

                    int rgbIndex = yIndex * 3;
                    rgbData[rgbIndex] = (byte)B;
                    rgbData[rgbIndex + 1] = (byte)G;
                    rgbData[rgbIndex + 2] = (byte)R;
                }
            }

            // Create a bitmap
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            // Copy RGB data to bitmap
            Marshal.Copy(rgbData, 0, bitmapData.Scan0, rgbData.Length);
            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }

        public static Bitmap Decode(byte[] frame, Guid formatSubtype, int width, int height)
        {
            Bitmap image = null;
            if (formatSubtype == MediaSubType.NV12)
            {
                image = NV12ToBitmap(frame, width, height);
            }
            else if (formatSubtype == MediaSubType.YUYV)
            {
                image = ConvertYUYVToBitmap(frame, width, height);
            }
            else if (formatSubtype == MediaSubType.YUY2)
            {
                image = ConvertYUY2ToBitmap(frame, width, height);
            }
            else if (formatSubtype == MediaSubType.I420)
            {
                image = ConvertI420ToBitmap(frame, width, height);
            }
            else if (formatSubtype == MediaSubType.RGB24)
            {
                // create new image
                image = new Bitmap(width, height, PixelFormat.Format24bppRgb);

                // lock bitmap data
                BitmapData imageData = image.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                // copy image data
                int srcStride = imageData.Stride;
                int dstStride = imageData.Stride;

                unsafe
                {
                    //byte* dst = (byte*)imageData.Scan0.ToPointer() + dstStride * (height - 1);
                    var dst = imageData.Scan0 + dstStride * (height - 1);
                    //byte* src = (byte*)frame[0];
                    int src = 0;

                    for (int y = 0; y < height; y++)
                    {
                        Marshal.Copy(frame, src, dst, srcStride);
                        //memcpy(dst, src, srcStride);
                        dst -= dstStride;
                        src += srcStride;
                    }
                }

                // unlock bitmap data
                image.UnlockBits(imageData);
            }
            else if (formatSubtype == MediaSubType.MJpeg)
            {
                if (frame.Length > 4)
                {
                    try
                    {
                            using (var ms = new MemoryStream(frame))
                            {
                                image = new Bitmap(ms);
                            }

                            //image = (Bitmap)Bitmap.FromStream(new MemoryStream(frame));
                                //new UnmanagedMemoryStream((byte*)buffer.ToPointer(), bufferLen));
                    }
                    catch { }
                }
            }
            else
            {
                throw new Exception($"Format {formatSubtype} not supported");
            }
            return image;
        }
    }
}
