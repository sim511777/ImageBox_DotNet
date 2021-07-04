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
        public static IFont lemon = new BdfFont(Resources.lemon);
        public static IFont ter_u12n = new BdfFont(Resources.ter_u12n);
        public static IFont ter_u12b = new BdfFont(Resources.ter_u12b);
        public static IFont ter_u14n = new BdfFont(Resources.ter_u14n);
        public static IFont ter_u14b = new BdfFont(Resources.ter_u14b);
        public static IFont ter_u16n = new BdfFont(Resources.ter_u16n);
        public static IFont ter_u16b = new BdfFont(Resources.ter_u16b);
        public static IFont ter_u18n = new BdfFont(Resources.ter_u18n);
        public static IFont ter_u18b = new BdfFont(Resources.ter_u18b);
        public static IFont ter_u20n = new BdfFont(Resources.ter_u20n);
        public static IFont ter_u20b = new BdfFont(Resources.ter_u20b);
        public static IFont ter_u22n = new BdfFont(Resources.ter_u22n);
        public static IFont ter_u22b = new BdfFont(Resources.ter_u22b);
        public static IFont ter_u24n = new BdfFont(Resources.ter_u24n);
        public static IFont ter_u24b = new BdfFont(Resources.ter_u24b);
        public static IFont ter_u28n = new BdfFont(Resources.ter_u28n);
        public static IFont ter_u28b = new BdfFont(Resources.ter_u28b);
        public static IFont ter_u32n = new BdfFont(Resources.ter_u32n);
        public static IFont ter_u32b = new BdfFont(Resources.ter_u32b);
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
        lemon,
        ter_u12n,
        ter_u12b,
        ter_u14n,
        ter_u14b,
        ter_u16n,
        ter_u16b,
        ter_u18n,
        ter_u18b,
        ter_u20n,
        ter_u20b,
        ter_u22n,
        ter_u22b,
        ter_u24n,
        ter_u24b,
        ter_u28n,
        ter_u28b,
        ter_u32n,
        ter_u32b,
        uismall,
        unifont_13_0_06_bdf,
        unifont_13_0_06,
    }
}
