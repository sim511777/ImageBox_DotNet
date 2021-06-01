using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShimLib {
    public class ImageDrawing {
        public IntPtr Buf { get; }
        public int Bw { get; }
        public int Bh { get; }
        public double ZoomFactor { get; }
        public Point PtPan { get; }

        public ImageDrawing(IntPtr imageBuf, int imageBw, int imageBh) : this(imageBuf, imageBw, imageBh, 1, Point.Empty) { }

        public ImageDrawing(IntPtr imageBuf, int imageBw, int imageBh, double zoomFactor, Point ptPan) {
            this.Buf = imageBuf;
            this.Bw = imageBw;
            this.Bh = imageBh;
            this.ZoomFactor = zoomFactor;
            this.PtPan = ptPan;
        }

        // 이미지 좌표 -> 화면 좌료
        private Point ImgToDisp(PointF ptImg) {
            int dispX = (int)Math.Floor((ptImg.X + 0.5) * ZoomFactor + PtPan.X);
            int dispY = (int)Math.Floor((ptImg.Y + 0.5) * ZoomFactor + PtPan.Y);
            return new Point(dispX, dispY);
        }

        // 이미지 좌표 -> 화면 좌료
        private Rectangle ImgToDisp(RectangleF rectImg) {
            int dispX = (int)Math.Floor((rectImg.X + 0.5) * ZoomFactor + PtPan.X);
            int dispY = (int)Math.Floor((rectImg.Y + 0.5) * ZoomFactor + PtPan.Y);
            int dispWidth = (int)Math.Floor(rectImg.Width * ZoomFactor);
            int dispHeight = (int)Math.Floor(rectImg.Height * ZoomFactor);
            return new Rectangle(dispX, dispY, dispWidth, dispHeight);
        }

        // ==== GDI 함수 ====
        public void DrawPixel(Color col, PointF pt) {
            Point ptd = ImgToDisp(pt);
            Drawing.DrawPixel(Buf, Bw, Bh, ptd.X, ptd.Y, col.ToArgb());
        }

        public void DrawPixel(Color col, float x, float y) {
            DrawPixel(col, new PointF(x, y));
        }

        public void DrawLine(Color col, PointF pt1, PointF pt2) {
            Point ptd1 = ImgToDisp(pt1);
            Point ptd2 = ImgToDisp(pt2);
            Drawing.DrawLine(Buf, Bw, Bh, ptd1.X, ptd1.Y, ptd2.X, ptd2.Y, col.ToArgb());
        }

        public void DrawLine(Color col, float x1, float y1, float x2, float y2) {
            this.DrawLine(col, new PointF(x1, y1), new PointF(x2, y2));
        }

        public void DrawHLineDot(Color col, float x1, float x2, float y) {
            var ptd1 = ImgToDisp(new PointF(x1, y));
            var ptd2 = ImgToDisp(new PointF(x2, y));
            Drawing.DrawHLineDot(Buf, Bw, Bh, ptd1.X, ptd2.X, ptd1.Y, col.ToArgb());
        }

        public void DrawVLineDot(Color col, float y1, float y2, float x) {
            var ptd1 = ImgToDisp(new PointF(x, y1));
            var ptd2 = ImgToDisp(new PointF(x, y2));
            Drawing.DrawVLineDot(Buf, Bw, Bh, ptd1.Y, ptd2.Y, ptd1.X, col.ToArgb());
        }

        public void DrawEllipse(Color col, RectangleF rect) {
            Rectangle rectd = ImgToDisp(rect);
            int cx = (rectd.Left + rectd.Right) / 2;
            int cy = (rectd.Top + rectd.Bottom) / 2;
            int rx = rectd.Width / 2;
            int ry = rectd.Height / 2;
            Drawing.DrawEllipse(Buf, Bw, Bh, cx, cy, rx, ry, col.ToArgb(), false);
        }

        public void DrawEllipse(Color col, float x, float y, float width, float height) {
            this.DrawEllipse(col, new RectangleF(x, y, width, height));
        }

        public void FillEllipse(Color col, RectangleF rect) {
            Rectangle rectd = ImgToDisp(rect);
            int cx = (rectd.Left + rectd.Right) / 2;
            int cy = (rectd.Top + rectd.Bottom) / 2;
            int rx = rectd.Width / 2;
            int ry = rectd.Height / 2;
            Drawing.DrawEllipse(Buf, Bw, Bh, cx, cy, rx, ry, col.ToArgb(), true);
        }

        public void FillEllipse(Color col, float x, float y, float width, float height) {
            this.FillEllipse(col, new RectangleF(x, y, width, height));
        }

        public void DrawRectangle(Color col, RectangleF rect) {
            Rectangle rectd = ImgToDisp(rect);
            Drawing.DrawRectangle(Buf, Bw, Bh, rectd.Left, rectd.Top, rectd.Right, rectd.Bottom, col.ToArgb(), false);
        }

        public void DrawRectangle(Color col, float x, float y, float width, float height) {
            this.DrawRectangle(col, new RectangleF(x, y, width, height));
        }

        public void DrawRectangleDot(Color col, RectangleF rect) {
            Rectangle rectd = ImgToDisp(rect);
            Drawing.DrawRectangleDot(Buf, Bw, Bh, rectd.Left, rectd.Top, rectd.Right, rectd.Bottom, col.ToArgb());
        }

        public void DrawRectangleDot(Color col, float x, float y, float width, float height) {
            this.DrawRectangleDot(col, new RectangleF(x, y, width, height));
        }

        public void FillRectangle(Color col, RectangleF rect) {
            Rectangle rectd = ImgToDisp(rect);
            Drawing.DrawRectangle(Buf, Bw, Bh, rectd.Left, rectd.Top, rectd.Right, rectd.Bottom, col.ToArgb(), true);
        }

        public void FillRectangle(Color col, float x, float y, float width, float height) {
            this.FillRectangle(col, new RectangleF(x, y, width, height));
        }

        public void DrawCircle(Color col, PointF pt, float size, bool pixelSize) {
            Point ptd = ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ZoomFactor, MidpointRounding.AwayFromZero);
            int half = sized / 2;
            Drawing.DrawCircle(Buf, Bw, Bh, ptd.X, ptd.Y, half, col.ToArgb(), false);
        }

        public void DrawCircle(Color col, float x, float y, float r, bool pixelSize) {
            this.DrawCircle(col, new PointF(x, y), r, pixelSize);
        }

        public void FillCircle(Color col, PointF pt, float size, bool pixelSize) {
            Point ptd = ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ZoomFactor, MidpointRounding.AwayFromZero);
            int half = sized / 2;
            Drawing.DrawCircle(Buf, Bw, Bh, ptd.X, ptd.Y, half, col.ToArgb(), true);
        }

        public void FillCircle(Color col, float x, float y, float r, bool pixelSize) {
            this.FillCircle(col, new PointF(x, y), r, pixelSize);
        }

        public void DrawSquare(Color col, PointF pt, float size, bool pixelSize) {
            Point ptd = ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ZoomFactor, MidpointRounding.AwayFromZero);
            int half = sized / 2;
            Drawing.DrawRectangle(Buf, Bw, Bh, ptd.X - half, ptd.Y - half, ptd.X + half, ptd.Y + half, col.ToArgb(), false);
        }

        public void DrawSquare(Color col, float x, float y, float r, bool pixelSize) {
            this.DrawSquare(col, new PointF(x, y), r, pixelSize);
        }

        public void FillSquare(Color col, PointF pt, float size, bool pixelSize) {
            Point ptd = ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ZoomFactor, MidpointRounding.AwayFromZero);
            int half = sized / 2;
            Drawing.DrawRectangle(Buf, Bw, Bh, ptd.X - half, ptd.Y - half, ptd.X + half, ptd.Y + half, col.ToArgb(), true);
        }

        public void FillSquare(Color col, float x, float y, float r, bool pixelSize) {
            this.FillSquare(col, new PointF(x, y), r, pixelSize);
        }

        public void DrawCross(Color col, PointF pt, float size, bool pixelSize) {
            Point ptd = ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ZoomFactor, MidpointRounding.AwayFromZero);
            int half = sized / 2;
            int iCol = col.ToArgb();
            Drawing.DrawLine(Buf, Bw, Bh, ptd.X - half, ptd.Y - half, ptd.X + half, ptd.Y + half, iCol);
            Drawing.DrawLine(Buf, Bw, Bh, ptd.X - half, ptd.Y + half, ptd.X + half, ptd.Y - half, iCol);
        }

        public void DrawCross(Color col, float x, float y, float r, bool pixelSize) {
            this.DrawCross(col, new PointF(x, y), r, pixelSize);
        }

        public void DrawPlus(Color col, PointF pt, float size, bool pixelSize) {
            Point ptd = ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ZoomFactor, MidpointRounding.AwayFromZero);
            int half = sized / 2;
            int iCol = col.ToArgb();
            Drawing.DrawLine(Buf, Bw, Bh, ptd.X, ptd.Y - half, ptd.X, ptd.Y + half, iCol);
            Drawing.DrawLine(Buf, Bw, Bh, ptd.X - half, ptd.Y, ptd.X + half, ptd.Y, iCol);
        }

        public void DrawPlus(Color col, float x, float y, float r, bool pixelSize) {
            this.DrawPlus(col, new PointF(x, y), r, pixelSize);
        }

        public void DrawString(string text, IFont font, Color col, PointF pt, Color? backColor = null) {
            Point ptd = ImgToDisp(pt);
            if (backColor != null) {
                var size = font.MeasureString(text);
                Drawing.DrawRectangle(Buf, Bw, Bh, ptd.X, ptd.Y, ptd.X + size.Width, ptd.Y + size.Height, backColor.Value.ToArgb(), true);
            }
            font.DrawString(text, Buf, Bw, Bh, ptd.X, ptd.Y, col);
        }

        public void DrawString(string text, IFont font, Color col, float x, float y, Color? backColor = null) {
            DrawString(text, font, col, new PointF(x, y), backColor);
        }

        public Size MeasureString(string text, IFont font) {
            return font.MeasureString(text);
        }
    }
}
