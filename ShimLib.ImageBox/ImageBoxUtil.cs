using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShimLib {
    public interface IPixelDrawable {
        void SetFloatValueMax(double floatValueMax);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe void SetPixel(byte* sp, int* dp);
    }

    public class ImageBoxUtil {
        // 디스플레이 버퍼 클리어
        public static unsafe void Clear(IntPtr dispBuf, int dispBw, int dispBh, int bgColor, bool useParallel) {
            Action<int> LineAction = (y) => {
                int* dp = (int*)dispBuf + (Int64)dispBw * y;
                Util.Memset4((IntPtr)dp, bgColor, dispBw);
            };
            if (useParallel)
                Parallel.For(0, dispBh, LineAction);
            else
                for (int y = 0; y < dispBh; y++) { LineAction(y); }
        }

        // 이미지 버퍼를 디스플레이 버퍼에 복사
        public static unsafe void DrawImageBufferZoom(IntPtr imgBuf, int imgBw, int imgBh, int bytepp, bool bufIsFloat, IntPtr dispBuf, int dispBw, int dispBh, int panx, int pany, double zoom, int bgColor, double floatValueMax, IPixelDrawable pixelDrawer, bool useParallel) {
            // 인덱스 버퍼 생성
            int[] siys = new int[dispBh];
            int[] sixs = new int[dispBw];
            for (int y = 0; y < dispBh; y++) {
                int siy = (int)Math.Floor((y - pany) / zoom);
                siys[y] = (siy < 0 || siy >= imgBh) ? -1 : siy;
            }
            for (int x = 0; x < dispBw; x++) {
                int six = (int)Math.Floor((x - panx) / zoom);
                sixs[x] = (six < 0 || six >= imgBw) ? -1 : six;
            }
            int x1Include = sixs.ToList().FindIndex(six => six != -1);
            int x2Exclude = sixs.ToList().FindLastIndex(six => six != -1) + 1;

            if (pixelDrawer == null) {
                pixelDrawer = new PixelDrawerNone();
            }

            Action<int> LineAction = (y) => {
                int* dp = (int*)dispBuf + (Int64)dispBw * y;
                int siy = siys[y];
                if (siy == -1 || x1Include == -1) {
                    Util.Memset4((IntPtr)dp, bgColor, dispBw);
                    return;
                }
                if (x1Include > 0) {
                    Util.Memset4((IntPtr)dp, bgColor, x1Include);
                }

                byte* sptr = (byte*)imgBuf + (Int64)imgBw * siy * bytepp;
                for (int x = x1Include; x < x2Exclude; x++, dp++) {
                    int six = sixs[x];
                    byte* sp = &sptr[six * bytepp];

                    pixelDrawer.SetPixel(sp, dp);
                }

                if (x2Exclude < dispBw) {
                    Util.Memset4((IntPtr)dp, bgColor, dispBw - x2Exclude);
                }
            };

            if (useParallel)
                Parallel.For(0, dispBh, LineAction);
            else
                for (int y = 0; y < dispBh; y++) { LineAction(y); }
        }

        // 이미지 좌표 -> 화면 좌료
        public static Point ImgToDisp(PointF ptImg, double zoomFactor, Point ptPan) {
            int dispX = (int)Math.Floor((ptImg.X + 0.5) * zoomFactor + ptPan.X);
            int dispY = (int)Math.Floor((ptImg.Y + 0.5) * zoomFactor + ptPan.Y);
            return new Point(dispX, dispY);
        }

        // 이미지 좌표 -> 화면 좌료
        public static Rectangle ImgToDisp(RectangleF rectImg, double zoomFactor, Point ptPan) {
            int dispX = (int)Math.Floor((rectImg.X + 0.5) * zoomFactor + ptPan.X);
            int dispY = (int)Math.Floor((rectImg.Y + 0.5) * zoomFactor + ptPan.Y);
            int dispWidth = (int)Math.Floor(rectImg.Width * zoomFactor);
            int dispHeight = (int)Math.Floor(rectImg.Height * zoomFactor);
            return new Rectangle(dispX, dispY, dispWidth, dispHeight);
        }

        // 화면 좌표 -> 이미지 좌표
        public static PointF DispToImg(Point ptDisp, double zoomFactor, Point ptPan) {
            float imgX = (float)((ptDisp.X - ptPan.X) / zoomFactor - 0.5);
            float imgY = (float)((ptDisp.Y - ptPan.Y) / zoomFactor - 0.5);
            return new PointF(imgX, imgY);
        }
    }

    public class PixelDrawerNone : IPixelDrawable {
        public void SetFloatValueMax(double floatValueMax) { }
        private int color = Color.Blue.ToArgb();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SetPixel(byte* sp, int* dp) {
            *dp = color;
        }
    }

    public class PixelDrawerFloat4 : IPixelDrawable {
        private float floatScale = 1.0f;
        public void SetFloatValueMax(double floatValueMax) => floatScale = (float)(255 / floatValueMax);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SetPixel(byte* sp, int* dp) {
            int v = (int)(*(float*)sp * floatScale);
            if (v > 255) v = 255;
            if (v < 0) v = 0;
            *dp = v | v << 8 | v << 16 | 0xff << 24;
        }
    }

    public class PixelDrawerFloat8 : IPixelDrawable {
        private double doubleScale = 1.0;
        public void SetFloatValueMax(double floatValueMax) => doubleScale = 255 / floatValueMax;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SetPixel(byte* sp, int* dp) {
            int v = (int)(*(double*)sp * doubleScale);
            if (v > 255) v = 255;
            if (v < 0) v = 0;
            *dp = v | v << 8 | v << 16 | 0xff << 24;
        }
    }

    public class PixelDrawerByte1 : IPixelDrawable {
        public void SetFloatValueMax(double floatValueMax) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SetPixel(byte* sp, int* dp) {
            int v = sp[0];
            *dp = v | v << 8 | v << 16 | 0xff << 24;
        }
    }

    public class PixelDrawerByte2BE : IPixelDrawable {
        public void SetFloatValueMax(double floatValueMax) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SetPixel(byte* sp, int* dp) {
            int v = sp[0];
            *dp = v | v << 8 | v << 16 | 0xff << 24;
        }
    }

    public class PixelDrawerByte2LE : IPixelDrawable {
        public void SetFloatValueMax(double floatValueMax) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SetPixel(byte* sp, int* dp) {
            int v = sp[1];
            *dp = v | v << 8 | v << 16 | 0xff << 24;
        }
    }

    public class PixelDrawerByte3 : IPixelDrawable {
        public void SetFloatValueMax(double floatValueMax) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SetPixel(byte* sp, int* dp) {
            int v = sp[0];
            *dp = sp[0] | sp[1] << 8 | sp[2] << 16 | 0xff << 24;
        }
    }

    public class PixelDrawerByte4 : IPixelDrawable {
        public void SetFloatValueMax(double floatValueMax) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SetPixel(byte* sp, int* dp) {
            int v = sp[0];
            *dp = sp[0] | sp[1] << 8 | sp[2] << 16 | 0xff << 24;
        }
    }
}
