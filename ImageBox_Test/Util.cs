using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ImageBox_Test {
    public class Util {
        // Load Bitmap to buffer
        public unsafe static void BitmapToImageBuffer(Bitmap bmp, ref IntPtr imgBuf, ref int bw, ref int bh, ref int bytepp) {
            bw = bmp.Width;
            bh = bmp.Height;
            if (bmp.PixelFormat == PixelFormat.Format8bppIndexed)
                bytepp = 1;
            else if (bmp.PixelFormat == PixelFormat.Format16bppGrayScale)
                bytepp = 2;
            else if (bmp.PixelFormat == PixelFormat.Format24bppRgb)
                bytepp = 3;
            else if (bmp.PixelFormat == PixelFormat.Format32bppRgb || bmp.PixelFormat == PixelFormat.Format32bppArgb || bmp.PixelFormat == PixelFormat.Format32bppPArgb)
                bytepp = 4;
            long bufSize = (long)bw * bh * bytepp;
            imgBuf = AllocBuffer(bufSize);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bw, bh), ImageLockMode.ReadOnly, bmp.PixelFormat);
            int copySize = bw * bytepp;
            for (int y = 0; y < bh; y++) {
                IntPtr dstPtr = (IntPtr)((long)imgBuf + bw * y * bytepp);
                IntPtr srcPtr = (IntPtr)((long)bmpData.Scan0 + bmpData.Stride * y);
                Memcpy(dstPtr, srcPtr, copySize);
            }

            bmp.UnlockBits(bmpData);
        }

        public static unsafe IntPtr Memcpy(IntPtr _Dst, IntPtr _Src, Int64 _Size) {
            Int64 size4 = _Size / 4;
            Int64 size1 = _Size % 4;

            int* pdst4 = (int*)_Dst;
            int* psrc4 = (int*)_Src;
            while (size4-- > 0)
                *pdst4++ = *psrc4++;

            byte* pdst1 = (byte*)pdst4;
            byte* psrc1 = (byte*)psrc4;
            while (size1-- > 0)
                *pdst1++ = *psrc1++;

            return _Dst;
        }

        public static unsafe IntPtr Memset(IntPtr _Dst, int _Val, Int64 _Size) {
            Int64 size4 = _Size / 4;
            Int64 size1 = _Size % 4;

            int val4 = _Val | _Val << 8 | _Val << 16 | _Val << 24;
            byte val1 = (byte)_Val;

            int* pdst4 = (int*)_Dst;
            while (size4-- > 0)
                *pdst4++ = val4;

            byte* pdst1 = (byte*)pdst4;
            while (size1-- > 0)
                *pdst1++ = val1;

            return _Dst;
        }

        public static unsafe IntPtr Memset4(IntPtr _Dst, int _Val, Int64 _Size) {
            int* pdst = (int*)_Dst;
            while (_Size-- > 0)
                *pdst++ = _Val;
            return _Dst;
        }

        // free and set null
        public static void FreeBuffer(ref IntPtr buf) {
            if (buf != IntPtr.Zero) {
                Marshal.FreeHGlobal(buf);
                buf = IntPtr.Zero;
            }
        }

        public static IntPtr AllocBuffer(Int64 size) {
            IntPtr buf = Marshal.AllocHGlobal((IntPtr)size);
            Memset(buf, 0, size);
            return buf;
        }
    }
}
