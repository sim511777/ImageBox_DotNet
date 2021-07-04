using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimLib {
    public class PcfFont : IFont {
        int fw = 16;
        int fh = 16;
        public PcfFont(byte[] pcf) {
        }

        public int FontHeight => fh;

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
                
                Drawing.DrawRectangle(dispBuf, dispBW, dispBH, x + 4, y + 4, x + 8, y + 8, icolor, false);
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
    }
}
