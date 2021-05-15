using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShimLib {
    public class ImageDrawing {
        private ImageBox ib;
        private IntPtr buf;
        private int bw;
        private int bh;

        public ImageDrawing(ImageBox imageBox, IntPtr imageBuf, int imageBw, int imageBh) {
            this.ib = imageBox;
            this.buf = imageBuf;
            this.bw = imageBw;
            this.bh = imageBh;
        }

        // ==== GDI 함수 ====
        public void DrawPixel(Color col, PointF pt) {
            Point ptd = ib.ImgToDisp(pt);
            Drawing.DrawPixel(buf, bw, bh, ptd.X, ptd.Y, col.ToArgb());
        }

        public void DrawPixel(Color col, float x, float y) {
            DrawPixel(col, new PointF(x, y));
        }

        public void DrawLine(Color col, PointF pt1, PointF pt2) {
            Point ptd1 = ib.ImgToDisp(pt1);
            Point ptd2 = ib.ImgToDisp(pt2);
            Drawing.DrawLine(buf, bw, bh, ptd1.X, ptd1.Y, ptd2.X, ptd2.Y, col.ToArgb());
        }

        public void DrawLine(Color col, float x1, float y1, float x2, float y2) {
            this.DrawLine(col, new PointF(x1, y1), new PointF(x2, y2));
        }

        public void DrawHLineDot(Color col, float x1, float x2, float y) {
            var ptd1 = ib.ImgToDisp(new PointF(x1, y));
            var ptd2 = ib.ImgToDisp(new PointF(x2, y));
            Drawing.DrawHLineDot(buf, bw, bh, ptd1.X, ptd2.X, ptd1.Y, col.ToArgb());
        }

        public void DrawVLineDot(Color col, float y1, float y2, float x) {
            var ptd1 = ib.ImgToDisp(new PointF(x, y1));
            var ptd2 = ib.ImgToDisp(new PointF(x, y2));
            Drawing.DrawVLineDot(buf, bw, bh, ptd1.Y, ptd2.Y, ptd1.X, col.ToArgb());
        }

        public void DrawEllipse(Color col, RectangleF rect) {
            Rectangle rectd = ib.ImgToDisp(rect);
            int cx = (rectd.Left + rectd.Right) / 2;
            int cy = (rectd.Top + rectd.Bottom) / 2;
            int rx = rectd.Width / 2;
            int ry = rectd.Height / 2;
            Drawing.DrawEllipse(buf, bw, bh, cx, cy, rx, ry, col.ToArgb(), false);
        }

        public void DrawEllipse(Color col, float x, float y, float width, float height) {
            this.DrawEllipse(col, new RectangleF(x, y, width, height));
        }

        public void FillEllipse(Color col, RectangleF rect) {
            Rectangle rectd = ib.ImgToDisp(rect);
            int cx = (rectd.Left + rectd.Right) / 2;
            int cy = (rectd.Top + rectd.Bottom) / 2;
            int rx = rectd.Width / 2;
            int ry = rectd.Height / 2;
            Drawing.DrawEllipse(buf, bw, bh, cx, cy, rx, ry, col.ToArgb(), true);
        }

        public void FillEllipse(Color col, float x, float y, float width, float height) {
            this.FillEllipse(col, new RectangleF(x, y, width, height));
        }

        public void DrawRectangle(Color col, RectangleF rect) {
            Rectangle rectd = ib.ImgToDisp(rect);
            Drawing.DrawRectangle(buf, bw, bh, rectd.Left, rectd.Top, rectd.Right, rectd.Bottom, col.ToArgb(), false);
        }

        public void DrawRectangle(Color col, float x, float y, float width, float height) {
            this.DrawRectangle(col, new RectangleF(x, y, width, height));
        }

        public void DrawRectangleDot(Color col, RectangleF rect) {
            Rectangle rectd = ib.ImgToDisp(rect);
            Drawing.DrawRectangleDot(buf, bw, bh, rectd.Left, rectd.Top, rectd.Right, rectd.Bottom, col.ToArgb());
        }

        public void DrawRectangleDot(Color col, float x, float y, float width, float height) {
            this.DrawRectangleDot(col, new RectangleF(x, y, width, height));
        }

        public void FillRectangle(Color col, RectangleF rect) {
            Rectangle rectd = ib.ImgToDisp(rect);
            Drawing.DrawRectangle(buf, bw, bh, rectd.Left, rectd.Top, rectd.Right, rectd.Bottom, col.ToArgb(), true);
        }

        public void FillRectangle(Color col, float x, float y, float width, float height) {
            this.FillRectangle(col, new RectangleF(x, y, width, height));
        }

        public void DrawCircle(Color col, PointF pt, float size, bool pixelSize) {
            Point ptd = ib.ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ib.GetZoomFactor(), MidpointRounding.AwayFromZero);
            int half = sized / 2;
            Drawing.DrawCircle(buf, bw, bh, ptd.X, ptd.Y, half, col.ToArgb(), false);
        }

        public void DrawCircle(Color col, float x, float y, float r, bool pixelSize) {
            this.DrawCircle(col, new PointF(x, y), r, pixelSize);
        }

        public void FillCircle(Color col, PointF pt, float size, bool pixelSize) {
            Point ptd = ib.ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ib.GetZoomFactor(), MidpointRounding.AwayFromZero);
            int half = sized / 2;
            Drawing.DrawCircle(buf, bw, bh, ptd.X, ptd.Y, half, col.ToArgb(), true);
        }

        public void FillCircle(Color col, float x, float y, float r, bool pixelSize) {
            this.FillCircle(col, new PointF(x, y), r, pixelSize);
        }

        public void DrawSquare(Color col, PointF pt, float size, bool pixelSize) {
            Point ptd = ib.ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ib.GetZoomFactor(), MidpointRounding.AwayFromZero);
            int half = sized / 2;
            Drawing.DrawRectangle(buf, bw, bh, ptd.X - half, ptd.Y - half, ptd.X + half, ptd.Y + half, col.ToArgb(), false);
        }

        public void DrawSquare(Color col, float x, float y, float r, bool pixelSize) {
            this.DrawSquare(col, new PointF(x, y), r, pixelSize);
        }

        public void FillSquare(Color col, PointF pt, float size, bool pixelSize) {
            Point ptd = ib.ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ib.GetZoomFactor(), MidpointRounding.AwayFromZero);
            int half = sized / 2;
            Drawing.DrawRectangle(buf, bw, bh, ptd.X - half, ptd.Y - half, ptd.X + half, ptd.Y + half, col.ToArgb(), true);
        }

        public void FillSquare(Color col, float x, float y, float r, bool pixelSize) {
            this.FillSquare(col, new PointF(x, y), r, pixelSize);
        }

        public void DrawCross(Color col, PointF pt, float size, bool pixelSize) {
            Point ptd = ib.ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ib.GetZoomFactor(), MidpointRounding.AwayFromZero);
            int half = sized / 2;
            int iCol = col.ToArgb();
            Drawing.DrawLine(buf, bw, bh, ptd.X - half, ptd.Y - half, ptd.X + half, ptd.Y + half, iCol);
            Drawing.DrawLine(buf, bw, bh, ptd.X - half, ptd.Y + half, ptd.X + half, ptd.Y - half, iCol);
        }

        public void DrawCross(Color col, float x, float y, float r, bool pixelSize) {
            this.DrawCross(col, new PointF(x, y), r, pixelSize);
        }

        public void DrawPlus(Color col, PointF pt, float size, bool pixelSize) {
            Point ptd = ib.ImgToDisp(pt);
            int sized = (pixelSize) ? (int)size : (int)Math.Round(size * ib.GetZoomFactor(), MidpointRounding.AwayFromZero);
            int half = sized / 2;
            int iCol = col.ToArgb();
            Drawing.DrawLine(buf, bw, bh, ptd.X, ptd.Y - half, ptd.X, ptd.Y + half, iCol);
            Drawing.DrawLine(buf, bw, bh, ptd.X - half, ptd.Y, ptd.X + half, ptd.Y, iCol);
        }

        public void DrawPlus(Color col, float x, float y, float r, bool pixelSize) {
            this.DrawPlus(col, new PointF(x, y), r, pixelSize);
        }

        public void DrawString(string text, IFont font, Color col, PointF pt, Color? backColor = null) {
            Point ptd = ib.ImgToDisp(pt);
            DrawStringWnd(text, font, col, ptd, backColor);
        }

        public void DrawString(string text, IFont font, Color col, float x, float y, Color? backColor = null) {
            DrawString(text, font, col, new PointF(x, y), backColor);
        }

        public void DrawStringWnd(string text, IFont font, Color col, Point ptd, Color? backColor = null) {
            if (backColor != null) {
                var size = font.MeasureString(text);
                Drawing.DrawRectangle(buf, bw, bh, ptd.X, ptd.Y, ptd.X + size.Width, ptd.Y + size.Height, backColor.Value.ToArgb(), true);
            }
            font.DrawString(text, buf, bw, bh, ptd.X, ptd.Y, col);
        }

        public void DrawStringWnd(string text, IFont font, Color col, int x, int y, Color? backColor = null) {
            DrawStringWnd(text, font, col, new Point(x, y), backColor);
        }

        public Size MeasureString(string text, IFont font) {
            return font.MeasureString(text);
        }
    }
}
