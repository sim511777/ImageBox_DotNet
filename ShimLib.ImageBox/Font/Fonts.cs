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
        public static IFont Ascii_04x06 = new BmpFont(Resource.Raster_04x06, 04, 06, 0, 0, 32);
        public static IFont Ascii_05x08 = new BmpFont(Resource.Raster_05x08, 05, 08, 0, 0, 32);
        public static IFont Ascii_05x12 = new BmpFont(Resource.Raster_05x12, 05, 12, 0, 0, 32);
        public static IFont Ascii_06x08 = new BmpFont(Resource.Raster_06x08, 06, 08, 0, 0, 32);
        public static IFont Ascii_06x13 = new BmpFont(Resource.Raster_06x13, 06, 13, 0, 0, 32);
        public static IFont Ascii_07x12 = new BmpFont(Resource.Raster_07x12, 07, 12, 0, 0, 32);
        public static IFont Ascii_08x08 = new BmpFont(Resource.Raster_08x08, 08, 08, 0, 0, 32);
        public static IFont Ascii_08x12 = new BmpFont(Resource.Raster_08x12, 08, 12, 0, 0, 32);
        public static IFont Ascii_08x16 = new BmpFont(Resource.Raster_08x16, 08, 16, 0, 0, 32);
        public static IFont Ascii_08x18 = new BmpFont(Resource.Raster_08x18, 08, 18, 0, 0, 32);
        public static IFont Ascii_10x18 = new BmpFont(Resource.Raster_10x18, 10, 18, 0, 0, 32);
        public static IFont Ascii_10x20 = new BmpFont(Resource.Raster_10x20, 10, 20, 0, 0, 32);
        public static IFont Ascii_10x22 = new BmpFont(Resource.Raster_10x22, 10, 22, 0, 0, 32);
        public static IFont Ascii_12x16 = new BmpFont(Resource.Raster_12x16, 12, 16, 0, 0, 32);
        public static IFont Ascii_12x27 = new BmpFont(Resource.Raster_12x27, 12, 27, 0, 0, 32);
        public static IFont Ascii_16x08 = new BmpFont(Resource.Raster_16x08, 16, 08, 0, 0, 32);
        public static IFont Ascii_16x12 = new BmpFont(Resource.Raster_16x12, 16, 12, 0, 0, 32);
        public static IFont Unicode_16x16_hex = new HexFont(Resource.unifont_13_0_06_hex);
    }
}
