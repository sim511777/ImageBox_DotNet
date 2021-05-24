using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimLib {
    public class BmpFont : IFont {
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

        public BmpFont(Bitmap bmp, int fw, int fh, int bufSx, int bufSy, ushort charSI) {
            int bytepp = 0;
            ImageUtil.BitmapToGrayImageBuffer(bmp, ref fontBuf, ref fontBw, ref fontBh, ref bytepp);
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
        }

        ~BmpFont() {
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
                
                if (ch >= charSI && ch <= charEI) {
                    int fontIdx = ch - charSI;
                    int fontImgY = fontIdx / nx * fh + bufSy;
                    int fontImgX = fontIdx % nx * fw + bufSx;
                    DrawChar(fontImgX, fontImgY, dispBuf, dispBW, dispBH, x, y, icolor);
                }
                x += fw;
            }
        }

        public int FontHeight => fh;

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
                byte* src = (byte*)fontBuf + fontBw * (fontImgY + y) + fontImgX;
                for (int x = 0; x < fw; x++, src++, dst++) {
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
