using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ShimLib.Properties;

namespace ShimLib {
    public class Fonts {
        public static IFont bitocra7 = new BdfFont(Resources.bitocra7);
        public static IFont spleen_05x08 = new BdfFont(Resources.spleen_05x08);
        public static IFont spleen_06x12 = new BdfFont(Resources.spleen_06x12);
        public static IFont spleen_08x16 = new BdfFont(Resources.spleen_08x16);
        public static IFont spleen_12x24 = new BdfFont(Resources.spleen_12x24);
        public static IFont spleen_16x32 = new BdfFont(Resources.spleen_16x32);
        public static IFont spleen_32x64 = new BdfFont(Resources.spleen_32x64);
        public static IFont uismall = new BdfFont(Resources.uismall);
        public static IFont unifont_13_0_06_bdf = new BdfFont(Resources.unifont_13_0_06_bdf);
        public static IFont unifont_13_0_06 = new PcfFont(Resources.unifont_13_0_06);

        public readonly static Dictionary<EFont, IFont> dic;
        static Fonts() {
            var fonts = typeof(Fonts).GetFields()
                .Where(fi => fi.FieldType == typeof(IFont))
                .Select(fi => fi.GetValue(null) as IFont);
            var fontEnums = Enum.GetValues(typeof(EFont))
                .OfType<EFont>();
            dic = fontEnums.Zip(fonts, (efont, font) => new { efont, font })
                .ToDictionary(item => item.efont, item => item.font);
        }
    }

    public enum EFont {
        bitocra7,
        spleen_05x08,
        spleen_06x12,
        spleen_08x16,
        spleen_12x24,
        spleen_16x32,
        spleen_32x64,
        uismall,
        unifont_13_0_06_bdf,
        unifont_13_0_06,
    }
}
