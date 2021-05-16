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
    public delegate void PaintBackbufferEventHandler(object sender, IntPtr buf, int bw, int bh);

    [ToolboxBitmap(typeof(ImageBox), "ImageBox.bmp")]
    public partial class ImageBox : Control {
        // 표시 옵션
        [Category("ImageBox")] public bool UseDrawPixelValue { get; set; } = true;
        [Category("ImageBox")] public bool UseDrawCenterLine { get; set; } = true;
        [Category("ImageBox")] public bool UseDrawCursorInfo { get; set; } = true;
        [Category("ImageBox")] public bool UseDrawDebugInfo { get; set; } = false;
        [Category("ImageBox")] public bool UseDrawRoiRectangles { get; set; } = true;

        [Category("ImageBox")] public Color CenterLineColor { get; set; } = Color.Yellow;
        [Category("ImageBox")] public Color RoiRectangleColor { get; set; } = Color.Fuchsia;
        [Category("ImageBox")] public double FloatValueMax { get; set; } = 1.0;
        [Category("ImageBox")] public int FloatValueDigit { get; set; } = 3;
        
        [Browsable(false)] public List<Rectangle> RoiList { get; } = new List<Rectangle>();
        [Browsable(false)] private string FloatValueFormat {
            get {
                return $"{{0:.{new string('0', Math.Max(FloatValueDigit, 0))}}}";
            }
        }

        // 생성자
        public ImageBox() {
            InitializeComponent();
            DoubleBuffered = true;
            SetStyle(ControlStyles.Opaque, true);
            this.RoiList = new List<Rectangle>();
        }

        // 백버퍼 그리기 이벤트
        public event PaintBackbufferEventHandler PaintBackBuffer;
        protected void OnPaintBackBuffer(IntPtr buf, int bw, int bh) {
            if (PaintBackBuffer != null)
                PaintBackBuffer(this, buf, bw, bh);
        }

        // 이미지 버퍼 정보
        private IntPtr imgBuf = IntPtr.Zero;
        private int imgBw = 0;
        private int imgBh = 0;
        private int imgBytepp = 0;
        private bool isImgbufFloat = false;

        // 디스플레이 버퍼
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

        protected override void OnMouseDoubleClick(MouseEventArgs e) {
            if (ModifierKeys.HasFlag(Keys.Control) && e.Button == MouseButtons.Left) {
                ShowAbout();
            } else {
                base.OnDoubleClick(e);
            }
        }

        private FormAbout frmAbout = null;
        private void ShowAbout() {
            if (frmAbout == null) {
                frmAbout = new FormAbout(this);
                frmAbout.FormClosed += (o, e) => frmAbout = null;
                frmAbout.Show(this);
            } else {
                frmAbout.Activate();
            }
        }

        // 리사이즈
        protected override void OnLayout(LayoutEventArgs levent) {
            if (dispBmp != null)
                dispBmp.Dispose();

            dispBmp = new Bitmap(Math.Max(Width, 64), Math.Max(Height, 64), PixelFormat.Format32bppPArgb);
            Invalidate();
            base.OnLayout(levent);
        }

        private Point ptMove = new Point(-1, -1);    // 마우스 커서 픽셀정보 표시용
        // 마우스 패닝
        private Point ptPanningOld;
        private bool isPanningDown = false;
        // ROI 입력
        private Point ptRoiStart;
        private Point ptRoiEnd;
        private bool isRoiDown = false;
        private Point ToInt(PointF ptf) { return new Point((int)Math.Ceiling(ptf.X), (int)Math.Ceiling(ptf.Y));}
        protected override void OnMouseDown(MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                if (Control.ModifierKeys.HasFlag(Keys.Control)) {
                    ptRoiEnd = ptRoiStart = ToInt(DispToImg(e.Location));
                    isRoiDown = true;
                } else {
                    ptPanningOld = e.Location;
                    isPanningDown = true;
                }
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            ptMove = e.Location;
            if (isRoiDown) {
                ptRoiEnd = ToInt(DispToImg(e.Location));
                Invalidate();
            } else  if (isPanningDown) {
                PtPan += (Size)e.Location - (Size)ptPanningOld;
                ptPanningOld = e.Location;
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
            if (isRoiDown) {
                ptRoiEnd = ToInt(DispToImg(e.Location));
                int x1 = ptRoiStart.X;
                int y1 = ptRoiStart.Y;
                int x2 = ptRoiEnd.X;
                int y2 = ptRoiEnd.Y;
                if (x1 > x2) Util.Swap(ref x1, ref x2);
                if (y1 > y2) Util.Swap(ref y1, ref y2);
                if (x1 != x2 && y1 != y2) {
                    var roi = new Rectangle(x1, y1, x2 - x1, y2 - y1);
                    RoiList.Add(roi);
                    if (frmAbout != null)
                        frmAbout.UpdateRoiList();
                }
                isRoiDown = false;
                Invalidate();
            }
            isPanningDown = false;
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
        
        double t_01, t_12, t_23, t_34, t_45, t_56, t_67, t_78, t_total;
        // 페인트
        protected override void OnPaint(PaintEventArgs pe) {
            Graphics g = pe.Graphics;
            // 디자인 모드
            if (DesignMode) {
                g.Clear(BackColor);
                g.DrawString(Name, Font, Brushes.Black, 0, 0);
                return;
            }

            var t0 = Util.GetTimeMs();

            var bmpData = dispBmp.LockBits(new Rectangle(Point.Empty, dispBmp.Size), ImageLockMode.WriteOnly, dispBmp.PixelFormat);
            
            // 이미지 확대 축소
            double zoom = GetZoomFactor();
            CopyImageBufferZoom(imgBuf, imgBw, imgBh, imgBytepp, isImgbufFloat, bmpData.Scan0, bmpData.Width, bmpData.Height, PtPan.X, PtPan.Y, zoom, BackColor.ToArgb(), FloatValueMax);
            var t1 = Util.GetTimeMs();

            var id = new ImageDrawing(this, bmpData.Scan0, bmpData.Width, bmpData.Height);

            // 픽셀값 표시
            if (UseDrawPixelValue)
                DrawPixelValue(id);
            var t2 = Util.GetTimeMs();

            if (UseDrawRoiRectangles) {
                DrawRoiRectangles(id);
            }
            if (isRoiDown) {
                DrawRoiDown(id);
            }
            
            // 중심선 표시
            if (UseDrawCenterLine)
                DrawCenterLine(id);
            var t3 = Util.GetTimeMs();

            // PaintBackBuffer이벤트 발생
            OnPaintBackBuffer(bmpData.Scan0, bmpData.Width, bmpData.Height); // 여기서 사용자가 정의한 Paint이벤트 함수가 호출됨
            var t4 = Util.GetTimeMs();

            dispBmp.UnlockBits(bmpData);
            
            // 이미지 그리기
            g.DrawImage(dispBmp, 0, 0);
            var t5 = Util.GetTimeMs();
            
            // Paint이벤트 발생
            base.OnPaint(pe);   // 여기서 사용자가 정의한 Paint이벤트 함수가 호출됨
            var t6 = Util.GetTimeMs();
            
            // 커서 정보 표시
            if (UseDrawCursorInfo)
                DrawCursorInfo(g, 2, 2);
            var t7 = Util.GetTimeMs();

            // 디비그 정보 표시
            if (UseDrawDebugInfo)
                DrawDebugInfo(g);
            var t8 = Util.GetTimeMs();

            t_01 = t1 - t0;
            t_12 = t2 - t1;
            t_23 = t3 - t2;
            t_34 = t4 - t3;
            t_45 = t5 - t4;
            t_56 = t6 - t5;
            t_67 = t7 - t6;
            t_78 = t8 - t7;
            t_total = t8 - t0;
        }

        private void DrawRoiDown(ImageDrawing id) {
            int x1 = ptRoiStart.X;
            int y1 = ptRoiStart.Y;
            int x2 = ptRoiEnd.X;
            int y2 = ptRoiEnd.Y;
            if (x1 > x2) Util.Swap(ref x1, ref x2);
            if (y1 > y2) Util.Swap(ref y1, ref y2);

            var roi = new Rectangle(x1, y1, x2 - x1, y2 - y1);
            var roiF = new RectangleF(roi.X - 0.5f, roi.Y - 0.5f, roi.Width, roi.Height);
            string roiStart = $"({roi.Left},{roi.Top})";
            string roiEnd = $"({roi.Width},{roi.Height})";
            var sizeStart = id.MeasureString(roiStart, Fonts.Ascii_10x18);
            var zoom = GetZoomFactor();
            id.DrawString(roiStart, Fonts.Ascii_10x18, RoiRectangleColor, roiF.X - sizeStart.Width / (float)zoom, roiF.Y - sizeStart.Height / (float)zoom, Color.Yellow);
            id.DrawString(roiEnd, Fonts.Ascii_10x18, RoiRectangleColor, roiF.X + roiF.Width, roiF.Y + roiF.Height, Color.Yellow);
            id.DrawRectangleDot(RoiRectangleColor, roiF);
        }

        private void DrawRoiRectangles(ImageDrawing id) {
            foreach (var roi in RoiList) {
                var roiF = new RectangleF(roi.X - 0.5f, roi.Y - 0.5f, roi.Width, roi.Height);
                id.DrawRectangle(RoiRectangleColor, roiF);
            }
        }

        private void DrawDebugInfo(Graphics g) {
            string info =
$@"== Image ==
{(imgBuf == IntPtr.Zero ? "X" : $"{imgBw}*{imgBh}*{imgBytepp*8}bpp{(isImgbufFloat ? "(float)" : "")}")}

== Draw ==
UseDrawPixelValue : {(UseDrawPixelValue ? "O" : "X")}
UseDrawCenterLine : {(UseDrawCenterLine ? "O" : "X")}
UseDrawCursorInfo : {(UseDrawCursorInfo ? "O" : "X")}
UseDrawDebugInfo : {(UseDrawDebugInfo ? "O" : "X")}

== Time ==
CopyImageBufferZoom : {t_01:0.0}ms
DrawPixelValue : {t_12:0.0}ms
DrawCenterLine : {t_23:0.0}ms
OnPaintBackBuffer : {t_34:0.0}ms
DrawImage : {t_45:0.0}ms
OnPaint : {t_56:0.0}ms
DrawCursorInfo : {t_67:0.0}ms
DrawDebugInfo : {t_78:0.0}ms
Total : {t_total:0.0}ms
";
            g.FillRectangle(Brushes.White, this.Width - 200, 0, 200, 400);
            g.DrawString(info, Font, Brushes.Black, this.Width - 200, 0);
        }

        // 픽셀값 표시
        private void DrawPixelValue(ImageDrawing id) {
            if (imgBuf == IntPtr.Zero)
                return;
            var zoom = GetZoomFactor();
            if (zoom < 16 || (imgBytepp != 1 && zoom < 32))
                return;

            IFont font;
            bool multiLine = false;
            if (imgBytepp == 1) {
                if (zoom <= 17) font = Fonts.Ascii_05x08;
                else if (zoom <= 25) font = Fonts.Ascii_06x13;
                else if (zoom <= 33) font = Fonts.Ascii_08x16;
                else font = Fonts.Ascii_10x18;
            } else {
                multiLine = true;
                if (zoom <= 33) font = Fonts.Ascii_05x08;
                else if (zoom <= 49) font = Fonts.Ascii_06x13;
                else if (zoom <= 65) font = Fonts.Ascii_08x16;
                else font = Fonts.Ascii_10x18;
            }

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
                    string pixelValueText = GetImagePixelValueText(ix, iy, multiLine);
                    int colIdx = GetImagePixelValueColorIndex(ix, iy);
                    id.DrawString(pixelValueText, font, pseudoColor[colIdx], ix - 0.5f, iy - 0.5f);
                }
            }
        }

        // 픽셀값 표시 문자열
        private unsafe string GetImagePixelValueText(int ix, int iy, bool multiLine = false) {
            if (imgBuf == IntPtr.Zero || ix < 0 || ix >= imgBw || iy < 0 || iy >= imgBh)
                return string.Empty;
            var ptr = (byte*)imgBuf + ((Int64)imgBw * iy + ix) * imgBytepp;
            if (imgBytepp == 1) {
                return (*ptr).ToString();
            } else {
                if (isImgbufFloat) {
                    if (imgBytepp == 4)
                        return string.Format(FloatValueFormat,*(float*)ptr);
                    else
                        return string.Format(FloatValueFormat, *(double*)ptr);
                } else {
                    if (multiLine)
                        return string.Format("{0}\n{1}\n{2}", ptr[2], ptr[1], ptr[0]);
                    else
                        return string.Format("{0},{1},{2}", ptr[2], ptr[1], ptr[0]);
                }
            }
        }

        // 픽셀값 표시 컬러 인덱스
        private unsafe int GetImagePixelValueColorIndex(int ix, int iy) {
            if (imgBuf == IntPtr.Zero || ix < 0 || ix >= imgBw || iy < 0 || iy >= imgBh)
                return 0;
            var ptr = (byte*)imgBuf + ((Int64)imgBw * iy + ix) * imgBytepp;
            if (imgBytepp == 1) {
                return (*ptr) / 32;
            } else {
                if (isImgbufFloat) {
                    if (imgBytepp == 4)
                        return Math.Min(Math.Max((int)(*(float*)ptr * 255 / FloatValueMax), 0), 255) / 32;
                    else
                        return Math.Min(Math.Max((int)((*(double*)ptr * 255 / FloatValueMax)), 0), 255) / 32;
                } else {
                    // rgb -> gray
                    return (ptr[2] * 3 + ptr[1] * 6 + ptr[0]) / 320;
                }
            }
        }

        // 픽셀값 표시 컬러
        private static readonly Color[] pseudoColor = {
            Color.White,      // 0~31
            Color.Cyan,       // 32~63
            Color.DodgerBlue, // 63~95
            Color.Yellow,     // 96~127
            Color.Brown,      // 128~159
            Color.DarkViolet, // 160~191
            Color.Red    ,    // 192~223
            Color.Black,      // 224~255
        };

        // 중심선 표시
        private void DrawCenterLine(ImageDrawing id) {
            if (imgBuf == IntPtr.Zero)
                return;
            id.DrawVLineDot(CenterLineColor, -0.5f, imgBh - 0.5f, imgBw / 2.0f - 0.5f);
            id.DrawHLineDot(CenterLineColor, -0.5f, imgBw - 0.5f, imgBh / 2.0f - 0.5f);
        }

        // 커서 정보 표시
        private void DrawCursorInfo(Graphics g, int ofsx, int ofsy) {
            var ptImg = DispToImg(ptMove);
            int ix = (int)Math.Round(ptImg.X);
            int iy = (int)Math.Round(ptImg.Y);
            var colText = GetImagePixelValueText(ix, iy);
            string zoomText = GetZoomText();
            string text = string.Format("zoom={0} ({1},{2})={3}", zoomText, ix, iy, colText);
            g.FillRectangle(Brushes.Black, ofsx, ofsy, 200, Font.Height);
            g.DrawString(text, Font, Brushes.White, ofsx, ofsy);
        }

        // ImageGraphics 리턴
        public ImageGraphics GetImageGraphics(Graphics g) {
            return new ImageGraphics(this, g);
        }

        // ImageDrawing 리턴
        public ImageDrawing GetImageDrawing(IntPtr buf, int bw, int bh) {
            return new ImageDrawing(this, buf, bw, bh);
        }

        // 이미지 버퍼를 디스플레이 버퍼에 복사
        private static unsafe void CopyImageBufferZoom(IntPtr imgBuf, int imgBw, int imgBh, int bytepp, bool bufIsFloat, IntPtr dispBuf, int dispBw, int dispBh, int panx, int pany, double zoom, int bgColor, double floatValueMax) {
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

            float floatScale = (float)(255 / floatValueMax);
            double doubleScale = 255 / floatValueMax;

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
                                int v = (int)(*(float*)sp * floatScale);
                                if (v > 255) v = 255;
                                if (v < 0) v = 0;
                                *dp = v | v << 8 | v << 16 | 0xff << 24;
                            } else if (bytepp == 8) {   // 8byte double gray
                                int v = (int)(*(double*)sp * doubleScale);
                                if (v > 255) v = 255;
                                if (v < 0) v = 0;
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

        public Bitmap GetBitmap() {
            if (imgBuf == null || isImgbufFloat)
                return null;
            Bitmap bmp = ImageUtil.ImageBufferToBitmap(imgBuf, imgBw, imgBh, imgBytepp);
            return bmp;
        }
    }
}
