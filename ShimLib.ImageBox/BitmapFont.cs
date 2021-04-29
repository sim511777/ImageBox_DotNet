using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ShimLib.Properties;

namespace ShimLib {
    public class BitmapFonts {
        public static BitmapFont Ascii_04x06 = new BitmapFont(Resource.Raster_04x06);
        public static BitmapFont Ascii_05x08 = new BitmapFont(Resource.Raster_05x08);
        public static BitmapFont Ascii_05x12 = new BitmapFont(Resource.Raster_05x12);
        public static BitmapFont Ascii_06x08 = new BitmapFont(Resource.Raster_06x08);
        public static BitmapFont Ascii_06x13 = new BitmapFont(Resource.Raster_06x13);
        public static BitmapFont Ascii_07x12 = new BitmapFont(Resource.Raster_07x12);
        public static BitmapFont Ascii_08x08 = new BitmapFont(Resource.Raster_08x08);
        public static BitmapFont Ascii_08x12 = new BitmapFont(Resource.Raster_08x12);
        public static BitmapFont Ascii_08x16 = new BitmapFont(Resource.Raster_08x16);
        public static BitmapFont Ascii_08x18 = new BitmapFont(Resource.Raster_08x18);
        public static BitmapFont Ascii_10x18 = new BitmapFont(Resource.Raster_10x18);
        public static BitmapFont Ascii_10x20 = new BitmapFont(Resource.Raster_10x20);
        public static BitmapFont Ascii_10x22 = new BitmapFont(Resource.Raster_10x22);
        public static BitmapFont Ascii_12x16 = new BitmapFont(Resource.Raster_12x16);
        public static BitmapFont Ascii_12x27 = new BitmapFont(Resource.Raster_12x27);
        public static BitmapFont Ascii_16x08 = new BitmapFont(Resource.Raster_16x08);
        public static BitmapFont Ascii_16x12 = new BitmapFont(Resource.Raster_16x12);
    }

    public class BitmapFont {
        private IntPtr fontBuf;
        private int fontBw;
        private int fontBh;
        private int fw;
        private int fh;
        private int bytepp;

        private int fontBufStartX;
        private int fontBufStartY;
        private int fontStartIndex;
        public BitmapFont(Bitmap bmp, int fontBufStartX, int fontBufStartY, int fontBw, int fontBh, int fontStartIndex) {

        }

        public BitmapFont(Bitmap bmp) {
            ImageBoxUtil.BitmapToGrayImageBuffer(bmp, ref fontBuf, ref fontBw, ref fontBh, ref bytepp);
            fw = fontBw / 32;
            fh = fontBh / 3;
        }
        
        ~BitmapFont() {
            ImageBoxUtil.FreeBuffer(ref fontBuf);
        }

        public void DrawString(string text, IntPtr dispBuf, int dispBW, int dispBH, int dx, int dy, Color color) {
            int icolor = color.ToArgb();
            int x = dx;
            int y = dy;
            foreach (char ch in text) {
                if (ch == '\r') {
                    continue;
                }
                if (ch == '\n') {
                    x = dx;
                    y += fh;
                    continue;
                }
                if (ch >= 32 && ch <= 128) {
                    int fontX = (ch - 32) * fw;
                    int fontImgY = fontX / fontBw * fh;
                    int fontImgX = fontX % fontBw;
                    DrawChar(fontImgX, fontImgY, dispBuf, dispBW, dispBH, x, y, icolor);
                }
                x += fw;
            }
        }

        public Size MeasureString(string text) {
            int maxX = 0;
            int maxY = 0;
            int x = 0;
            int y = 0;
            foreach (char ch in text) {
                if (ch == '\r') {
                    continue;
                }
                if (ch == '\n') {
                    x = 0;
                    y += fh;
                    continue;
                }
                maxX = Math.Max(maxX, x + fw);
                maxY = Math.Max(maxY, y + fh);
                x += fw;
            }

            return new Size(maxX, maxY);
        }

        private unsafe void DrawChar(int fontImgX, int fontImgY, IntPtr dispBuf, int dispBW, int dispBH, int dx, int dy, int icolor) {
            int x1 = dx;
            int y1 = dy;
            int x2 = dx + fw - 1;
            int y2 = dy + fh - 1;
            if (x1 >= dispBW || x2 < 0 || y1 >= dispBH || y2 < 0)
                return;

            for (int y = 0; y < fh; y++) {
                if (dy + y < 0 || dy + y >= dispBH)
                    continue;
                int* dst = (int*)dispBuf + dispBW * (dy + y) + dx;
                byte* src = (byte*)fontBuf + (fontBw * (fontImgY + y) + fontImgX) * bytepp;
                for (int x = 0; x < fw; x++, src += bytepp, dst++) {
                    if (dx + x < 0 || dx + x >= dispBW)
                        continue;
                    if (*src == 0) {
                        *dst = icolor;
                    }
                }
            }
        }
    }
}
