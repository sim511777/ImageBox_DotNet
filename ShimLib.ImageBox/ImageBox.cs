﻿using System;
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
using System.Threading.Tasks;
using System.Drawing.Text;

namespace ShimLib {
    public delegate void PaintBackbufferEventHandler(object sender, IntPtr buf, int bw, int bh);

    [ToolboxBitmap(typeof(ImageBox), "ImageBox.bmp")]
    public partial class ImageBox : Control {
        // 옵션
        public ImageBoxOption Option { get; set; } = new ImageBoxOption();
        
        // ROI 리스트
        [Browsable(false)]
        public List<Rectangle> RoiList { get; } = new List<Rectangle>();

        // float 표시 자리수
        private string GetFloatValueFormat() {
            return $"{{0:.{new string('0', Math.Max(Option.FloatValueDigit, 0))}}}";
        }

        // 생성자
        public ImageBox() {
            InitializeComponent();
            SetStyle(ControlStyles.Opaque, true);
        }

        // 백버퍼 그리기 이벤트
        [Category("ImageBox")] public event PaintBackbufferEventHandler PaintBackBuffer;
        protected virtual void OnPaintBackBuffer(IntPtr buf, int bw, int bh) {
            PaintBackBuffer?.Invoke(this, buf, bw, bh);
        }

        // 이미지 버퍼 정보
        private IntPtr imgBuf = IntPtr.Zero;
        private int imgBw = 0;
        private int imgBh = 0;
        private int imgBytepp = 0;
        private bool isImgbufFloat = false;
        private LineDrawAction lineDrawAction = null;

        // 디스플레이 버퍼
        private Bitmap dispBmp = null;
        private BufferedGraphics bfg = null;

        // 패닝 옵셋(픽셀)
        private Point ptPan = new Point(2, 2);
        [Browsable(false)]
        public Point PtPan {
            get { return ptPan; }
            set {
                if (imgBuf == IntPtr.Zero)
                    ptPan = new Point(2, 2);
                else {
                    var zoom = GetZoomFactor();
                    int x = Util.Clamp(value.X, -(int)(imgBw * zoom) + 2, 2);
                    int y = Util.Clamp(value.Y, -(int)(imgBh * zoom) + 2, 2);
                    ptPan = new Point(x, y);
                }
            } 
        }
        // 줌 레벨(0 = 1x)
        private int zoomLevel = 0;
        public int ZoomLevel {
            get { return zoomLevel; }
            set { zoomLevel = Util.Clamp(value, -16, 16); }
        }

        private void GetZoomFactorComponents(out int exp_num, out int c) {
            exp_num = (ZoomLevel >= 0) ? ZoomLevel / 2 : (ZoomLevel - 1) / 2;
            if (ZoomLevel % 2 != 0)
                exp_num--;
            c = (ZoomLevel % 2 != 0) ? 3 : 1;
        }

        public double GetZoomFactor() {
            GetZoomFactorComponents(out int exp_num, out int c);
            return c * Math.Pow(2, exp_num);
        }

        private string GetZoomText() {
            GetZoomFactorComponents(out int exp_num, out int c);
            return (exp_num >= 0) ? (c * (int)Math.Pow(2, exp_num)).ToString() : c.ToString() + "/" + ((int)Math.Pow(2, -exp_num)).ToString();
        }

        // 이미지 버퍼 설정
        public unsafe void SetImageBuffer(IntPtr _buf, int _bw, int _bh, int _bytepp, bool _isFloat) {
            imgBuf = _buf;
            imgBw = _bw;
            imgBh = _bh;
            imgBytepp = _bytepp;
            isImgbufFloat = _isFloat;

            lineDrawAction = null;
            if (isImgbufFloat) {
                if (imgBytepp == 4) {          // 4byte float gray
                    lineDrawAction = ImageBoxUtil.LineDrawActionFloat4;
                } else if (imgBytepp == 8) {   // 8byte double gray
                    lineDrawAction = ImageBoxUtil.LineDrawActionFloat8;
                }
            } else {
                if (imgBytepp == 1) {          // 1byte gray
                    lineDrawAction = ImageBoxUtil.LineDrawActionByte1;
                } else if (imgBytepp == 2) {   // 2byte gray (*.hra)
                    lineDrawAction = ImageBoxUtil.LineDrawActionByte2;
                } else if (imgBytepp == 3) {   // 3byte bgr
                    lineDrawAction = ImageBoxUtil.LineDrawActionByte3;
                } else if (imgBytepp == 4) {   // rbyte bgra
                    lineDrawAction = ImageBoxUtil.LineDrawActionByte4;
                }
            }
        }

