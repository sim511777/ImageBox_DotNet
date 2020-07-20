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

namespace ImageBox {
    public partial class ImageBox : Control {
        public ImageBox() {
            InitializeComponent();
            DoubleBuffered = true;
        }

        private static double GetTime() {
            return (double)Stopwatch.GetTimestamp() / Stopwatch.Frequency;
        }

        private static double GetTimeMs() {
            return GetTime() * 1000.0;
        }

        // 이미지 버퍼를 디스플레이 버퍼에 복사
        private static unsafe void DrawImageZoom(IntPtr sbuf, int sbw, int sbh, IntPtr dbuf, int dbw, int dbh, Int64 panx, Int64 pany, double zoom, int bytepp, int bgColor, bool bufIsFloat) {
            // 인덱스 버퍼 생성
            int[] siys = new int[dbh];
            int[] sixs = new int[dbw];
            for (int y = 0; y < dbh; y++) {
                int siy = (int)Math.Floor((y - pany) / zoom);
                siys[y] = (sbuf == IntPtr.Zero || siy < 0 || siy >= sbh) ? -1 : siy;
            }
            for (int x = 0; x < dbw; x++) {
                int six = (int)Math.Floor((x - panx) / zoom);
                sixs[x] = (sbuf == IntPtr.Zero || six < 0 || six >= sbw) ? -1 : six;
            }

            for (int y = 0; y < dbh; y++) {
                int siy = siys[y];
                byte* sptr = (byte*)sbuf + (Int64)sbw * siy * bytepp;
                int* dp = (int*)dbuf + (Int64)dbw * y;
                for (int x = 0; x < dbw; x++, dp++) {
                    int six = sixs[x];
                    if (siy == -1 || six == -1) {   // out of boundary of image
                        *dp = bgColor;
                    } else {
                        byte* sp = &sptr[six * bytepp];
                        if (bufIsFloat) {
                            if (bytepp == 4) {
                                int v = (int)*(float*)sp;
                                *dp = v | v << 8 | v << 16 | 0xff << 24;
                            } else if (bytepp == 8) {
                                int v = (int)*(double*)sp;
                                *dp = v | v << 8 | v << 16 | 0xff << 24;
                            }
                        } else {
                            if (bytepp == 1) {          // 8bit gray
                                int v = sp[0];
                                *dp = v | v << 8 | v << 16 | 0xff << 24;
                            } else if (bytepp == 2) {   // 16bit gray (*.hra)
                                int v = sp[0];
                                *dp = v | v << 8 | v << 16 | 0xff << 24;
                            } else if (bytepp == 3) {   // 24bit bgr
                                *dp = sp[0] | sp[1] << 8 | sp[2] << 16 | 0xff << 24;
                            } else if (bytepp == 4) {   // 32bit bgra
                                *dp = sp[0] | sp[1] << 8 | sp[2] << 16 | 0xff << 24;
                            }
                        }
                    }
                }
            }
        }

