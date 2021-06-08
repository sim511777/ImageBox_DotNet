using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ShimLib {
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct BITMAPFILEHEADER {
        public ushort bfType;
        public uint bfSize;
        public ushort bfReserved1;
        public ushort bfReserved2;
        public uint bfOffBits;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFOHEADER {
        public uint biSize;
        public int biWidth;
        public int biHeight;
        public ushort biPlanes;
        public ushort biBitCount;
        public uint biCompression;
        public uint biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public uint biClrUsed;
        public uint biClrImportant;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RGBQuad {
        public byte rgbBlue;
        public byte rgbGreen;
        public byte rgbRed;
        public byte rgbReserved;
        public RGBQuad(byte r, byte g, byte b, byte a) {
            rgbBlue = b;
            rgbGreen = g;
            rgbRed = r;
            rgbReserved = a;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BITMAPINFO {
        public BITMAPINFOHEADER bmiHeader;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public RGBQuad[] bmiColors;
    }

    public enum TernaryRasterOperation : uint {
        SRCCOPY             = 0x00CC0020, /* dest = source                   */
        SRCPAINT            = 0x00EE0086, /* dest = source OR dest           */
        SRCAND              = 0x008800C6, /* dest = source AND dest          */
        SRCINVERT           = 0x00660046, /* dest = source XOR dest          */
        SRCERASE            = 0x00440328, /* dest = source AND (NOT dest )   */
        NOTSRCCOPY          = 0x00330008, /* dest = (NOT source)             */
        NOTSRCERASE         = 0x001100A6, /* dest = (NOT src) AND (NOT dest) */
        MERGECOPY           = 0x00C000CA, /* dest = (source AND pattern)     */
        MERGEPAINT          = 0x00BB0226, /* dest = (NOT source) OR dest     */
        PATCOPY             = 0x00F00021, /* dest = pattern                  */
        PATPAINT            = 0x00FB0A09, /* dest = DPSnoo                   */
        PATINVERT           = 0x005A0049, /* dest = pattern XOR dest         */
        DSTINVERT           = 0x00550009, /* dest = (NOT dest)               */
        BLACKNESS           = 0x00000042, /* dest = BLACK                    */
        WHITENESS           = 0x00FF0062, /* dest = WHITE                    */
        NOMIRRORBITMAP      = 0x80000000, /* Do not Mirror the bitmap in this call */
        CAPTUREBLT          = 0x40000000, /* Include layered windows */
    }

    public class Win32Api {
        [DllImport("user32.dll")] public static extern IntPtr GetDC(IntPtr ptr);
        [DllImport("user32.dll")] public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);
        [DllImport("gdi32.dll")] public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")] public static extern IntPtr DeleteDC(IntPtr hDc);
        [DllImport("gdi32.dll")] public static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO bmi, uint Usage, out IntPtr bits, IntPtr hSection, uint dwOffset);
        [DllImport("gdi32.dll")] public static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
        [DllImport("gdi32.dll")] public static extern IntPtr DeleteObject(IntPtr hDc);
        [DllImport("gdi32.dll")] public static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, TernaryRasterOperation RasterOp);
    }
}