        public unsafe void SetImageBufferCustomDisp(IntPtr _buf, int _bw, int _bh, int _bytepp, LineDrawAction _lineDrawAction) {
            imgBuf = _buf;
            imgBw = _bw;
            imgBh = _bh;
            imgBytepp = _bytepp;

            lineDrawAction = _lineDrawAction;
        }

        // 줌 리셋
        public void ResetZoom() {
            PtPan = new Point(2, 2);
            ZoomLevel = 0;
        }

        // 이미지 좌표 -> 화면 좌료
        public Point ImgToDisp(PointF ptImg) {
            return ImageBoxUtil.ImgToDisp(ptImg, GetZoomFactor(), PtPan);
        }

        // 이미지 좌표 -> 화면 좌료
        public Rectangle ImgToDisp(RectangleF rectImg) {
            return ImageBoxUtil.ImgToDisp(rectImg, GetZoomFactor(), PtPan);
        }

        // 화면 좌표 -> 이미지 좌표
        public PointF DispToImg(Point ptDisp) {
            return ImageBoxUtil.DispToImg(ptDisp, GetZoomFactor(), PtPan);
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

            if (bfg != null)
                bfg.Dispose();
            bfg = BufferedGraphicsManager.Current.Allocate(this.CreateGraphics(), ClientRectangle);
            bfg.Graphics.CompositingMode = CompositingMode.SourceCopy;
            bfg.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            bfg.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            bfg.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            bfg.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
            bfg.Graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;

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
                if (Option.UseDrawCursorInfo) {
                    using (Bitmap bmp = new Bitmap(8 * 35, Fonts.dic[Option.InfoFont].FontHeight, PixelFormat.Format32bppPArgb)) {
                        var bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                        DrawCursorInfo(new ImageDrawing(bd.Scan0, bd.Width, bd.Height), 0, 0);
                        bmp.UnlockBits(bd);
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
        
        List<Tuple<string, double>> dtList = new List<Tuple<string, double>>();
        public new void Invalidate() {
            List<Tuple<string, double>> tList = new List<Tuple<string, double>>();
            tList.Add(Tuple.Create("Start", Util.GetTimeMs()));

            double zoom = GetZoomFactor();
            var bmpData = dispBmp.LockBits(new Rectangle(Point.Empty, dispBmp.Size), ImageLockMode.WriteOnly, dispBmp.PixelFormat);
            IntPtr dispBuf = bmpData.Scan0;
            int dispBw = bmpData.Width;
            int dispBh = bmpData.Height;
            var id = new ImageDrawing(dispBuf, dispBw, dispBh, zoom, PtPan);
            var idWnd = new ImageDrawing(dispBuf, dispBw, dispBh);

            // 이미지 확대 축소
            if (imgBuf == IntPtr.Zero) {
                ImageBoxUtil.Clear(dispBuf, dispBw, dispBh, BackColor.ToArgb(), Option.UseParallelToDraw);
            } else {
                ImageBoxUtil.DrawImageBufferZoom(imgBuf, imgBw, imgBh, imgBytepp, isImgbufFloat, dispBuf, dispBw, dispBh, PtPan.X, PtPan.Y, zoom, BackColor.ToArgb(), Option.FloatValueMax, lineDrawAction, Option.UseParallelToDraw);
                if (lineDrawAction == null) {
                    idWnd.DrawString("LineDrawAction not assigned,\nso i can not display image.", Fonts.dic[Option.InfoFont], Color.Yellow, 2, 25);
                }
            }

            tList.Add(Tuple.Create("DrawImageBufferZoom", Util.GetTimeMs()));

            // 픽셀값 표시
            if (Option.UseDrawPixelValue)
                DrawPixelValue(id);
            tList.Add(Tuple.Create("DrawPixelValue", Util.GetTimeMs()));

            // ROI 표시
            if (Option.UseDrawRoiRectangles) {
                DrawRoiRectangles(id);
                if (isRoiDown) {
                    DrawRoiDown(id);
                }
            }
            tList.Add(Tuple.Create("DrawRoiRectangles", Util.GetTimeMs()));
            
            // 중심선 표시
            if (Option.UseDrawCenterLine)
                DrawCenterLine(id);
            tList.Add(Tuple.Create("DrawCenterLine", Util.GetTimeMs()));

            // PaintBackBuffer이벤트 발생
            OnPaintBackBuffer(dispBuf, dispBw, dispBh); // 여기서 사용자가 정의한 Paint이벤트 함수가 호출됨
            tList.Add(Tuple.Create("OnPaintBackBuffer", Util.GetTimeMs()));

            // Paint이벤트 발생
            using (var bmpTemp = new Bitmap(dispBw, dispBh, bmpData.Stride, bmpData.PixelFormat, dispBuf)) {
                var g = Graphics.FromImage(bmpTemp);
                base.OnPaint(new PaintEventArgs(g, ClientRectangle));   // 여기서 사용자가 정의한 Paint이벤트 함수가 호출됨
                g.Dispose();
            }
            tList.Add(Tuple.Create("OnPaint", Util.GetTimeMs()));

            // 커서 정보 표시
            if (Option.UseDrawCursorInfo)
                DrawCursorInfo(idWnd, 2, 2);
            tList.Add(Tuple.Create("DrawCursorInfo", Util.GetTimeMs()));

            // 디비그 정보 표시
            if (Option.UseDrawDebugInfo)
                DrawDebugInfo(idWnd);
            tList.Add(Tuple.Create("DrawDebugInfo", Util.GetTimeMs()));

            dispBmp.UnlockBits(bmpData);
            
            // 백버퍼에다 복사
            bfg.Graphics.DrawImage(dispBmp, 0, 0);
            tList.Add(Tuple.Create("DrawImage", Util.GetTimeMs()));
            
            // 프런트버퍼에다 복사
            bfg.Render();
            tList.Add(Tuple.Create("Render", Util.GetTimeMs()));
            
            // delta 계산 및 total 계산
            var nextList = tList.Skip(1);
            dtList = nextList.Zip(tList, (next, prev) => Tuple.Create(next.Item1, next.Item2 - prev.Item2)).ToList();
            dtList.Add(Tuple.Create("Total", tList.Last().Item2 - tList.First().Item2));
         }

        // 페인트
        protected override void OnPaint(PaintEventArgs pe) {
            // 디자인 모드
            if (DesignMode) {
                pe.Graphics.Clear(BackColor);
                pe.Graphics.DrawString(Name, Font, Brushes.Black, 0, 0);
                return;
            }
            
            // 이미지 그리기
            bfg.Render();
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
            id.DrawString(roiStart, Fonts.Ascii_10x18, Option.RoiRectangleColor, roiF.X - sizeStart.Width / (float)zoom, roiF.Y - sizeStart.Height / (float)zoom, Color.Yellow);
            id.DrawString(roiEnd, Fonts.Ascii_10x18, Option.RoiRectangleColor, roiF.X + roiF.Width, roiF.Y + roiF.Height, Color.Yellow);
            id.DrawRectangleDot(Option.RoiRectangleColor, roiF);
        }

        private void DrawRoiRectangles(ImageDrawing id) {
            foreach (var roi in RoiList) {
                var roiF = new RectangleF(roi.X - 0.5f, roi.Y - 0.5f, roi.Width, roi.Height);
                id.DrawRectangle(Option.RoiRectangleColor, roiF);
            }
        }

        private void DrawDebugInfo(ImageDrawing id) {
            var sb = new StringBuilder();
            string imageInfo = imgBuf == IntPtr.Zero ? "X" : $"{imgBw}*{imgBh}*{imgBytepp*8}bpp{(isImgbufFloat ? "(float)" : "")}";
            sb.AppendLine("== Image ==");
            sb.AppendLine(imageInfo);
            sb.AppendLine();
            sb.AppendLine("== Draw ==");
            sb.AppendLine($"UseDrawPixelValue : {(Option.UseDrawPixelValue ? "O" : "X")}");
            sb.AppendLine($"UseDrawCenterLine : {(Option.UseDrawCenterLine ? "O" : "X")}");
            sb.AppendLine($"UseDrawCursorInfo : {(Option.UseDrawCursorInfo ? "O" : "X")}");
            sb.AppendLine($"UseDrawDebugInfo : {(Option.UseDrawDebugInfo ? "O" : "X")}");
            sb.AppendLine($"UseDrawRoiRectangles : {(Option.UseDrawRoiRectangles ? "O" : "X")}");
            sb.AppendLine();
            sb.AppendLine("== Time ==");
            var dtTextList = dtList.Select(dt => $"{dt.Item1} : {dt.Item2:0.0}ms");
            sb.Append(string.Join("\r\n", dtTextList));
            id.DrawString(sb.ToString(), Fonts.dic[Option.InfoFont], Color.Black, this.Width - 230, 2, Color.White);
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
            ix2 = Math.Min(ix2, imgBw - 1) + 1; // ix end exclusive
            iy2 = Math.Min(iy2, imgBh - 1) + 1; // iy end exclusive
            void iyAction(int iy) {
                for (int ix = ix1; ix < ix2; ix++) {
                    string pixelValueText = GetImagePixelValueText(ix, iy, multiLine);
                    int colIdx = GetImagePixelValueColorIndex(ix, iy);
                    id.DrawString(pixelValueText, font, pseudoColor[colIdx], ix - 0.5f, iy - 0.5f);
                }
            }
            if (Option.UseParallelToDraw)
                Parallel.For(iy1, iy2, iyAction);
            else
                for (int iy = iy1; iy < iy2; iy++) { iyAction(iy); }
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
                        return string.Format(GetFloatValueFormat(), *(float*)ptr);
                    else
                        return string.Format(GetFloatValueFormat(), *(double*)ptr);
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
                        return Util.Clamp((int)(*(float*)ptr * 255 / Option.FloatValueMax), 0, 255) / 32;
                    else
                        return Util.Clamp((int)((*(double*)ptr * 255 / Option.FloatValueMax)), 0, 255) / 32;
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
            id.DrawVLineDot(Option.CenterLineColor, -0.5f, imgBh - 0.5f, imgBw / 2.0f - 0.5f);
            id.DrawHLineDot(Option.CenterLineColor, -0.5f, imgBw - 0.5f, imgBh / 2.0f - 0.5f);
        }

        // 커서 정보 표시
        private void DrawCursorInfo(ImageDrawing id, int ofsx, int ofsy) {
            var ptImg = DispToImg(ptMove);
            int ix = (int)Math.Round(ptImg.X);
            int iy = (int)Math.Round(ptImg.Y);
            var colText = GetImagePixelValueText(ix, iy);
            string zoomText = GetZoomText();
            string text = ($"zoom={zoomText} ({ix},{iy})={colText}").PadRight(35);
            var size = Fonts.dic[Option.InfoFont].MeasureString(text);
            id.DrawString(text, Fonts.dic[Option.InfoFont], Color.White, ofsx, ofsy, Color.Black);
        }

        // ImageGraphics 리턴
        public ImageGraphics GetImageGraphics(Graphics g) {
            return new ImageGraphics(g, GetZoomFactor(), PtPan);
        }

        // ImageDrawing 리턴
        public ImageDrawing GetImageDrawing(IntPtr buf, int bw, int bh) {
            return new ImageDrawing(buf, bw, bh, GetZoomFactor(), PtPan);
        }

        public Bitmap GetBitmap() {
            if (imgBuf == null)
                return null;
            Bitmap bmp = ImageUtil.ImageBufferToBitmap(imgBuf, imgBw, imgBh, imgBytepp, isImgbufFloat, Option.FloatValueMax);
            return bmp;
        }
    }
}
