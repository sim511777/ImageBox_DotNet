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
        // 화면 표시 옵션
        public bool UseDrawPixelValue { get; set; } = true;
        public bool UseDrawCenterLine { get; set; } = true;
        public bool UseDrawCursorInfo { get; set; } = true;
        public bool UseDrawDebugInfo { get; set; } = false;
        public bool UseDrawRoiRectangles { get; set; } = true;
        public bool UseParallelToDraw { get; set; } = true;
        public Color CenterLineColor { get; set; } = Color.Yellow;
        public Color RoiRectangleColor { get; set; } = Color.Blue;
        public double FloatValueMax { get; set; } = 1.0;
        public int FloatValueDigit { get; set; } = 3;
        public EFont InfoFont { get; set; } = EFont.Unifont_13_0_06_bdf;

        public object Clone() {
            return MemberwiseClone();
        }

        public ImageBoxOption Copy() {
            return Clone() as ImageBoxOption;
        }
    }
}
