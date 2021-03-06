﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ShimLib {
    public class ImageUtil {
        // *.bmp file load / save
        static readonly byte[] grayPalette = Enumerable.Range(0, 1024).Select(i => i % 4 == 3 ? (byte)0xff : (byte)(i / 4)).ToArray();
        public static unsafe T StreamReadStructure<T>(Stream sr) {
            int size = Marshal.SizeOf(typeof(T));
            byte[] buf = new byte[size];
            sr.Read(buf, 0, size);
            fixed (byte* ptr = buf) {
                T obj = (T)Marshal.PtrToStructure((IntPtr)ptr, typeof(T));
                return obj;
            }
        }
        public static unsafe void StreamWriteStructure<T>(Stream sr, T obj) {
            int size = Marshal.SizeOf(typeof(T));
            byte[] buf = new byte[size];
            fixed (byte* ptr = buf) {
                Marshal.StructureToPtr(obj, (IntPtr)ptr, false);
            }
            sr.Write(buf, 0, size);
        }
        public static unsafe void LoadBmpFile(string filePath, ref IntPtr imgBuf, ref int bw, ref int bh, ref int bytepp) {
            // 파일 오픈
            FileStream hFile;
            try {
                hFile = File.OpenRead(filePath);
            } catch {
                return;
            }

            // 파일 헤더
            BITMAPFILEHEADER fh = StreamReadStructure<BITMAPFILEHEADER>(hFile);

            // 정보 헤더
            BITMAPINFOHEADER ih = StreamReadStructure<BITMAPINFOHEADER>(hFile);
            
            if (ih.biBitCount == 1) {
                hFile.Dispose();
                var bmp = new Bitmap(filePath);
                BitmapToImageBuffer1Bit(bmp, ref imgBuf, ref bw, ref bh, ref bytepp);
                bmp.Dispose();
                return;
            }

            bw = ih.biWidth;
            bh = ih.biHeight;
            bytepp = ih.biBitCount / 8;
            int bstep = bw * bytepp;
            imgBuf = Marshal.AllocHGlobal(bstep * bh);


            // bmp파일은 파일 저장시 라인당 4byte padding을 한다.
            // bstep가 4로 나눠 떨어지지 않을경우 padding처리 해야 함
            int fstep = (bstep + 3) / 4 * 4;
            byte[] fbuf = new byte[fstep * bh];
            hFile.Seek(fh.bfOffBits, SeekOrigin.Begin);
            hFile.Read(fbuf, 0, bh * fstep);

            // bmp파일은 위아래가 뒤집혀 있으므로 파일에서 아래 라인부터 읽어서 버퍼에 쓴다
            for (int y = 0; y < bh; y++) {
                Marshal.Copy(fbuf, (bh - y - 1) * fstep, imgBuf + bstep * y, bstep);
            }

            hFile.Dispose();
            return;
        }
        public static unsafe void SaveBmpFile(string filePath, IntPtr imgBuf, int bw, int bh, int bytepp) {
            // 파일 오픈
            FileStream hFile;
            try {
                hFile = File.OpenWrite(filePath);
            } catch {
                return;
            }

            int bstep = bw * bytepp;
            int fstep = (bstep + 3) / 4 * 4;

            // 파일 헤더
            BITMAPFILEHEADER fh;
            fh.bfType = 0x4d42;  // Magic NUMBER "BM"
            fh.bfOffBits = (uint)(Marshal.SizeOf(typeof(BITMAPFILEHEADER)) + Marshal.SizeOf(typeof(BITMAPINFOHEADER)) + (bytepp == 1 ? grayPalette.Length : 0));   // offset to bitmap buffer from start
            fh.bfSize = fh.bfOffBits + (uint)(fstep * bh);  // file size
            fh.bfReserved1 = 0;
            fh.bfReserved2 = 0;
            StreamWriteStructure(hFile, fh);

            // 정보 헤더
            BITMAPINFOHEADER ih;
            ih.biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));   // struct of BITMAPINFOHEADER
            ih.biWidth = bw; // widht
            ih.biHeight = bh; // height
            ih.biPlanes = 1;
            ih.biBitCount = (ushort)(8 * bytepp);
            ih.biCompression = 0;
            ih.biSizeImage = (uint)(fstep * bh);
            ih.biXPelsPerMeter = 3780;  // pixels-per-meter
            ih.biYPelsPerMeter = 3780;  // pixels-per-meter
            ih.biClrUsed = (bytepp == 1) ? (uint)256 : 0;   // grayPalette count
            ih.biClrImportant = (bytepp == 1) ? (uint)256 : 0;   // grayPalette count
            StreamWriteStructure(hFile, ih);

            // RGB Palette
            if (bytepp == 1)
                hFile.Write(grayPalette, 0, grayPalette.Length);

            // bmp파일은 파일 저장시 라인당 4byte padding을 한다.
            // bstep가 4로 나눠 떨어지지 않을경우 padding처리 해야 함
            int paddingSize = fstep - bstep;
            byte[] paddingBuf = { 0, 0, 0, 0 };

            byte[] fbuf = new byte[bh * fstep];
            // bmp파일은 위아래가 뒤집혀 있으므로 버퍼 아래라인 부터 읽어서 파일에 쓴다
            for (int y = bh - 1; y >= 0; y--) {
                Marshal.Copy(imgBuf + y * bstep, fbuf, (bh - y - 1) * fstep, bstep);
                if (paddingSize > 0)
                    Array.Copy(paddingBuf, 0, fbuf, (bh - y - 1) * fstep + bstep, paddingSize);
            }
            hFile.Write(fbuf, 0, bh * fstep);

            hFile.Dispose();
            return;
        }

        // *.hra file load / save
        public static unsafe void LoadHraFile(string fileName, ref IntPtr imgBuf, ref int bw, ref int bh, ref int bytepp) {
            Stream sr = null;
            try {
                sr = File.OpenRead(fileName);
                using (var br = new BinaryReader(sr)) {
                    sr = null;
                    br.BaseStream.Position = 252;
                    bytepp = br.ReadInt32();
                    bw = br.ReadInt32();
                    bh = br.ReadInt32();

                    int bufSize = bw * bh * bytepp;
                    imgBuf = Util.AllocBuffer(bufSize);

                    byte[] fbuf = br.ReadBytes(bufSize);
                    for (int y = 0; y < bh; y++) {
                        byte* dp = (byte*)imgBuf.ToPointer() + bw * bytepp * y;
                        int idx = bw * bytepp * y;
                        for (int x = 0; x < bw; x++, dp += bytepp, idx += bytepp) {
                            if (bytepp == 1)
                                dp[0] = fbuf[idx];
                            else if (bytepp == 2) {
                                dp[0] = fbuf[idx];
                                dp[1] = fbuf[idx + 1];
                            }
                        }
                    }
                }
            } finally {
                sr?.Dispose();
            }
        }
        public static unsafe void SaveHraFile(string fileName, IntPtr imgBuf, int bw, int bh, int bytepp) {
            Stream sr = null;
            try {
                sr = File.OpenWrite(fileName);
                using (var bwr = new BinaryWriter(sr)) {
                    sr = null;
                    for (int i = 0; i < 252; i++)
                        bwr.Write((byte)0);
                    bwr.Write(bytepp);
                    bwr.Write(bw);
                    bwr.Write(bh);

                    int bufSize = bw * bh * bytepp;
                    byte[] fbuf = new byte[bufSize];

                    for (int y = 0; y < bh; y++) {
                        byte* sp = (byte*)imgBuf.ToPointer() + bw * bytepp * y;
                        int idx = bw * bytepp * y;
                        for (int x = 0; x < bw; x++, sp += bytepp, idx += bytepp) {
                            if (bytepp == 1)
                                fbuf[idx] = sp[0];
                            else if (bytepp == 2) {
                                fbuf[idx] = sp[0];
                                fbuf[idx + 1] = sp[1];
                            }
                        }
                    }
                    bwr.Write(fbuf);
                }
            } finally {
                sr?.Dispose();
            }
        }
        // hra buffer to Bitmap24
        private static unsafe Bitmap HraBufferToBitmap24(IntPtr imgBuf, int bw, int bh, int bytepp) {
            Bitmap bmp = new Bitmap(bw, bh, PixelFormat.Format24bppRgb);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bw, bh), ImageLockMode.WriteOnly, bmp.PixelFormat);
            int paddingSize = bmpData.Stride - bw * 3;
            byte[] paddingBuf = { 0, 0, 0, 0 };
            for (int y = 0; y < bh; y++) {
                byte* srcPtr = (byte*)imgBuf + bw * bytepp * y;
                byte* dstPtr = (byte*)bmpData.Scan0 + bmpData.Stride * y;
                for (int x = 0; x < bw; x++, srcPtr += bytepp, dstPtr += 3) {
                    byte gray = srcPtr[0];
                    dstPtr[0] = gray;
                    dstPtr[1] = gray;
                    dstPtr[2] = gray;
                }

                if (paddingSize > 0)
                    Marshal.Copy(paddingBuf, 0, (IntPtr)dstPtr, paddingSize);
            }
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        // Bitmap class to / from Image buffer
        public static unsafe void BitmapToImageBuffer(Bitmap bmp, ref IntPtr imgBuf, ref int bw, ref int bh, ref int bytepp) {
            if (bmp.PixelFormat == PixelFormat.Format1bppIndexed) {
                BitmapToImageBuffer1Bit(bmp, ref imgBuf, ref bw, ref bh, ref bytepp);
                return;
            }

            bw = bmp.Width;
            bh = bmp.Height;
            if (bmp.PixelFormat == PixelFormat.Format8bppIndexed)
                bytepp = 1;
            else if (bmp.PixelFormat == PixelFormat.Format16bppGrayScale)
                bytepp = 2;
            else if (bmp.PixelFormat == PixelFormat.Format24bppRgb)
                bytepp = 3;
            else if (bmp.PixelFormat == PixelFormat.Format32bppRgb || bmp.PixelFormat == PixelFormat.Format32bppArgb || bmp.PixelFormat == PixelFormat.Format32bppPArgb)
                bytepp = 4;
            Int64 bufSize = (Int64)bw * bh * bytepp;
            imgBuf = Util.AllocBuffer(bufSize);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bw, bh), ImageLockMode.ReadOnly, bmp.PixelFormat);
            int copySize = bw * bytepp;
            for (int y = 0; y < bh; y++) {
                IntPtr dstPtr = imgBuf + bw * y * bytepp;
                IntPtr srcPtr = bmpData.Scan0 + bmpData.Stride * y;
                Util.Memcpy(dstPtr, srcPtr, copySize);
            }

            bmp.UnlockBits(bmpData);
        }
        public static unsafe Bitmap ImageBufferToBitmap(IntPtr imgBuf, int bw, int bh, int bytepp, bool isImgbufFloat, double floatValueMax) {
            if (isImgbufFloat) {
                return FloatBufferToBitmap8(imgBuf, bw, bh, bytepp, floatValueMax);
            }
            if (bytepp == 2) {
                return HraBufferToBitmap24(imgBuf, bw, bh, bytepp);
            }
            PixelFormat pixelFormat;
            if (bytepp == 1)
                pixelFormat = PixelFormat.Format8bppIndexed;
            else if (bytepp == 3)
                pixelFormat = PixelFormat.Format24bppRgb;
            else if (bytepp == 4)
                pixelFormat = PixelFormat.Format32bppRgb;
            else
                return null;

            Bitmap bmp = new Bitmap(bw, bh, pixelFormat);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bw, bh), ImageLockMode.WriteOnly, bmp.PixelFormat);
            int copySize = bw * bytepp;
            int paddingSize = bmpData.Stride - copySize;
            byte[] paddingBuf = { 0, 0, 0, 0 };
            for (int y = 0; y < bh; y++) {
                IntPtr srcPtr = imgBuf + bw * y * bytepp;
                IntPtr dstPtr = bmpData.Scan0 + bmpData.Stride * y;
                Util.Memcpy(dstPtr, srcPtr, copySize);
                if (paddingSize > 0)
                    Marshal.Copy(paddingBuf, 0, dstPtr + copySize, paddingSize);
            }
            bmp.UnlockBits(bmpData);
            if (bmp.PixelFormat == PixelFormat.Format8bppIndexed) {
                var pal = bmp.Palette;
                for (int i = 0; i < pal.Entries.Length; i++) {
                    pal.Entries[i] = Color.FromArgb(i, i, i);
                }
                bmp.Palette = pal;
            }
            return bmp;
        }
        // All Bitmap to gray Image Buffer
        public static unsafe void BitmapToGrayImageBuffer(Bitmap bmp, ref IntPtr imgBuf, ref int bw, ref int bh, ref int bytepp) {
            if (bmp.PixelFormat == PixelFormat.Format1bppIndexed) {
                Bitmap1BitToGrayImageBuffer(bmp, ref imgBuf, ref bw, ref bh, ref bytepp);
                return;
            }

            bw = bmp.Width;
            bh = bmp.Height;
            if (bmp.PixelFormat == PixelFormat.Format8bppIndexed)
                bytepp = 1;
            else if (bmp.PixelFormat == PixelFormat.Format16bppGrayScale)
                bytepp = 2;
            else if (bmp.PixelFormat == PixelFormat.Format24bppRgb)
                bytepp = 3;
            else if (bmp.PixelFormat == PixelFormat.Format32bppRgb || bmp.PixelFormat == PixelFormat.Format32bppArgb || bmp.PixelFormat == PixelFormat.Format32bppPArgb)
                bytepp = 4;
            Int64 bufSize = (Int64)bw * bh;
            imgBuf = Util.AllocBuffer(bufSize);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bw, bh), ImageLockMode.ReadOnly, bmp.PixelFormat);
            for (int y = 0; y < bh; y++) {
                byte* dstPtr = (byte*)imgBuf + bw * y;
                byte* srcPtr = (byte*)bmpData.Scan0 + bmpData.Stride * y;
                for (int x = 0; x < bw; x++, dstPtr++, srcPtr += bytepp) {
                    byte gray = 0;
                    if (bytepp == 1) {
                        gray = *srcPtr;
                    } else if (bytepp == 2) {
                        gray = *(srcPtr + 1);
                    } else if (bytepp == 3 || bytepp == 4) {
                        int b = *srcPtr;
                        int g = *(srcPtr + 1);
                        int r = *(srcPtr + 2);
                        gray = (byte)((3 * r + 6 * g + 1 * b) / 10);
                    }
                    *dstPtr = gray;
                }
            }

            bmp.UnlockBits(bmpData);

            bytepp = 1;
        }
        
        // 1bit Bitmap to gray Image buffer => 두 함수가 동일 기능 같아 보임
        public static unsafe void Bitmap1BitToGrayImageBuffer(Bitmap bmp, ref IntPtr imgBuf, ref int bw, ref int bh, ref int bytepp) {
            bw = bmp.Width;
            bh = bmp.Height;
            Int64 bufSize = (Int64)bw * bh;
            imgBuf = Util.AllocBuffer(bufSize);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bw, bh), ImageLockMode.ReadOnly, bmp.PixelFormat);
            for (int y = 0; y < bh; y++) {
                byte* dstPtr = (byte*)imgBuf + bw * y;
                byte* srcPtr = (byte*)bmpData.Scan0 + bmpData.Stride * y;
                
                int stepNum = bw / 8;
                int stepRemains = bw % 8;
                int val = 0;
                for (int step = 0; step < stepNum; step++, dstPtr += 8, srcPtr++) {
                    val = *srcPtr;
                    for (int bit = 0; bit < 8; bit++) {
                        dstPtr[bit] = (byte)((val >> (7 - bit) & 1) * 255);
                    }
                }
                val = *srcPtr;
                for (int bit = 0; bit < stepRemains; bit++) {
                    dstPtr[bit] = (byte)((val >> (7 - bit) & 1) * 255);
                }
            }

            bmp.UnlockBits(bmpData);

            bytepp = 1;
        }
        private static unsafe void BitmapToImageBuffer1Bit(Bitmap bmp, ref IntPtr imgBuf, ref int bw, ref int bh, ref int bytepp) {
            bw = bmp.Width;
            bh = bmp.Height;
            bytepp = 1;
            long bufSize = (long)bw * bh * bytepp;
            imgBuf = Util.AllocBuffer(bufSize);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bw, bh), ImageLockMode.ReadOnly, bmp.PixelFormat);
            int w8 = bw / 8;
            for (int y = 0; y < bh; y++) {
                byte* sptr = (byte*)bmpData.Scan0 + bmpData.Stride * y;
                byte* dptr = (byte*)imgBuf + bw * y;
                for (int x8 = 0; x8 < w8; x8++, sptr++) {
                    byte sp = *sptr;
                    for (int x = 0; x < 8; x++, dptr++) {
                        if (((sp << x) & 0x80) != 0)
                            *dptr = 255;
                        else
                            *dptr = 0;
                    }
                }
            }

            bmp.UnlockBits(bmpData);
        }
        
        // float Image buffer to 8Bit Gray BItmap
        private static unsafe Bitmap FloatBufferToBitmap8(IntPtr imgBuf, int bw, int bh, int bytepp, double floatValueMax) {
            Bitmap bmp = new Bitmap(bw, bh, PixelFormat.Format8bppIndexed);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bw, bh), ImageLockMode.WriteOnly, bmp.PixelFormat);
            float floatScale = 255 / (float)floatValueMax;
            int copySize = bw * bytepp;
            int paddingSize = bmpData.Stride - copySize;
            byte[] paddingBuf = { 0, 0, 0, 0 };
            for (int y = 0; y < bh; y++) {
                IntPtr srcPtr = imgBuf + bw * y * bytepp;
                IntPtr dstPtr = bmpData.Scan0 + bmpData.Stride * y;
                float* sp = (float*)srcPtr;
                byte* dp = (byte*)dstPtr;
                for (int x = 0; x < bw; x++, sp++, dp++) {
                    float gray = *(float*)sp * floatScale;
                    if (gray > 255f) gray = 255f;
                    if (gray < 0f) gray = 0f;
                    *dp = (byte)gray;
                }
                if (paddingSize > 0)
                    Marshal.Copy(paddingBuf, 0, dstPtr + copySize, paddingSize);
            }
            bmp.UnlockBits(bmpData);
            if (bmp.PixelFormat == PixelFormat.Format8bppIndexed) {
                var pal = bmp.Palette;
                for (int i = 0; i < pal.Entries.Length; i++) {
                    pal.Entries[i] = Color.FromArgb(i, i, i);
                }
                bmp.Palette = pal;
            }
            return bmp;
        }
        // float / double Image buffer to gray Image buffer;
        public static unsafe void FloatBufToByte(IntPtr floatBuf, int bw, int bh, int bytepp, IntPtr byteBuf) {
            for (int y = 0; y < bh; y++) {
                byte* psrc = (byte*)floatBuf + (bw * y) * bytepp;
                byte* pdst = (byte*)byteBuf + bw * y;
                for (int x = 0; x < bw; x++, pdst++, psrc += bytepp) {
                    if (bytepp == 4)
                        *pdst = (byte)Util.Clamp(*(float*)psrc, 0, 255);
                    else if (bytepp == 8)
                        *pdst = (byte)Util.Clamp(*(double*)psrc, 0, 255);
                }
            }
        }
    }
}
