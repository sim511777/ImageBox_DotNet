using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimLib {
    interface IFont {
        Size MeasureString(string text);
        void DrawString(string text, IntPtr dispBuf, int dispBW, int dispBH, int dx, int dy, Color color);
    }
}
