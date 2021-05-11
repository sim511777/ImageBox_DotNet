using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimLib {
    public class ImageGraphics {
        private ImageBox ib;
        private Graphics g;

        public ImageGraphics(ImageBox imageBox, Graphics graphics) {
            this.ib = imageBox;
            this.g = graphics;
        }

        // ==== GDI 함수 ====
        public void DrawLine(Pen pen, PointF pt1, PointF pt2) {
            g.DrawLine(pen, ib.ImgToDisp(pt1), ib.ImgToDisp(pt2));
        }

        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2) {
            this.DrawLine(pen, new PointF(x1, y1), new PointF(x2, y2));
        }

        public void DrawString(string s, Font font, Brush brush, PointF pt, Brush backBrush = null) {
            var ptWnd = ib.ImgToDisp(pt);
            if (backBrush != null) {
                var size = g.MeasureString(s, font);
                g.FillRectangle(backBrush, ptWnd.X, ptWnd.Y, size.Width, size.Height);
            }
            g.DrawString(s, font, brush, ptWnd);
        }

        public void DrawString(string s, Font font, Brush brush, float x, float y, Brush backBrush = null) {
            this.DrawString(s, font, brush, new PointF(x, y), backBrush);
        }

        public void DrawEllipse(Pen pen, RectangleF rect) {
            g.DrawEllipse(pen, ib.ImgToDisp(rect));
        }

        public void DrawEllipse(Pen pen, float x, float y, float width, float height) {
            this.DrawEllipse(pen, new RectangleF(x, y, width, height));
        }

        public void DrawRectangle(Pen pen, RectangleF rect) {
            g.DrawRectangle(pen, ib.ImgToDisp(rect));
        }

        public void DrawRectangle(Pen pen, float x, float y, float width, float height) {
            this.DrawRectangle(pen, new RectangleF(x, y, width, height));
        }

        public void DrawCircle(Pen pen, PointF pt, float size, bool pixelSize) {
            Point ptd = ib.ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ib.GetZoomFactor(), MidpointRounding.AwayFromZero);
            int half = sized / 2;
            g.DrawEllipse(pen, ptd.X - half, ptd.Y - half, sized, sized);
        }

        public void DrawCircle(Pen pen, float x, float y, float r, bool pixelSize) {
            this.DrawCircle(pen, new PointF(x, y), r, pixelSize);
        }

        public void DrawSquare(Pen pen, PointF pt, float size, bool pixelSize) {
            Point ptd = ib.ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ib.GetZoomFactor(), MidpointRounding.AwayFromZero);
            int half = sized / 2;
            g.DrawRectangle(pen, ptd.X - half, ptd.Y - half, sized, sized);
        }

        public void DrawSquare(Pen pen, float x, float y, float r, bool pixelSize) {
            this.DrawSquare(pen, new PointF(x, y), r, pixelSize);
        }

        public void DrawCross(Pen pen, PointF pt, float size, bool pixelSize) {
            Point ptd = ib.ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ib.GetZoomFactor(), MidpointRounding.AwayFromZero);
            int half = sized / 2;
            g.DrawLine(pen, ptd.X - half, ptd.Y - half, ptd.X + half, ptd.Y + half);
            g.DrawLine(pen, ptd.X - half, ptd.Y + half, ptd.X + half, ptd.Y - half);
        }

        public void DrawCross(Pen pen, float x, float y, float r, bool pixelSize) {
            this.DrawCross(pen, new PointF(x, y), r, pixelSize);
        }

        public void DrawPlus(Pen pen, PointF pt, float size, bool pixelSize) {
            Point ptd = ib.ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ib.GetZoomFactor(), MidpointRounding.AwayFromZero);
            int half = sized / 2;
            g.DrawLine(pen, ptd.X, ptd.Y - half, ptd.X, ptd.Y + half);
            g.DrawLine(pen, ptd.X - half, ptd.Y, ptd.X + half, ptd.Y);
        }

        public void DrawPlus(Pen pen, float x, float y, float r, bool pixelSize) {
            this.DrawPlus(pen, new PointF(x, y), r, pixelSize);
        }
    }
}
