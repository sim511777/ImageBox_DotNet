using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageBox {
    public class ImageGraphics {
        private ImageBox imgBox;
        private Graphics g;
        
        public ImageGraphics(ImageBox _imgBox, Graphics _g) {
            imgBox = _imgBox;
            g = _g;
        }

        public void DrawLine(Pen pen, PointF pt1, PointF pt2) {
            g.DrawLine(pen, imgBox.ImgToDisp(pt1), imgBox.ImgToDisp(pt2));
        }

        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2) {
            this.DrawLine(pen, new PointF(x1, y1), new PointF(x2, y2));
        }

        public void DrawString(string s, Font font, Brush brush, PointF pt) {
            g.DrawString(s, font, brush, imgBox.ImgToDisp(pt));
        }

        public void DrawString(string s, Font font, Brush brush, float x, float y) {
            this.DrawString(s, font, brush, new PointF(x, y));
        }
    }
}
