using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimLib {
    public class ImageUtil {

        public unsafe static void BitmapToGrayImageBuffer(Bitmap bmp, ref IntPtr imgBuf, ref int bw, ref int bh, ref int bytepp) {
            if (bmp.PixelFormat == PixelFormat.Format1bppIndexed) {
                Bitmap1BitToGrayImageBuffer(bmp, ref imgBuf, ref bw, ref bh, ref bytepp);
                return;
            }

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
            Int64 bufSize = (Int64)bw * bh;
            imgBuf = Util.AllocBuffer(bufSize);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bw, bh), ImageLockMode.ReadOnly, bmp.PixelFormat);
            for (int y = 0; y < bh; y++) {
                byte* dstPtr = (byte*)imgBuf + bw * y;
                byte* srcPtr = (byte*)bmpData.Scan0 + bmpData.Stride * y;
                for (int x = 0; x < bw; x++, dstPtr++, srcPtr += bytepp) {
                    byte gray = 0;
                    if (bytepp == 1) {
                        gray = *srcPtr;
                    } else if (bytepp == 2) {
                        gray = *(srcPtr + 1);
                    } else if (bytepp == 3 || bytepp == 4) {
                        int b = *srcPtr;
                        int g = *(srcPtr + 1);
                        int r = *(srcPtr + 2);
                        gray = (byte)((3 * r + 6 * g + 1 * b) / 10);
                    }
                    *dstPtr = gray;
                }
            }

            bmp.UnlockBits(bmpData);

            bytepp = 1;
        }

        public unsafe static void Bitmap1BitToGrayImageBuffer(Bitmap bmp, ref IntPtr imgBuf, ref int bw, ref int bh, ref int bytepp) {
            bw = bmp.Width;
            bh = bmp.Height;
            Int64 bufSize = (Int64)bw * bh;
            imgBuf = Util.AllocBuffer(bufSize);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bw, bh), ImageLockMode.ReadOnly, bmp.PixelFormat);
            for (int y = 0; y < bh; y++) {
                byte* dstPtr = (byte*)imgBuf + bw * y;
                byte* srcPtr = (byte*)bmpData.Scan0 + bmpData.Stride * y;
                
                int stepNum = bw / 8;
                int stepRemains = bw % 8;
                int val = 0;
                for (int step = 0; step < stepNum; step++, dstPtr += 8, srcPtr++) {
                    val = *srcPtr;
                    for (int bit = 0; bit < 8; bit++) {
                        dstPtr[bit] = (byte)((val >> (7 - bit) & 1) * 255);
                    }
                }
                val = *srcPtr;
                for (int bit = 0; bit < stepRemains; bit++) {
                    dstPtr[bit] = (byte)((val >> (7 - bit) & 1) * 255);
                }
            }

            bmp.UnlockBits(bmpData);

            bytepp = 1;
        }
    }
}
