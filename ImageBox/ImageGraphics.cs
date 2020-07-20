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

        public void DrawEllipse(Pen pen, RectangleF rect) {
            g.DrawEllipse(pen, imgBox.ImgToDisp(rect));
        }

        public void DrawEllipse(Pen pen, float x, float y, float width, float height) {
            this.DrawEllipse(pen, new RectangleF(x, y, width, height));
        }

        public void DrawRectangle(Pen pen, RectangleF rect) {
            g.DrawRectangle(pen, imgBox.ImgToDisp(rect));
        }

        public void DrawRectangle(Pen pen, float x, float y, float width, float height) {
            this.DrawRectangle(pen, new RectangleF(x, y, width, height));
        }

        public void DrawCircle(Pen pen, float x, float y, float r) {
            float left = x - r;
            float top = y - r;
            float size = r + r;
            this.DrawEllipse(pen, left, top, size, size);
        }

        public void DrawCircle(Pen pen, PointF pt, float r) {
            this.DrawCircle(pen, pt.X, pt.Y, r);
        }

        public void DrawSquare(Pen pen, float x, float y, float r) {
            float left = x - r;
            float top = y - r;
            float size = r + r;
            this.DrawRectangle(pen, left, top, size, size);
        }

        public void DrawSquare(Pen pen, PointF pt, float r) {
            this.DrawSquare(pen, pt.X, pt.Y, r);
        }
    }
}
