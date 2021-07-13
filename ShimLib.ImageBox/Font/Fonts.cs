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
        public static IFont tom_thumb { get; } = new BdfFont(Resources.tom_thumb);
        public static IFont spleen_05x08 { get; } = new BdfFont(Resources.spleen_05x08);
        public static IFont spleen_06x12 { get; } = new BdfFont(Resources.spleen_06x12);
        public static IFont spleen_08x16 { get; } = new BdfFont(Resources.spleen_08x16);
        public static IFont spleen_12x24 { get; } = new BdfFont(Resources.spleen_12x24);
        public static IFont spleen_16x32 { get; } = new BdfFont(Resources.spleen_16x32);
        public static IFont spleen_32x64 { get; } = new BdfFont(Resources.spleen_32x64);
        public static IFont uismall { get; }= new BdfFont(Resources.uismall);
        public static IFont unifont_13_0_06_bdf { get; } = new BdfFont(Resources.unifont_13_0_06_bdf);

        public static Dictionary<EFont, IFont> dic { get; }
        static Fonts() {
            var fonts = typeof(Fonts).GetProperties()
                .Where(pi => pi.PropertyType == typeof(IFont))
                .Select(pi => pi.GetValue(null) as IFont);
            var fontEnums = Enum.GetValues(typeof(EFont))
                .OfType<EFont>();
            dic = fontEnums.Zip(fonts, (efont, font) => new { efont, font })
                .ToDictionary(item => item.efont, item => item.font);
        }
    }

    public enum EFont {
        tom_thumb,
        spleen_05x08,
        spleen_06x12,
        spleen_08x16,
        spleen_12x24,
        spleen_16x32,
        spleen_32x64,
        uismall,
        unifont_13_0_06_bdf,
    }
}
