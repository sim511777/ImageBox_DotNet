using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimLib {
    public unsafe delegate void LineDrawAction(int x1Include, int x2Exclude, int[] sixs, int bytepp, byte* sptr, int* dp, double floatValueMax);
    
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
        public static unsafe void DrawImageBufferZoom(IntPtr imgBuf, int imgBw, int imgBh, int bytepp, bool bufIsFloat, IntPtr dispBuf, int dispBw, int dispBh, int panx, int pany, double zoom, int bgColor, double floatValueMax, LineDrawAction lineDrawAction, bool useParallel) {
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

            if (lineDrawAction == null) {
                lineDrawAction = LineDrawActionNone;
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
                lineDrawAction(x1Include, x2Exclude, sixs, bytepp, sptr, dp + x1Include, floatValueMax);

                if (x2Exclude < dispBw) {
                    Util.Memset4((IntPtr)(dp + x2Exclude), bgColor, dispBw - x2Exclude);
                }
            };

            if (useParallel)
                Parallel.For(0, dispBh, LineAction);
            else
                for (int y = 0; y < dispBh; y++) { LineAction(y); }
        }

        public static unsafe void LineDrawActionNone(int x1Include, int x2Exclude, int[] sixs, int bytepp, byte* sptr, int* dp, double floatValueMax) {
            int color = Color.Blue.ToArgb();
            for (int x = x1Include; x < x2Exclude; x++, dp++) {
                *dp = color;
            }
        }

        public static unsafe void LineDrawActionFloat4(int x1Include, int x2Exclude, int[] sixs, int bytepp, byte* sptr, int* dp, double floatValueMax) {
            float floatScale = (float)(255 / floatValueMax);

            for (int x = x1Include; x < x2Exclude; x++, dp++) {
                int six = sixs[x];
                byte* sp = &sptr[six * bytepp];

                int v = (int)(*(float*)sp * floatScale);
                if (v > 255) v = 255;
                if (v < 0) v = 0;
                *dp = v | v << 8 | v << 16 | 0xff << 24;
            }
        }

        public static unsafe void LineDrawActionFloat8(int x1Include, int x2Exclude, int[] sixs, int bytepp, byte* sptr, int* dp, double floatValueMax) {
            double doubleScale = 255 / floatValueMax;
            
            for (int x = x1Include; x < x2Exclude; x++, dp++) {
                int six = sixs[x];
                byte* sp = &sptr[six * bytepp];

                int v = (int)(*(double*)sp * doubleScale);
                if (v > 255) v = 255;
                if (v < 0) v = 0;
                *dp = v | v << 8 | v << 16 | 0xff << 24;
            }
        }

        public static unsafe void LineDrawActionByte1(int x1Include, int x2Exclude, int[] sixs, int bytepp, byte* sptr, int* dp, double floatValueMax) {
            for (int x = x1Include; x < x2Exclude; x++, dp++) {
                int six = sixs[x];
                byte* sp = &sptr[six * bytepp];

                int v = sp[0];
                *dp = v | v << 8 | v << 16 | 0xff << 24;
            }
        }

        public static unsafe void LineDrawActionByte2BE(int x1Include, int x2Exclude, int[] sixs, int bytepp, byte* sptr, int* dp, double floatValueMax) {
            for (int x = x1Include; x < x2Exclude; x++, dp++) {
                int six = sixs[x];
                byte* sp = &sptr[six * bytepp];

                int v = sp[0];
                *dp = v | v << 8 | v << 16 | 0xff << 24;
            }
        }

        public static unsafe void LineDrawActionByte2LE(int x1Include, int x2Exclude, int[] sixs, int bytepp, byte* sptr, int* dp, double floatValueMax) {
            for (int x = x1Include; x < x2Exclude; x++, dp++) {
                int six = sixs[x];
                byte* sp = &sptr[six * bytepp];

                int v = sp[1];
                *dp = v | v << 8 | v << 16 | 0xff << 24;
            }
        }

        public static unsafe void LineDrawActionByte3(int x1Include, int x2Exclude, int[] sixs, int bytepp, byte* sptr, int* dp, double floatValueMax) {
            for (int x = x1Include; x < x2Exclude; x++, dp++) {
                int six = sixs[x];
                byte* sp = &sptr[six * bytepp];

                *dp = sp[0] | sp[1] << 8 | sp[2] << 16 | 0xff << 24;
            }
        }

        public static unsafe void LineDrawActionByte4(int x1Include, int x2Exclude, int[] sixs, int bytepp, byte* sptr, int* dp, double floatValueMax) {
            for (int x = x1Include; x < x2Exclude; x++, dp++) {
                int six = sixs[x];
                byte* sp = &sptr[six * bytepp];

                *dp = *(int*)sp;
            }
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
}
