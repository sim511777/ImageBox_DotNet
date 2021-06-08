using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ShimLib {
    public unsafe class DibSection  : IDisposable{
        IntPtr m_hWnd;              // 핸들
        IntPtr m_hScreenDC;         // 화면DC
        int m_iWidth;               // 너비
        int m_iHeight;              // 높이    
        IntPtr m_hMemoryDC;         // 메모리DC    
        IntPtr m_hDIBBitmap;        // DIB비트맵
        IntPtr m_hOldBitmap;        // 이전 선택 비트맵    
        IntPtr m_pBits;             // 레스터 데이터 어드레스
        int m_iBytesPerScanLine;    // 라인당 바이트수

        public int Width => m_iWidth;
        public int Height => m_iHeight;
        public IntPtr BufPtr => m_pBits;
        public IntPtr Hdc => m_hMemoryDC;

        public DibSection(IntPtr hWnd, int iWidth, int iHeight) {
            Create(hWnd, iWidth, iHeight);
        }

        // Dispose, Finalizer 패턴
        private bool disposed = false;

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DibSection() {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposed) {
                if (disposing) {
                    // Release Managed Resource
                }
                // Release Unmanaged Resource
                Destroy();
                disposed = true;
            }
        }

        void Create(IntPtr hWnd, int iWidth, int iHeight) {
            m_hWnd = hWnd;
            m_hScreenDC = Win32Api.GetDC(hWnd);
            m_hMemoryDC = Win32Api.CreateCompatibleDC(m_hScreenDC);
            m_iWidth = iWidth;
            m_iHeight = iHeight;
            m_iBytesPerScanLine = (iWidth * 4);

            BITMAPINFO Bitmapinfo = new BITMAPINFO();
            Bitmapinfo.bmiHeader.biSize = (uint)sizeof(BITMAPINFOHEADER);
            Bitmapinfo.bmiHeader.biWidth = iWidth;
            Bitmapinfo.bmiHeader.biHeight = -iHeight;
            Bitmapinfo.bmiHeader.biPlanes = 1;
            Bitmapinfo.bmiHeader.biBitCount = 32; // BGRA 
            Bitmapinfo.bmiHeader.biCompression = 0;
            Bitmapinfo.bmiHeader.biSizeImage = 0; // This may be set to zero for BI_RGB bitmaps. 
            Bitmapinfo.bmiHeader.biXPelsPerMeter = 0;
            Bitmapinfo.bmiHeader.biYPelsPerMeter = 0;
            Bitmapinfo.bmiHeader.biClrUsed = 0;
            Bitmapinfo.bmiHeader.biClrImportant = 0;
            Bitmapinfo.bmiColors = new RGBQuad[256];
            Bitmapinfo.bmiColors[0].rgbBlue = 0;
            Bitmapinfo.bmiColors[0].rgbGreen = 0;
            Bitmapinfo.bmiColors[0].rgbRed = 0;
            Bitmapinfo.bmiColors[0].rgbReserved = 0;

            m_hDIBBitmap = Win32Api.CreateDIBSection(m_hMemoryDC, ref Bitmapinfo, 0, out m_pBits, IntPtr.Zero, 0);
            m_hOldBitmap = Win32Api.SelectObject(m_hMemoryDC, m_hDIBBitmap);
        }

        void Destroy() {
            Win32Api.SelectObject(m_hMemoryDC, m_hOldBitmap);
            Win32Api.DeleteObject(m_hDIBBitmap);
            Win32Api.DeleteDC(m_hMemoryDC);
            if (m_hScreenDC != IntPtr.Zero)
                Win32Api.ReleaseDC(m_hWnd, m_hScreenDC);
        }

        public void BitBlt(IntPtr hDC) {
            if (hDC == IntPtr.Zero)
                hDC = m_hScreenDC;

            if (hDC != IntPtr.Zero && m_hMemoryDC != IntPtr.Zero)
                Win32Api.BitBlt(hDC, 0, 0, m_iWidth, m_iHeight, m_hMemoryDC, 0, 0, TernaryRasterOperation.SRCCOPY);
        }
    }
}
