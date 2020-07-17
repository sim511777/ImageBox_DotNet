using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageBox {
    public partial class ImageBox : Control {
        public ImageBox() {
            InitializeComponent();
            DoubleBuffered = true;  // 자동더블버퍼링 활성화 : OnPaint함수의 PainintEventArgs.Graphics
            PixelValueDrawFont = new Font("Arial", 6);
        }

        // 픽셀값 표시 폰트
        public Font PixelValueDrawFont { get; set; }

        // 패닝
        private int panX;
        private int panY;
        
        // 줌레벨
        private int zoomLevel;
        
        private int PanX {
            set { panX = value; }
            get { return panX; }
        }

        private int PanY {
            set { panY = value; }
            get { return panY; }
        }

        // 이미지 버퍼 설정
        public void SetImageBuffer(IntPtr buf, int bw, int bh, int bytepp) {
        }

        // 이미지 좌표를 표시 좌표로 변경
        public PointF DispToImg(Point ptDisp) {
            throw new NotImplementedException();
            PointF ptImg = PointF.Empty;
            return ptImg;
        }

        // 표시 좌표를 이미지 좌표로 변경
        public Point ImgToDisp(PointF ptImg) {
            throw new NotImplementedException();
            Point ptDisp = Point.Empty;
            return ptDisp;
        }

        // 배경컬러, 배경이미지 그려지지 않기 위해 재정의
        protected override void OnPaintBackground(PaintEventArgs pevent) { }

        // 화면 표시
        protected override void OnPaint(PaintEventArgs pe) {
            if (DesignMode) {
                // 폼디자인시에 표시되는 항목
                pe.Graphics.Clear(BackColor);
                pe.Graphics.DrawString(Name, Font, Brushes.Black, 0, 0);
                return;
            }

            DrawImageBuffer();  // 이미지 버퍼 표시
            DrawPixelValue();   // 픽셀값 표시
            DrawCenterLine();   // 중앙선 표시
            DrawCursorInfo();   // 마우스 커서 위치의 픽셀 정보 표시
            DrawDebugInfo();    // 디버그 정보 표시

            base.OnPaint(pe);   // 여기서 Paint이벤트가 호출됨
        }

        // 이미지 버퍼 표시
        private void DrawImageBuffer() {
            throw new NotImplementedException();
        }

        // 픽셀값 표시
        private void DrawPixelValue() {
            throw new NotImplementedException();
        }

        // 중앙선 표시
        private void DrawCenterLine() {
            throw new NotImplementedException();
        }

        // 마우스 커서 위치 픽셀 정보 표시
        private void DrawCursorInfo() {
            throw new NotImplementedException();
        }

        // 디버그 정보 표시
        private void DrawDebugInfo() {
            throw new NotImplementedException();
        }
    }
}
