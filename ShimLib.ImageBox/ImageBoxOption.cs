using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimLib {
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("ImageBox")]
    public class ImageBoxOption : ICloneable {
        private bool _useDrawPixelValue = true;
        private bool _useDrawCenterLine = true;
        private bool _useDrawCursorInfo = true;
        private bool _useDrawDebugInfo = false;
        private bool _useDrawRoiRectangles = true;
        private bool _useParallelToDraw = true;
        private Color _centerLineColor = Color.Yellow;
        private Color _roiRectangleColor = Color.Blue;
        private double _floatValueMax = 1.0;
        private int _floatValueDigit = 3;
        private EFont _infoFont = EFont.Unicode_16x16_hex;

        // 화면 표시 옵션
        public bool UseDrawPixelValue { get => _useDrawPixelValue; set => _useDrawPixelValue = value; }
        public bool UseDrawCenterLine { get => _useDrawCenterLine; set => _useDrawCenterLine = value; }
        public bool UseDrawCursorInfo { get => _useDrawCursorInfo; set => _useDrawCursorInfo = value; }
        public bool UseDrawDebugInfo { get => _useDrawDebugInfo; set => _useDrawDebugInfo = value; }
        public bool UseDrawRoiRectangles { get => _useDrawRoiRectangles; set => _useDrawRoiRectangles = value; }
        public bool UseParallelToDraw { get => _useParallelToDraw; set => _useParallelToDraw = value; }
        public Color CenterLineColor { get => _centerLineColor; set => _centerLineColor = value; }
        public Color RoiRectangleColor { get => _roiRectangleColor; set => _roiRectangleColor = value; }
        public double FloatValueMax { get => _floatValueMax; set => _floatValueMax = value; }
        public int FloatValueDigit { get => _floatValueDigit; set => _floatValueDigit = value; }
        public EFont InfoFont { get => _infoFont; set => _infoFont = value; }

        public object Clone() {
            return MemberwiseClone();
        }

        public ImageBoxOption Copy() {
            return Clone() as ImageBoxOption;
        }
    }
}
