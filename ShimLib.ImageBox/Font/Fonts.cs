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
        public static IFont Mame_UnSmall = new BdfFont(Resources.uismall);
        public static IFont Ascii_04x06 = new BmpFont(Resources.Raster_04x06, 04, 06, 0, 0, 32);
        public static IFont Ascii_04x06_2 = new BmpFont(Resources.Raster_04x06_2, 04, 06, 0, 0, 32);
        public static IFont Ascii_05x08 = new BmpFont(Resources.Raster_05x08, 05, 08, 0, 0, 32);
        public static IFont Ascii_05x12 = new BmpFont(Resources.Raster_05x12, 05, 12, 0, 0, 32);
        public static IFont Ascii_06x08 = new BmpFont(Resources.Raster_06x08, 06, 08, 0, 0, 32);
        public static IFont Ascii_06x13 = new BmpFont(Resources.Raster_06x13, 06, 13, 0, 0, 32);
        public static IFont Ascii_07x12 = new BmpFont(Resources.Raster_07x12, 07, 12, 0, 0, 32);
        public static IFont Ascii_08x08 = new BmpFont(Resources.Raster_08x08, 08, 08, 0, 0, 32);
        public static IFont Ascii_08x12 = new BmpFont(Resources.Raster_08x12, 08, 12, 0, 0, 32);
        public static IFont Ascii_08x16 = new BmpFont(Resources.Raster_08x16, 08, 16, 0, 0, 32);
        public static IFont Ascii_08x18 = new BmpFont(Resources.Raster_08x18, 08, 18, 0, 0, 32);
        public static IFont Ascii_10x18 = new BmpFont(Resources.Raster_10x18, 10, 18, 0, 0, 32);
        public static IFont Ascii_10x20 = new BmpFont(Resources.Raster_10x20, 10, 20, 0, 0, 32);
        public static IFont Ascii_10x22 = new BmpFont(Resources.Raster_10x22, 10, 22, 0, 0, 32);
        public static IFont Ascii_12x16 = new BmpFont(Resources.Raster_12x16, 12, 16, 0, 0, 32);
        public static IFont Ascii_12x27 = new BmpFont(Resources.Raster_12x27, 12, 27, 0, 0, 32);
        public static IFont Ascii_16x08 = new BmpFont(Resources.Raster_16x08, 16, 08, 0, 0, 32);
        public static IFont Ascii_16x12 = new BmpFont(Resources.Raster_16x12, 16, 12, 0, 0, 32);
        public static IFont Unifont_13_0_06_bdf = new BdfFont(Resources.unifont_13_0_06_bdf);

        // Fonts.dic[EFont.Ascii_16x12];
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
        Mame_UnSmall,
        Ascii_04x06,
        Ascii_04x06_2,
        Ascii_05x08,
        Ascii_05x12,
        Ascii_06x08,
        Ascii_06x13,
        Ascii_07x12,
        Ascii_08x08,
        Ascii_08x12,
        Ascii_08x16,
        Ascii_08x18,
        Ascii_10x18,
        Ascii_10x20,
        Ascii_10x22,
        Ascii_12x16,
        Ascii_12x27,
        Ascii_16x08,
        Ascii_16x12,
        Unifont_13_0_06_bdf,
    }
}
