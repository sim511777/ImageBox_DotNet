using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShimLib {
    public class Drawing {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void DrawPixel(int* ptr, int bw, int bh, int x, int y, int iCol) {
            if (x >= 0 && x < bw && y >= 0 && y < bh)
                *(ptr + bw * y + x) = iCol;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void DrawHLine(int* ptr, int bw, int bh, int x1, int x2, int y, int iCol) {
            if (y < 0 || y >= bh || x1 >= bw || x2 < 0)
                return;

            if (x1 < 0)
                x1 = 0;
            if (x2 >= bw)
                x2 = bw - 1;

            int* ptr1 = ptr + (bw * y) + x1;
            int size = x2 - x1 + 1;
            while (size-- > 0)
                *ptr1++ = iCol;
        }

        public static unsafe void DrawHLineDot(IntPtr buf, int bw, int bh, int x1, int x2, int y, int iCol) {
            if (y < 0 || y >= bh || x1 >= bw || x2 < 0)
                return;

            if (x1 < 0)
                x1 = 0;
            if (x2 >= bw)
                x2 = bw - 1;

            int* ptr = (int*)buf + (bw * y) + x1;
            int size = x2 - x1 + 1;

            while (size > 0) {
                *ptr = iCol;
                ptr += 2;
                size -= 2;
            }
        }

        public static unsafe void DrawVLineDot(IntPtr buf, int bw, int bh, int y1, int y2, int x, int iCol) {
            if (x < 0 || x >= bw || y1 >= bh || y2 < 0)
                return;

            if (y1 < 0)
                y1 = 0;
            if (y2 >= bh)
                y2 = bh - 1;

            int* ptr = (int*)buf + (bw * y1) + x;
            int size = y2 - y1 + 1;
            while (size > 0) {
                *ptr = iCol;
                ptr += 2 * bw;
                size -= 2;
            }
        }

        public static unsafe void DrawRectangleDot(IntPtr buf, int bw, int bh, int x1, int y1, int x2, int y2, int iCol) {
            if (x1 > x2) Util.Swap(ref x1, ref x2);
            if (y1 > y2) Util.Swap(ref y1, ref y2);
            if (x1 >= bw || x2 < 0 || y1 >= bh || y2 < 0)
                return;
            DrawHLineDot(buf, bw, bh, x1, x2, y1, iCol);
            DrawHLineDot(buf, bw, bh, x1, x2, y2, iCol);
            DrawVLineDot(buf, bw, bh, y1, y2, x1, iCol);
            DrawVLineDot(buf, bw, bh, y1, y2, x2, iCol);
        }

        public static unsafe void DrawPixel(IntPtr buf, int bw, int bh, int x, int y, int iCol) {
            DrawPixel((int*)buf, bw, bh, x, y, iCol);
        }

        public static unsafe void DrawLine(IntPtr buf, int bw, int bh, int x1, int y1, int x2, int y2, int iCol) {
            if ((x1 < 0 && x2 < 0) || (x1 >= bw && x2 > bw) || (y1 < 0 && y2 < 0) || (y1 >= bh && y2 >= bh))
                return;

            int dx = (x2 > x1) ? (x2 - x1) : (x1 - x2);
            int dy = (y2 > y1) ? (y2 - y1) : (y1 - y2);
            int sx = (x2 > x1) ? 1 : -1;
            int sy = (y2 > y1) ? 1 : -1;
            int dx2 = dx * 2;
            int dy2 = dy * 2;

            int* ptr = (int*)buf;
            int x = x1;
            int y = y1;
            if (dy < dx) {
                int d = dy2 - dx;
                while (x != x2) {
                    DrawPixel(ptr, bw, bh, x, y, iCol);
                    x += sx;
                    d += dy2;
                    if (d > 0) {
                        y += sy;
                        d -= dx2;
                    }
                }
            } else {
                int d = dx2 - dy;
                while (y != y2) {
                    DrawPixel(ptr, bw, bh, x, y, iCol);
                    y += sy;
                    d += dx2;
                    if (d > 0) {
                        x += sx;
                        d -= dy2;
                    }
                }
            }
            DrawPixel(ptr, bw, bh, x2, y2, iCol); // 끝점도 찍음
        }

        public static unsafe void DrawCircle(IntPtr buf, int bw, int bh, int cx, int cy, int r, int iCol, bool fill) {
            int x1 = cx - r;
            int x2 = cx + r;
            int y1 = cy - r;
            int y2 = cy + r;
            if (x1 >= bw || x2 < 0 || y1 >= bh || y2 < 0)
                return;

            int* ptr = (int*)buf;

            int h;
            int x, y;
            int deltaE, deltaNE;
            x = 0;
            y = r;
            h = 1 - r;
            deltaE = 3;
            deltaNE = 5 - 2 * r;
            while (x <= y) {
                if (fill) {
                    DrawHLine(ptr, bw, bh, cx - x, cx + x, cy + y, iCol);
                    DrawHLine(ptr, bw, bh, cx - y, cx + y, cy + x, iCol);
                    DrawHLine(ptr, bw, bh, cx - y, cx + y, cy - x, iCol);
                    DrawHLine(ptr, bw, bh, cx - x, cx + x, cy - y, iCol);
                } else {
                    DrawPixel(ptr, bw, bh, cx + x, cy + y, iCol);
                    DrawPixel(ptr, bw, bh, cx + y, cy + x, iCol);
                    DrawPixel(ptr, bw, bh, cx + y, cy - x, iCol);
                    DrawPixel(ptr, bw, bh, cx + x, cy - y, iCol);
                    DrawPixel(ptr, bw, bh, cx - x, cy - y, iCol);
                    DrawPixel(ptr, bw, bh, cx - y, cy - x, iCol);
                    DrawPixel(ptr, bw, bh, cx - y, cy + x, iCol);
                    DrawPixel(ptr, bw, bh, cx - x, cy + y, iCol);
                }
                if (h < 0)    /* case E */ {
                    h += deltaE;
                    deltaE += 2;
                    deltaNE += 2;
                } else    /* case NE */ {
                    h += deltaNE;
                    deltaE += 2;
                    deltaNE += 4;
                    y--;
                }
                x++;
            }
        }

        public static unsafe void DrawEllipse(IntPtr buf, int bw, int bh, int cx, int cy, int rx, int ry, int iCol, bool fill) {
            int x1 = cx - rx;
            int x2 = cx + rx;
            int y1 = cy - ry;
            int y2 = cy + ry;
            if (x1 >= bw || x2 < 0 || y1 >= bh || y2 < 0)
                return;

            int* ptr = (int*)buf;

            int h;
            int a2 = rx * rx, b2 = ry * ry;
            int a82 = 8 * a2, b82 = 8 * b2;
            int x, y;
            int deltaE, deltaNE;
            x = 0;
            y = ry;
            h = 4 * b2 + a2 - 4 * a2 * ry;
            deltaE = 12 * b2;
            deltaNE = 12 * b2 - 8 * a2 * ry + 8 * a2;
            while (b2 * x <= a2 * y) {
                if (fill) {
                    DrawHLine(ptr, bw, bh, cx - x, cx + x, cy + y, iCol);
                    DrawHLine(ptr, bw, bh, cx - x, cx + x, cy - y, iCol);
                } else {
                    DrawPixel(ptr, bw, bh, cx + x, cy + y, iCol);
                    DrawPixel(ptr, bw, bh, cx + x, cy - y, iCol);
                    DrawPixel(ptr, bw, bh, cx - x, cy - y, iCol);
                    DrawPixel(ptr, bw, bh, cx - x, cy + y, iCol);
                }
                if (h < 0) {    /* case E */
                    h += deltaE;
                    deltaE += b82;
                    deltaNE += b82;
                } else {   /* case NE */
                    h += deltaNE;
                    deltaE += b82;
                    deltaNE += b82 + a82;
                    y--;
                }
                x++;
            }
            x = rx;
            y = 0;
            h = 4 * a2 + a2 - 4 * rx * b2;
            deltaE = 12 * a2;
            deltaNE = 12 * a2 - 8 * b2 * rx + 8 * b2;
            while (b2 * x > a2 * y) {
                if (fill) {
                    DrawHLine(ptr, bw, bh, cx - x, cx + x, cy + y, iCol);
                    DrawHLine(ptr, bw, bh, cx - x, cx + x, cy - y, iCol);
                } else {
                    DrawPixel(ptr, bw, bh, cx + x, cy + y, iCol);
                    DrawPixel(ptr, bw, bh, cx + x, cy - y, iCol);
                    DrawPixel(ptr, bw, bh, cx - x, cy - y, iCol);
                    DrawPixel(ptr, bw, bh, cx - x, cy + y, iCol);
                }
                if (h < 0) {   /* case E */
                    h += deltaE;
                    deltaE += a82;
                    deltaNE += a82;
                } else {   /* case NE */
                    h += deltaNE;
                    deltaE += a82;
                    deltaNE += b82 + a82;
                    x--;
                }
                y++;
            }
        }

        public static unsafe void DrawRectangle(IntPtr buf, int bw, int bh, int x1, int y1, int x2, int y2, int iCol, bool fill) {
            if (x1 > x2) Util.Swap(ref x1, ref x2);
            if (y1 > y2) Util.Swap(ref y1, ref y2);
            if (x1 >= bw || x2 < 0 || y1 >= bh || y2 < 0)
                return;

            if (fill) {
                int* ptr = (int*)buf;
                for (int y = y1; y <= y2; y++) {
                    DrawHLine(ptr, bw, bh, x1, x2, y, iCol);
                }
            } else {
                DrawLine(buf, bw, bh, x1, y1, x2, y1, iCol);
                DrawLine(buf, bw, bh, x2, y1, x2, y2, iCol);
                DrawLine(buf, bw, bh, x2, y2, x1, y2, iCol);
                DrawLine(buf, bw, bh, x1, y2, x1, y1, iCol);
            }
        }

        public static unsafe void DrawImage(IntPtr buf, int bw, int bh, IntPtr sbuf, int sw, int sh, int x0, int y0) {
            if (x0 >= bw || x0 <= -sw || y0 >= bh || y0 <= -sh)
                return;

            int dx1 = x0;
            int dx2 = x0 + sw;
            int sx1 = 0;
            int sx2 = sw;

            if (dx1 < 0) {
                sx1 -= dx1;
                dx1 = 0;
            }
            if (dx2 > bw) {
                sx2 -= dx2 - bw; ;
                dx2 = bw;
            }
            int copyw = dx2 - dx1;

            int dy1 = y0;
            int dy2 = y0 + sh;
            int sy1 = 0;
            int sy2 = sh;

            if (dy1 < 0) {
                sy1 -= dy1;
                dy1 = 0;
            }
            if (dy2 > bh) {
                sy2 -= dy2 - bh;
                dy2 = bh;
            }

            for (int dy = dy1, sy = sy1; dy < dy2; dy++, sy++) {
                int* dptr = (int*)buf + bw * dy + dx1;
                int* sptr = (int*)sbuf + sw * sy + sx1;
                Util.Memcpy((IntPtr)dptr, (IntPtr)sptr, copyw * 4);
            }
        }
    }

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

        public void DrawString(string text, BitmapFont font, Color col, PointF pt, Color? backColor = null) {
            Point ptd = ib.ImgToDisp(pt);
            DrawStringWnd(text, font, col, ptd, backColor);
        }

        public void DrawString(string text, BitmapFont font, Color col, float x, float y, Color? backColor = null) {
            DrawString(text, font, col, new PointF(x, y), backColor);
        }

        public void DrawStringWnd(string text, BitmapFont font, Color col, Point ptd, Color? backColor = null) {
            if (backColor != null) {
                var size = font.MeasureString(text);
                Drawing.DrawRectangle(buf, bw, bh, ptd.X, ptd.Y, ptd.X + size.Width, ptd.Y + size.Height, backColor.Value.ToArgb(), true);
            }
            font.DrawString(text, buf, bw, bh, ptd.X, ptd.Y, col);
        }

        public void DrawStringWnd(string text, BitmapFont font, Color col, int x, int y, Color? backColor = null) {
            DrawStringWnd(text, font, col, new Point(x, y), backColor);
        }

        public Size MeasureString(string text, BitmapFont font) {
            return font.MeasureString(text);
        }
    }
}
