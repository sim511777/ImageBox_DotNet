using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimLib {
    public class HexFont : IFont {
        public void DrawString(string text, IntPtr dispBuf, int dispBW, int dispBH, int dx, int dy, Color color) {
            throw new NotImplementedException();
        }

        public Size MeasureString(string text) {
            throw new NotImplementedException();
        }
    }
}
