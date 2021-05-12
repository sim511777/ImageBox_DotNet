using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimLib {
    public class HexChar {
        public int fw;
        public int fh;
        public byte[] charBuf;
    }

    public class HexFont : IFont {
        private HexChar[] fontChars = new HexChar[65536];
        private int fw;
        private int fh;

        public HexFont(string hex) {
            this.fw = 16;
            this.fh = 16;

            byte[] pal = { 0, 1, };
            
            string[] lines = hex.Split(new char[]{'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines) {
                var words = line.Split(':');
                int charIdx = int.Parse(words[0], System.Globalization.NumberStyles.HexNumber);
                var fontChar = new HexChar();
                fontChars[charIdx] = fontChar;
                fontChar.fw = words[1].Length > 32 ? 16 : 8;
                fontChar.fh = 16;
                fontChar.charBuf = new byte[fontChar.fw * fontChar.fh];
                uint[] uints = HexToUint(words[1]);
                int ii = 0;
                for (int i = 0; i < uints.Length; i++) {
                    uint val = uints[i];
                    for (int j = 0; j < 32; j++) {
                        fontChar.charBuf[ii] = pal[(val >> (31-j)) & 1];
                        ii++;
                    }
                }
            }
        }

        private static uint[] HexToUint(string hex) {
            var arr = new uint[hex.Length / 8];
            for (int i = 0; i < arr.Length; i++) {
                arr[i] = uint.Parse(hex.Substring(i*8, 8), System.Globalization.NumberStyles.HexNumber);
            }
            return arr;
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
                
                int _fw = fw;
                if (fontChars[ch] != null) {
                    _fw = fontChars[ch].fw;
                    DrawChar(fontChars[ch], dispBuf, dispBW, dispBH, x, y, icolor);
                }
                x += _fw;
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
                int _fw = fw;
                if (fontChars[ch] != null)
                        _fw = fontChars[ch].fw;
                maxX = Math.Max(maxX, x + _fw);
                maxY = Math.Max(maxY, y + fh);
                x += _fw;
            }

            return new Size(maxX, maxY);
        }

        private unsafe void DrawChar(HexChar fontChar, IntPtr dispBuf, int dispBW, int dispBH, int dx, int dy, int icolor) {
            int _fw = fontChar.fw;
            int x1 = dx;
            int y1 = dy;
            int x2 = dx + _fw - 1;
            int y2 = dy + fh - 1;
            if (x1 >= dispBW || x2 < 0 || y1 >= dispBH || y2 < 0)
                return;
            byte[] src = fontChar.charBuf;
            for (int y = 0; y < fh; y++) {
                if (dy + y < 0 || dy + y >= dispBH)
                    continue;
                int* dst = (int*)dispBuf + dispBW * (dy + y) + dx;
                int srcIdx = y * _fw;
                for (int x = 0; x < _fw; x++, srcIdx++, dst++) {
                    if (dx + x < 0 || dx + x >= dispBW)
                        continue;
                    if (src[srcIdx] != 0) {
                        *dst = icolor;
                    }
                }
            }
        }
    }
}
