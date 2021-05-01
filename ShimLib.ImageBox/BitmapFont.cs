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
        public static BitmapFont Ascii_04x06 = new BitmapFont(Resource.Raster_04x06, 04, 06, 0, 0, 32);
        public static BitmapFont Ascii_05x08 = new BitmapFont(Resource.Raster_05x08, 05, 08, 0, 0, 32);
        public static BitmapFont Ascii_05x12 = new BitmapFont(Resource.Raster_05x12, 05, 12, 0, 0, 32);
        public static BitmapFont Ascii_06x08 = new BitmapFont(Resource.Raster_06x08, 06, 08, 0, 0, 32);
        public static BitmapFont Ascii_06x13 = new BitmapFont(Resource.Raster_06x13, 06, 13, 0, 0, 32);
        public static BitmapFont Ascii_07x12 = new BitmapFont(Resource.Raster_07x12, 07, 12, 0, 0, 32);
        public static BitmapFont Ascii_08x08 = new BitmapFont(Resource.Raster_08x08, 08, 08, 0, 0, 32);
        public static BitmapFont Ascii_08x12 = new BitmapFont(Resource.Raster_08x12, 08, 12, 0, 0, 32);
        public static BitmapFont Ascii_08x16 = new BitmapFont(Resource.Raster_08x16, 08, 16, 0, 0, 32);
        public static BitmapFont Ascii_08x18 = new BitmapFont(Resource.Raster_08x18, 08, 18, 0, 0, 32);
        public static BitmapFont Ascii_10x18 = new BitmapFont(Resource.Raster_10x18, 10, 18, 0, 0, 32);
        public static BitmapFont Ascii_10x20 = new BitmapFont(Resource.Raster_10x20, 10, 20, 0, 0, 32);
        public static BitmapFont Ascii_10x22 = new BitmapFont(Resource.Raster_10x22, 10, 22, 0, 0, 32);
        public static BitmapFont Ascii_12x16 = new BitmapFont(Resource.Raster_12x16, 12, 16, 0, 0, 32);
        public static BitmapFont Ascii_12x27 = new BitmapFont(Resource.Raster_12x27, 12, 27, 0, 0, 32);
        public static BitmapFont Ascii_16x08 = new BitmapFont(Resource.Raster_16x08, 16, 08, 0, 0, 32);
        public static BitmapFont Ascii_16x12 = new BitmapFont(Resource.Raster_16x12, 16, 12, 0, 0, 32);
        public static BitmapFont Unicode_16x16 = new BitmapFont(Resource.Unifont_16x16, 16, 16, 32, 64, 0, true);
    }

    public class BitmapFont {
        private IntPtr fontBuf;
        private int fontBw;
        private int fontBh;

        private int fw;
        private int fh;
        private int bufSx;
        private int bufSy;
        private int nx;
        private int ny;
        private ushort charSI;
        private ushort charEI;
        private bool asciiHalfWidth;

        public BitmapFont(Bitmap bmp, int fw, int fh, int bufSx, int bufSy, ushort charSI, bool asciiHalfWidth = false) {
            int bytepp = 0;
            Util.BitmapToGrayImageBuffer(bmp, ref fontBuf, ref fontBw, ref fontBh, ref bytepp);
            this.fw = fw;
            this.fh = fh;
            this.bufSx = bufSx;
            this.bufSy = bufSy;
            this.charSI = charSI;

            int useBw = fontBw - bufSx;
            int useBh = fontBh - bufSy;

            this.nx = useBw / fw;
            this.ny = useBh / fh;

            charEI = (ushort)(charSI + nx * ny - 1);

            this.asciiHalfWidth = asciiHalfWidth;
        }
        
        ~BitmapFont() {
            Util.FreeBuffer(ref fontBuf);
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
                
                bool halfSize = (ch >= 0 && ch <= 127 && asciiHalfWidth);
                if (ch >= charSI && ch <= charEI) {
                    int fontIdx = ch - charSI;
                    int fontImgY = fontIdx / nx * fh + bufSy;
                    int fontImgX = fontIdx % nx * fw + bufSx;
                    DrawChar(fontImgX, fontImgY, dispBuf, dispBW, dispBH, x, y, icolor, halfSize);
                }
                x += halfSize ? fw / 2 : fw;
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
                bool halfSize = (ch >= 0 && ch <= 127 && asciiHalfWidth);
                int _fw = halfSize ? fw / 2 : fw;
                maxX = Math.Max(maxX, x + _fw);
                maxY = Math.Max(maxY, y + fh);
                x += _fw;
            }

            return new Size(maxX, maxY);
        }

        private unsafe void DrawChar(int fontImgX, int fontImgY, IntPtr dispBuf, int dispBW, int dispBH, int dx, int dy, int icolor, bool halfSize) {
            int _fw = halfSize ? fw / 2 : fw;
            int x1 = dx;
            int y1 = dy;
            int x2 = dx + _fw - 1;
            int y2 = dy + fh - 1;
            if (x1 >= dispBW || x2 < 0 || y1 >= dispBH || y2 < 0)
                return;

            for (int y = 0; y < fh; y++) {
                if (dy + y < 0 || dy + y >= dispBH)
                    continue;
                int* dst = (int*)dispBuf + dispBW * (dy + y) + dx;
                byte* src = (byte*)fontBuf + fontBw * (fontImgY + y) + fontImgX;
                for (int x = 0; x < _fw; x++, src++, dst++) {
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
