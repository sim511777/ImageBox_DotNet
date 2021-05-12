using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimLib {
    public class BdfChar {
        public int Encoding;            // 유니코드 문자코드
        public string Name;             // 문자 이름
        public int Width;               // 너비
        public int Height;              // 높이
        public int LeftOffset;          // 왼쪽 옵셋?
        public int BottomOffset;        // 아래쪽 옵셋?
        public byte[] Bitmap;           // 비트맵
    }

    public class BdfFont : IFont{
        public string PostScriptName;   // 이름?
        public int Size;                // 사이즈?
        public int MaximumWidth;        // 최대 너비
        public int Height;              // 높이
        public BdfChar[] Chars;         // 문자 배열

        public void DrawString(string text, IntPtr dispBuf, int dispBW, int dispBH, int dx, int dy, Color color) {
            throw new NotImplementedException();
        }

        public Size MeasureString(string text) {
            throw new NotImplementedException();
        }
    }
}
