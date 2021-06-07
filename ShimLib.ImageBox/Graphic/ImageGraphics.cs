using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimLib {
    public class ImageGraphics {
        private Graphics g;
        public double ZoomFactor { get; }
        public Point PtPan { get; }

        public ImageGraphics(Graphics graphics) : this(graphics, 1, Point.Empty) { }

        public ImageGraphics(Graphics graphics, double zoomFactor, Point ptPan) {
            g = graphics;
            ZoomFactor = zoomFactor;
            PtPan = ptPan;
        }

        // 이미지 좌표 -> 화면 좌료
        private Point ImgToDisp(PointF ptImg) {
            return ImageBoxUtil.ImgToDisp(ptImg, ZoomFactor, PtPan);
        }

        // 이미지 좌표 -> 화면 좌료
        private Rectangle ImgToDisp(RectangleF rectImg) {
            return ImageBoxUtil.ImgToDisp(rectImg, ZoomFactor, PtPan);
        }

        // ==== GDI 함수 ====
        public void DrawLine(Pen pen, PointF pt1, PointF pt2) {
            Point ptd1 = ImgToDisp(pt1);
            Point ptd2 = ImgToDisp(pt2);
            g.DrawLine(pen, ptd1.X, ptd1.Y, ptd2.X, ptd2.Y);
        }

        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2) {
            this.DrawLine(pen, new PointF(x1, y1), new PointF(x2, y2));
        }

        public void DrawEllipse(Pen pen, RectangleF rect) {
            g.DrawEllipse(pen, ImgToDisp(rect));
        }

        public void DrawEllipse(Pen pen, float x, float y, float width, float height) {
            this.DrawEllipse(pen, new RectangleF(x, y, width, height));
        }

        public void DrawRectangle(Pen pen, RectangleF rect) {
            g.DrawRectangle(pen, ImgToDisp(rect));
        }

        public void DrawRectangle(Pen pen, float x, float y, float width, float height) {
            this.DrawRectangle(pen, new RectangleF(x, y, width, height));
        }

        public void FillRectangle(Brush brush, RectangleF rect) {
            g.FillRectangle(brush, ImgToDisp(rect));
        }

        public void FillRectangle(Brush brush, float x, float y, float width, float height) {
            this.FillRectangle(brush, new RectangleF(x, y, width, height));
        }

        public void DrawCircle(Pen pen, PointF pt, float size, bool pixelSize) {
            Point ptd = ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ZoomFactor, MidpointRounding.AwayFromZero);
            int half = sized / 2;
            g.DrawEllipse(pen, ptd.X - half, ptd.Y - half, sized, sized);
        }

        public void DrawCircle(Pen pen, float x, float y, float r, bool pixelSize) {
            this.DrawCircle(pen, new PointF(x, y), r, pixelSize);
        }

        public void FillCircle(Brush brush, PointF pt, float size, bool pixelSize) {
            Point ptd = ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ZoomFactor, MidpointRounding.AwayFromZero);
            int half = sized / 2;
            g.FillEllipse(brush, ptd.X - half, ptd.Y - half, sized, sized);
        }

        public void FillCircle(Brush brush, float x, float y, float r, bool pixelSize) {
            this.FillCircle(brush, new PointF(x, y), r, pixelSize);
        }

        public void DrawSquare(Pen pen, PointF pt, float size, bool pixelSize) {
            Point ptd = ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ZoomFactor, MidpointRounding.AwayFromZero);
            int half = sized / 2;
            g.DrawRectangle(pen, ptd.X - half, ptd.Y - half, sized, sized);
        }

        public void DrawSquare(Pen pen, float x, float y, float r, bool pixelSize) {
            this.DrawSquare(pen, new PointF(x, y), r, pixelSize);
        }

        public void FillSquare(Brush brush, PointF pt, float size, bool pixelSize) {
            Point ptd = ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ZoomFactor, MidpointRounding.AwayFromZero);
            int half = sized / 2;
            g.FillRectangle(brush, ptd.X - half, ptd.Y - half, sized, sized);
        }

        public void FillSquare(Brush brush, float x, float y, float r, bool pixelSize) {
            this.FillSquare(brush, new PointF(x, y), r, pixelSize);
        }

        public void DrawCross(Pen pen, PointF pt, float size, bool pixelSize) {
            Point ptd = ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ZoomFactor, MidpointRounding.AwayFromZero);
            int half = sized / 2;
            g.DrawLine(pen, ptd.X - half, ptd.Y - half, ptd.X + half, ptd.Y + half);
            g.DrawLine(pen, ptd.X - half, ptd.Y + half, ptd.X + half, ptd.Y - half);
        }

        public void DrawCross(Pen pen, float x, float y, float r, bool pixelSize) {
            this.DrawCross(pen, new PointF(x, y), r, pixelSize);
        }

        public void DrawPlus(Pen pen, PointF pt, float size, bool pixelSize) {
            Point ptd = ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ZoomFactor, MidpointRounding.AwayFromZero);
            int half = sized / 2;
            g.DrawLine(pen, ptd.X, ptd.Y - half, ptd.X, ptd.Y + half);
            g.DrawLine(pen, ptd.X - half, ptd.Y, ptd.X + half, ptd.Y);
        }

        public void DrawPlus(Pen pen, float x, float y, float r, bool pixelSize) {
            this.DrawPlus(pen, new PointF(x, y), r, pixelSize);
        }

        public void DrawString(string s, Font font, Brush brush, PointF pt, Brush backBrush = null) {
            var ptWnd = ImgToDisp(pt);
            if (backBrush != null) {
                var size = g.MeasureString(s, font);
                g.FillRectangle(backBrush, ptWnd.X, ptWnd.Y, size.Width, size.Height);
            }
            g.DrawString(s, font, brush, ptWnd);
        }

        public void DrawString(string s, Font font, Brush brush, float x, float y, Brush backBrush = null) {
            this.DrawString(s, font, brush, new PointF(x, y), backBrush);
        }

        public Size MeasureString(string text, Font font) {
            return g.MeasureString(text, font).ToSize();
        }
    }
}
