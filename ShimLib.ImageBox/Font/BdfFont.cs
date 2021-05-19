using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace ShimLib {
    public class BdfChar {
        public int encoding;            // 유니코드
        public string name;             // 문자 이름
        public int width;               // 문자 w : 다음 글자 위치
        public int bbW;                 // 비트맵 w
        public int bbH;                 // 비트맵 h
        public int bbLeft;              // 비트맵 왼쪽 시작 좌표
        public int bbBottom;            // 비트맵 아래쪽 시작 좌표
        public byte[] bitmap;           // 비트맵 버퍼
    }

    public class BdfFont : IFont{
        public int fbW;                 // 폰트영역 w : 문자가 없을때 다음 글자 위치
        public int fbH;                 // 폰트영역 h : 다음 라인 위치
        public int fbLeft;              // 폰트영역 왼쪽 시작 좌표
        public int fbBottom;            // 폰트영역 아래 시작 좌표
        public Dictionary<int, BdfChar> fontChars = new Dictionary<int, BdfChar>();         // 문자 배열

        BdfChar currChar = null;
        public BdfFont(string bdf) {
            string[] lines = bdf.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            int bitmapIdx = -1;
            foreach (var line in lines) {
                string name = string.Empty;
                string data = string.Empty;
                var spaceIdx = line.IndexOf(' ');
                if (spaceIdx > 0) {
                    name = line.Substring(0, spaceIdx);
                    data = line.Substring(spaceIdx + 1);
                } else {
                    name = line;
                }
                    
                if (name == "ENDCHAR") {
                    currChar = null;
                    bitmapIdx = -1;
                } if (bitmapIdx != -1) {
                    ParseBitmap(currChar.bitmap, bitmapIdx, line, currChar.bbW);
                    bitmapIdx += currChar.bbW;
                } else {
                    if (name == "FONTBOUNDINGBOX") {
                        var words = data.Split(' ');
                        this.fbW = int.Parse(words[0]);
                        this.fbH = int.Parse(words[1]);
                        this.fbLeft = int.Parse(words[2]);
                        this.fbBottom = int.Parse(words[3]);
                    } else if (name == "STARTCHAR") {
                        currChar = new BdfChar();
                        currChar.name = data;
                    } else if (name == "ENCODING") {
                        int charIdx = int.Parse(data);
                        fontChars[charIdx] = currChar;
                        currChar.encoding = charIdx;
                    } else if (name == "DWIDTH") {
                        var words = data.Split(' ');
                        currChar.width = int.Parse(words[0]);
                    } else if (name == "BBX") {
                        var words = data.Split(' ');
                        currChar.bbW = int.Parse(words[0]);
                        currChar.bbH = int.Parse(words[1]);
                        currChar.bbLeft = int.Parse(words[2]);
                        currChar.bbBottom = int.Parse(words[3]);
                        currChar.bitmap = new byte[currChar.bbW * currChar.bbH];
                    } else if (name == "BITMAP") {
                        bitmapIdx = 0;
                    }
                }
            }
        }

        private void ParseBitmap(byte[] bitmap, int bitmapIdx, string line, int bitmapWidth) {
            int bitmapX = 0;
            byte[] pal = { 0, 1};
            for (int i = 0; i < line.Length; i += 2) {
                if (bitmapX > bitmapWidth)
                    break;
                byte b = byte.Parse(line.Substring(i, 2), NumberStyles.HexNumber);
                for (int j = 0; j < 8; j++) {
                    if (bitmapX >= bitmapWidth)
                        break;
                    bitmap[bitmapIdx + bitmapX] = pal[(b >> (7-j)) & 1];
                    bitmapX++;
                }
            }
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
                    y += fbH;
                    continue;
                }
                
                int fw = fontChars.ContainsKey(ch) ? fontChars[ch].width : fbW;
                if (fontChars.ContainsKey(ch)) {
                    DrawChar(fontChars[ch], dispBuf, dispBW, dispBH, x, y, icolor);
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
                    y += fbH;
                    continue;
                }
                int fw = fontChars.ContainsKey(ch) ? fontChars[ch].width : fbW;
                maxX = Math.Max(maxX, x + fw);
                maxY = Math.Max(maxY, y + fbH);
                x += fw;
            }

            return new Size(maxX, maxY);
        }

        private unsafe void DrawChar(BdfChar fontChar, IntPtr dispBuf, int dispBW, int dispBH, int dx, int dy, int icolor) {
            dx = dx - this.fbLeft + fontChar.bbLeft;
            dy = dy + this.fbH - fontChar.bbH + this.fbBottom - fontChar.bbBottom;
            int x1 = dx;
            int y1 = dy;
            int x2 = dx + fbW - 1;
            int y2 = dy + fbH - 1;
            if (x1 >= dispBW || x2 < 0 || y1 >= dispBH || y2 < 0)
                return;

            int fw = fontChar.bbW;
            int fh = fontChar.bbH;
            byte[] src = fontChar.bitmap;
            for (int y = 0; y < fh; y++) {
                if (dy + y < 0 || dy + y >= dispBH)
                    continue;
                int* dst = (int*)dispBuf + dispBW * (dy + y) + dx;
                int srcIdx = y * fw;
                for (int x = 0; x < fw; x++, srcIdx++, dst++) {
                    if (dx + x < 0 || dx + x >= dispBW)
                        continue;
                    if (src[srcIdx] != 0) {
                        *dst = icolor;
                    }
                }
            }
        }

        public int GetFontHeight() {
            return fbH;
        }
    }
}
