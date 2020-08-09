using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ShimLib {
    public partial class ImageBox : Control {
        public ImageBox() {
            InitializeComponent();
            DoubleBuffered = true;

            UseDrawPixelValue = true;
            UseDrawCenterLine = true;
            UseDrawCursorInfo = true;
        }

        // 이미지 버퍼를 디스플레이 버퍼에 복사
        private static unsafe void CopyImageBufferZoom(IntPtr imgBuf, int imgBw, int imgBh, int bytepp, bool bufIsFloat, IntPtr dispBuf, int dispBw, int dispBh, int panx, int pany, double zoom, int bgColor) {
            // 인덱스 버퍼 생성
            int[] siys = new int[dispBh];
            int[] sixs = new int[dispBw];
            for (int y = 0; y < dispBh; y++) {
                int siy = (int)Math.Floor((y - pany) / zoom);
                siys[y] = (imgBuf == IntPtr.Zero || siy < 0 || siy >= imgBh) ? -1 : siy;
            }
            for (int x = 0; x < dispBw; x++) {
                int six = (int)Math.Floor((x - panx) / zoom);
                sixs[x] = (imgBuf == IntPtr.Zero || six < 0 || six >= imgBw) ? -1 : six;
            }

            for (int y = 0; y < dispBh; y++) {
                int siy = siys[y];
                byte* sptr = (byte*)imgBuf + (Int64)imgBw * siy * bytepp;
                int* dp = (int*)dispBuf + (Int64)dispBw * y;
                for (int x = 0; x < dispBw; x++, dp++) {
                    int six = sixs[x];
                    if (siy == -1 || six == -1) {       // out of boundary of image
                        *dp = bgColor;
                    } else {
                        byte* sp = &sptr[six * bytepp];
                        if (bufIsFloat) {
                            if (bytepp == 4) {          // 4byte float gray
                                int v = (int)*(float*)sp;
                                *dp = v | v << 8 | v << 16 | 0xff << 24;
                            } else if (bytepp == 8) {   // 8byte double gray
                                int v = (int)*(double*)sp;
                                *dp = v | v << 8 | v << 16 | 0xff << 24;
                            }
                        } else {
                            if (bytepp == 1) {          // 1byte gray
                                int v = sp[0];
                                *dp = v | v << 8 | v << 16 | 0xff << 24;
                            } else if (bytepp == 2) {   // 2byte gray (*.hra)
                                int v = sp[0];
                                *dp = v | v << 8 | v << 16 | 0xff << 24;
                            } else if (bytepp == 3) {   // 3byte bgr
                                *dp = sp[0] | sp[1] << 8 | sp[2] << 16 | 0xff << 24;
                            } else if (bytepp == 4) {   // rbyte bgra
                                *dp = sp[0] | sp[1] << 8 | sp[2] << 16 | 0xff << 24;
                            }
                        }
                    }
                }
            }
        }

        // 표시 옵션
        public bool UseDrawPixelValue { get; set; }
        public bool UseDrawCenterLine { get; set; }
        public bool UseDrawCursorInfo { get; set; }

        // 이미지 버퍼 정보
        private IntPtr imgBuf = IntPtr.Zero;
        private int imgBw = 0;
        private int imgBh = 0;
        private int imgBytepp = 0;
        private bool isImgbufFloat = false;

        // 디스플레이 버퍼 정보
        private IntPtr dispBuf = IntPtr.Zero;
        private int dispBw = 0;
        private int dispBh = 0;
        private Bitmap dispBmp = null;

        // 패닝 옵셋(픽셀)
        private Point ptPan = new Point(2, 2);
        private Point PtPan {
            get { return ptPan; }
            set {
                if (imgBuf == IntPtr.Zero)
                    ptPan = new Point(2, 2);
                else {
                    var zoom = GetZoomFactor();
                    int x = Math.Max(Math.Min(value.X, 2), -(int)(imgBw * zoom) + 2);
                    int y = Math.Max(Math.Min(value.Y, 2), -(int)(imgBh * zoom) + 2);
                    ptPan = new Point(x, y);
                }
            } 
        }
        // 줌 레벨(0 = 1x)
        private int zoomLevel = 0;
        private int ZoomLevel {
            get { return zoomLevel; }
            set { zoomLevel = Math.Max(Math.Min(value, 16), -16); }
        }
        private void GetZoomFactorComponents(out int exp_num, out int c) {
            exp_num = (ZoomLevel >= 0) ? ZoomLevel / 2 : (ZoomLevel - 1) / 2;
            if (ZoomLevel % 2 != 0)
                exp_num--;
            c = (ZoomLevel % 2 != 0) ? 3 : 1;
        }
        private double GetZoomFactor() {
            int exp_num;
            int c;
            GetZoomFactorComponents(out exp_num, out c);
            return c * Math.Pow(2, exp_num);
        }
        private string GetZoomText() {
            int exp_num;
            int c;
            GetZoomFactorComponents(out exp_num, out c);
            return (exp_num >= 0) ? (c * (int)Math.Pow(2, exp_num)).ToString() : c.ToString() + "/" + ((int)Math.Pow(2, -exp_num)).ToString();
        }

        // 이미지 버퍼 설정
        public void SetImageBuffer(IntPtr _buf, int _bw, int _bh, int _bytepp, bool _isFloat) {
            imgBuf = _buf;
            imgBw = _bw;
            imgBh = _bh;
            imgBytepp = _bytepp;
            isImgbufFloat = _isFloat;
        }

        // 줌 리셋
        public void ResetZoom() {
            PtPan = new Point(2, 2);
            ZoomLevel = 0;
        }

        // 이미지 좌표 -> 화면 좌료
        public Point ImgToDisp(PointF ptImg) {
            double zoom = GetZoomFactor();
            int dispX = (int)Math.Floor((ptImg.X + 0.5) * zoom + PtPan.X);
            int dispY = (int)Math.Floor((ptImg.Y + 0.5) * zoom + PtPan.Y);
            return new Point(dispX, dispY);
        }

        // 이미지 좌표 -> 화면 좌료
        public Rectangle ImgToDisp(RectangleF rectImg) {
            double zoom = GetZoomFactor();
            int dispX = (int)Math.Floor((rectImg.X + 0.5) * zoom + PtPan.X);
            int dispY = (int)Math.Floor((rectImg.Y + 0.5) * zoom + PtPan.Y);
            int dispWidth = (int)Math.Floor(rectImg.Width * zoom);
            int dispHeight = (int)Math.Floor(rectImg.Height * zoom);
            return new Rectangle(dispX, dispY, dispWidth, dispHeight);
        }

        // 화면 좌표 -> 이미지 좌표
        public PointF DispToImg(Point ptDisp) {
            double zoom = GetZoomFactor();
            float imgX = (float)((ptDisp.X - PtPan.X) / zoom - 0.5);
            float imgY = (float)((ptDisp.Y - PtPan.Y) / zoom - 0.5);
            return new PointF(imgX, imgY);
        }

        // 리사이즈
        protected override void OnLayout(LayoutEventArgs levent) {
            if (dispBmp != null) {
                dispBmp.Dispose();
                Marshal.FreeHGlobal(dispBuf);
            }
            dispBw = Math.Max(Width, 64);
            dispBh = Math.Max(Height, 64);
            dispBuf = Marshal.AllocHGlobal(dispBw * 4 * dispBh);
            dispBmp = new Bitmap(dispBw, dispBh, dispBw * 4, PixelFormat.Format32bppPArgb, dispBuf);
            Invalidate();
            base.OnLayout(levent);
        }

        // 마우스 패닝
        private Point ptMove = new Point(-1, -1);
        private Point ptDown;
        private bool bDown = false;
        protected override void OnMouseDown(MouseEventArgs e) {
            ptDown = e.Location;
            bDown = true;
            base.OnMouseDown(e);
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            ptMove = e.Location;
            if (bDown) {
                PtPan += (Size)e.Location - (Size)ptDown;
                ptDown = e.Location;
                Invalidate();
            } else {
                if (UseDrawCursorInfo) {
                    using (Bitmap bmp = new Bitmap(200, Font.Height)) {
                        using (Graphics bg = Graphics.FromImage(bmp)) {
                            DrawCursorInfo(bg, 0, 0);
                        }
                        using (Graphics g = CreateGraphics()) {
                            g.DrawImage(bmp, 2, 2);
                        }
                    }
                }
            }
            base.OnMouseMove(e);
        }
        protected override void OnMouseUp(MouseEventArgs e) {
            bDown = false;
            base.OnMouseUp(e);
        }

        // 마우스주밍
        protected override void OnMouseWheel(MouseEventArgs e) {
            var ptImg = DispToImg(e.Location);
            ZoomLevel += (e.Delta > 0) ? 1 : -1;
            var ptDisp = ImgToDisp(ptImg);
            PtPan += ((Size)e.Location - (Size)ptDisp);
            Invalidate();
            base.OnMouseWheel(e);
        }
        
        // 배경컬러 배경이미지 아무것도 안그림
        protected override void OnPaintBackground(PaintEventArgs pevent) { }

        // 페인트
        protected override void OnPaint(PaintEventArgs pe) {
            var g = pe.Graphics;
            if (DesignMode) {
                g.Clear(BackColor);
                g.DrawString(Name, Font, Brushes.Black, 0, 0);
                return;
            }

            double zoom = GetZoomFactor();
            CopyImageBufferZoom(imgBuf, imgBw, imgBh, imgBytepp, isImgbufFloat, dispBuf, dispBw, dispBh, PtPan.X, PtPan.Y, zoom, BackColor.ToArgb());
            g.DrawImage(dispBmp, 0, 0);
            if (UseDrawPixelValue)
                DrawPixelValue(g);
            if (UseDrawCenterLine)
                DrawCenterLine(g);
            base.OnPaint(pe);   // 여기서 사용자가 정의한 Paint이벤트 함수가 호출됨
            if (UseDrawCursorInfo)
                DrawCursorInfo(g, 2, 2);
        }

        // 픽셀값 표시
        private void DrawPixelValue(Graphics g) {
            if (imgBuf == IntPtr.Zero)
                return;
            var zoom = GetZoomFactor();
            if (zoom < 16 || (imgBytepp != 1 && zoom < 32))
                return;

            float fontSize = 0;
            if (imgBytepp == 1) {
                if (zoom <= 17) fontSize = 6;
                else if (zoom <= 25) fontSize = 8;
                else fontSize = 10;
            } else {
                if (zoom <= 33) fontSize = 6;
                else if (zoom <= 49) fontSize = 8;
                else fontSize = 10;
            }

            using (var font = new Font("arial", fontSize)) {
                var ptImgLT = DispToImg(Point.Empty);
                var ptImgRB = DispToImg((Point)Size);
                int ix1 = (int)Math.Round(ptImgLT.X);
                int iy1 = (int)Math.Round(ptImgLT.Y);
                int ix2 = (int)Math.Round(ptImgRB.X);
                int iy2 = (int)Math.Round(ptImgRB.Y);
                ix1 = Math.Max(ix1, 0);
                iy1 = Math.Max(iy1, 0);
                ix2 = Math.Min(ix2, imgBw - 1);
                iy2 = Math.Min(iy2, imgBh - 1);
                for (int iy = iy1; iy <= iy2; iy++) {
                    for (int ix = ix1; ix <= ix2; ix++) {
                        string pixelValueText = GetImagePixelValueText(ix, iy, true);
                        int colIdx = GetImagePixelValueColorIndex(ix, iy);
                        DrawString(g, pixelValueText, font, pseudo[colIdx], ix - 0.5f, iy - 0.5f);
                    }
                }
            }
        }

        // 픽셀 표지 문자열
        private unsafe string GetImagePixelValueText(int ix, int iy, bool multiLine) {
            if (imgBuf == IntPtr.Zero || ix < 0 || ix >= imgBw || iy < 0 || iy >= imgBh)
                return string.Empty;
            var ptr = (byte*)imgBuf.ToPointer() + ((long)imgBw * iy + ix) * imgBytepp;
            if (imgBytepp == 1) {
                return (*ptr).ToString();
            } else {
                if (isImgbufFloat) {
                    if (imgBytepp == 4)
                        return string.Format("{0:f2}",*(float*)ptr);
                    else
                        return string.Format("{0:f2}", *(double*)ptr);
                } else {
                    return string.Format(multiLine ? "{0},\r\n{1},\r\n{2}" : "{0},{1},{2}", ptr[2], ptr[1], ptr[0]);
                }
            }
        }

        // 픽셀 표시 컬러 인덱스
        private unsafe int GetImagePixelValueColorIndex(int ix, int iy) {
            if (imgBuf == IntPtr.Zero || ix < 0 || ix >= imgBw || iy < 0 || iy >= imgBh)
                return 0;
            var ptr = (byte*)imgBuf.ToPointer() + ((long)imgBw * iy + ix) * imgBytepp;
            if (imgBytepp == 1) {
                return (*ptr) / 32;
            } else {
                if (isImgbufFloat) {
                    if (imgBytepp == 4)
                        return Math.Min(Math.Max((int)*(float*)ptr, 0), 255) / 32;
                    else
                        return Math.Min(Math.Max((int)*(double*)ptr, 0), 255) / 32;
                } else {
                    return (ptr[2] + ptr[1] + ptr[0]) / 96;
                }
            }
        }

        // 픽셀값 표시 컬러
        private static readonly Brush[] pseudo = {
            Brushes.White,      // 0~31
            Brushes.Cyan,       // 32~63
            Brushes.DodgerBlue, // 63~95
            Brushes.Yellow,     // 96~127
            Brushes.Brown,      // 128~159
            Brushes.DarkViolet, // 160~191
            Brushes.Red    ,    // 192~223
            Brushes.Black,      // 224~255
        };

        // 중심선 표시
        private void DrawCenterLine(Graphics g) {
            if (imgBuf == IntPtr.Zero)
                return;
            using (var pen = new Pen(Color.Yellow)) {
                pen.DashStyle = DashStyle.Dot;
                DrawLine(g, pen, imgBw / 2.0f - 0.5f, -0.5f, imgBw / 2.0f - 0.5f, imgBh - 0.5f);
                DrawLine(g, pen, -0.5f, imgBh / 2.0f - 0.5f, imgBw - 0.5f, imgBh / 2.0f - 0.5f);
            }
        }

        // 커서 정보 표시
        private void DrawCursorInfo(Graphics g, int ofsx, int ofsy) {
            var ptImg = DispToImg(ptMove);
            int ix = (int)Math.Round(ptImg.X);
            int iy = (int)Math.Round(ptImg.Y);
            var colText = GetImagePixelValueText(ix, iy, false);
            string zoomText = GetZoomText();
            string text = string.Format("zoom={0} ({1},{2})={3}", zoomText, ix, iy, colText);
            g.FillRectangle(Brushes.Black, ofsx, ofsy, 200, Font.Height);
            g.DrawString(text, Font, Brushes.White, ofsx, ofsy);
        }

        // ==== GDI 함수 ====
        public void DrawLine(Graphics g, Pen pen, PointF pt1, PointF pt2) {
            g.DrawLine(pen, ImgToDisp(pt1), ImgToDisp(pt2));
        }

        public void DrawLine(Graphics g, Pen pen, float x1, float y1, float x2, float y2) {
            this.DrawLine(g, pen, new PointF(x1, y1), new PointF(x2, y2));
        }

        public void DrawString(Graphics g, string s, Font font, Brush brush, PointF pt) {
            g.DrawString(s, font, brush, ImgToDisp(pt));
        }

        public void DrawString(Graphics g, string s, Font font, Brush brush, float x, float y) {
            this.DrawString(g, s, font, brush, new PointF(x, y));
        }

        public void DrawEllipse(Graphics g, Pen pen, RectangleF rect) {
            g.DrawEllipse(pen, ImgToDisp(rect));
        }

        public void DrawEllipse(Graphics g, Pen pen, float x, float y, float width, float height) {
            this.DrawEllipse(g, pen, new RectangleF(x, y, width, height));
        }

        public void DrawRectangle(Graphics g, Pen pen, RectangleF rect) {
            g.DrawRectangle(pen, ImgToDisp(rect));
        }

        public void DrawRectangle(Graphics g, Pen pen, float x, float y, float width, float height) {
            this.DrawRectangle(g, pen, new RectangleF(x, y, width, height));
        }

        public void DrawCircle(Graphics g, Pen pen, float x, float y, float r) {
            float left = x - r;
            float top = y - r;
            float size = r + r;
            this.DrawEllipse(g, pen, left, top, size, size);
        }

        public void DrawCircle(Graphics g, Pen pen, PointF pt, float r) {
            this.DrawCircle(g, pen, pt.X, pt.Y, r);
        }

        public void DrawSquare(Graphics g, Pen pen, float x, float y, float r) {
            float left = x - r;
            float top = y - r;
            float size = r + r;
            this.DrawRectangle(g, pen, left, top, size, size);
        }

        public void DrawSquare(Graphics g, Pen pen, PointF pt, float r) {
            this.DrawSquare(g, pen, pt.X, pt.Y, r);
        }
    }
}