        private IntPtr imgBuf = IntPtr.Zero;    // 이미지 버퍼
        private int imgBw = 0;
        private int imgBh = 0;
        private int bytepp = 0;
        private bool bufIsFloat = false;

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
        public double GetZoomFactor() {
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

        // 디버그 정보 표시
        public bool UseDrawDebufInfo { get; set; }

        // 이미지 버퍼 설정
        public void SetImageBuffer(IntPtr _buf, int _bw, int _bh, int _bytepp, bool _bufIsFloat) {
            imgBuf = _buf;
            imgBw = _bw;
            imgBh = _bh;
            bytepp = _bytepp;
            bufIsFloat = _bufIsFloat;
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
            dispBw = Width;
            dispBh = Height;
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
                using (Bitmap bmp = new Bitmap(200, Font.Height)) {
                    using (Graphics bg = Graphics.FromImage(bmp)) {
                        DrawCursorInfo(bg, 0, 0);
                    }
                    using (Graphics g = CreateGraphics()) {
                        g.DrawImage(bmp, 2, 2);
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

            var ig = new ImageGraphics(this, g);
            double zoom = GetZoomFactor();
            var t0 = GetTimeMs();
            DrawImageZoom(imgBuf, imgBw, imgBh, dispBuf, dispBw, dispBh, PtPan.X, PtPan.Y, zoom, bytepp, BackColor.ToArgb(), bufIsFloat);
            var t1 = GetTimeMs();
            g.DrawImage(dispBmp, 0, 0);
            var t2 = GetTimeMs();
            DrawPixelValue(ig);
            var t3 = GetTimeMs();
            DrawCenterLine(ig);
            var t4 = GetTimeMs();
            base.OnPaint(pe);   // 여기서 사용자가 정의한 Paint이벤트 함수가 호출됨
            var t5 = GetTimeMs();
            DrawCursorInfo(g, 2, 2);
            var t6 = GetTimeMs();
            
            if (UseDrawDebufInfo) {
                string debugInfo = string.Format(
@"Image : {0}
DrawImageZoom : {1:f2}ms
DrawImage : {2:f2}ms
DrawPixelValue : {3:f2}ms
DrawCenterLine : {4:f2}ms
OnPaint : {5:f2}ms
DrawCursorInfo : {6:f2}ms
Total : {7:f2}ms",
                    imgBuf == IntPtr.Zero ? "null" : string.Format("{0}x{1},{2}byte", imgBw, imgBh, bytepp),
                    t1 - t0,
                    t2 - t1,
                    t3 - t2,
                    t4 - t3,
                    t5 - t4,
                    t6 - t5,
                    t6 - t0);
                DrawDebufInfo(g, debugInfo);
            }
        }

        // 디버그 정보 표시
        private void DrawDebufInfo(Graphics g, string debugInfo) {
            var size = g.MeasureString(debugInfo, Font);
            g.FillRectangle(Brushes.White, Width - size.Width, 0, size.Width, size.Height);
            g.DrawString(debugInfo, Font, Brushes.Black, Width - size.Width, 0);
        }

        // 픽셀값 표시
        private void DrawPixelValue(ImageGraphics ig) {
            if (imgBuf == IntPtr.Zero)
                return;
            var zoom = GetZoomFactor();
            if (zoom < 16 || (bytepp != 1 && zoom < 32))
                return;

            float fontSize = 0;
            if (bytepp == 1) {
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
                        ig.DrawString(pixelValueText, font, pseudo[colIdx], ix - 0.5f, iy - 0.5f);
                    }
                }
            }
        }

        // 픽셀 표지 문자열
        private unsafe string GetImagePixelValueText(int ix, int iy, bool multiLine) {
            if (imgBuf == IntPtr.Zero || ix < 0 || ix >= imgBw || iy < 0 || iy >= imgBh)
                return string.Empty;
            var ptr = (byte*)imgBuf.ToPointer() + ((long)imgBw * iy + ix) * bytepp;
            if (bytepp == 1) {
                return (*ptr).ToString();
            } else {
                return string.Format(multiLine ? "{0},\r\n{1},\r\n{2}" : "{0},{1},{2}", ptr[2], ptr[1], ptr[0]);
            }
        }

        // 픽셀 표시 컬러 인덱스
        private unsafe int GetImagePixelValueColorIndex(int ix, int iy) {
            if (imgBuf == IntPtr.Zero || ix < 0 || ix >= imgBw || iy < 0 || iy >= imgBh)
                return 0;
            var ptr = (byte*)imgBuf.ToPointer() + ((long)imgBw * iy + ix) * bytepp;
            if (bytepp == 1) {
                return (*ptr) / 32;
            } else {
                return (ptr[2] + ptr[1] + ptr[0]) / 96;
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
        private void DrawCenterLine(ImageGraphics ig) {
            if (imgBuf == IntPtr.Zero)
                return;
            using (var pen = new Pen(Color.Yellow)) {
                pen.DashStyle = DashStyle.Dot;
                ig.DrawLine(pen, imgBw / 2.0f - 0.5f, -0.5f, imgBw / 2.0f - 0.5f, imgBh - 0.5f);
                ig.DrawLine(pen, -0.5f, imgBh / 2.0f - 0.5f, imgBw - 0.5f, imgBh / 2.0f - 0.5f);
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
    }
}
