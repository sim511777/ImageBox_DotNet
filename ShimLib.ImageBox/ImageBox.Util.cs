using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimLib {
    public partial class ImageBox {
        // 이미지 버퍼를 디스플레이 버퍼에 복사
        private static unsafe void CopyImageBufferZoom(IntPtr imgBuf, int imgBw, int imgBh, int bytepp, bool bufIsFloat, IntPtr dispBuf, int dispBw, int dispBh, int panx, int pany, double zoom, int bgColor, double floatValueMax) {
            // 인덱스 버퍼 생성
            int[] siys = new int[dispBh];
            int[] sixs = new int[dispBw];
            for (int y = 0; y < dispBh; y++) {
                int siy = (int)Math.Floor((y - pany) / zoom);
                siys[y] = (imgBuf == IntPtr.Zero || siy < 0 || siy >= imgBh) ? -1 : siy;
            }
            for (int x = 0; x < dispBw; x++) {
                int six = (int)Math.Floor((x - panx) / zoom);
                sixs[x] = (imgBuf == IntPtr.Zero || six < 0 || six >= imgBw) ? -1 : six;
            }

            float floatScale = (float)(255 / floatValueMax);
            double doubleScale = 255 / floatValueMax;

            for (int y = 0; y < dispBh; y++) {
                int siy = siys[y];
                byte* sptr = (byte*)imgBuf + (Int64)imgBw * siy * bytepp;
                int* dp = (int*)dispBuf + (Int64)dispBw * y;
                for (int x = 0; x < dispBw; x++, dp++) {
                    int six = sixs[x];
                    if (siy == -1 || six == -1) {       // out of boundary of image
                        *dp = bgColor;
                    } else {
                        byte* sp = &sptr[six * bytepp];
                        if (bufIsFloat) {
                            if (bytepp == 4) {          // 4byte float gray
                                int v = (int)(*(float*)sp * floatScale) & 0x000000ff;
                                *dp = v | v << 8 | v << 16 | 0xff << 24;
                            } else if (bytepp == 8) {   // 8byte double gray
                                int v = (int)(*(double*)sp * doubleScale) & 0x000000ff;
                                *dp = v | v << 8 | v << 16 | 0xff << 24;
                            }
                        } else {
                            if (bytepp == 1) {          // 1byte gray
                                int v = sp[0];
                                *dp = v | v << 8 | v << 16 | 0xff << 24;
                            } else if (bytepp == 2) {   // 2byte gray (*.hra)
                                int v = sp[0];
                                *dp = v | v << 8 | v << 16 | 0xff << 24;
                            } else if (bytepp == 3) {   // 3byte bgr
                                *dp = sp[0] | sp[1] << 8 | sp[2] << 16 | 0xff << 24;
                            } else if (bytepp == 4) {   // rbyte bgra
                                *dp = sp[0] | sp[1] << 8 | sp[2] << 16 | 0xff << 24;
                            }
                        }
                    }
                }
            }
        }
    }
}
