using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ImageBox_Test.Properties;
using ShimLib;
using System.IO;

namespace ImageBox_Test {
    public partial class FormMain : Form {
        public FormMain() {
            InitializeComponent();
            var fonts = typeof(Fonts).GetFields().Select(fi => new { FontName = fi.Name, Font = fi.GetValue(null) }).ToArray();
            cbxFont.DataSource = fonts;
            cbxFont.SelectedIndex = cbxFont.Items.Count - 1;
            lbxDrawTest.SelectedIndex = 0;
        }

        private void imgBox_PaintBackBuffer(object sender, IntPtr buf, int bw, int bh) {
            ImageDrawing id = imgBox.GetImageDrawing(buf, bw, bh);
            if (lbxDrawTest.SelectedIndex == 1)
                ImageDrawingShape(id);
            else if (lbxDrawTest.SelectedIndex == 3)
                ImageDrawingRepeat(id);
        }

        private void imageBox_Paint(object sender, PaintEventArgs e) {
            ImageGraphics ig = imgBox.GetImageGraphics(e.Graphics);
            if (lbxDrawTest.SelectedIndex == 2)
                ImageGraphicsShape(ig);
            else if (lbxDrawTest.SelectedIndex == 4)
                ImageGraphicsRepeat(ig);
        }

        private void ImageDrawingRepeat(ImageDrawing id) {
            for (int i = 0; i < 100; i++) {
                for (int j = 0; j < 100; j++) {
                    id.DrawCircle(Color.Lime, j, i, 1, false);
                    id.DrawSquare(Color.Lime, j, i, 1, false);
                    id.DrawCross(Color.Lime, j, i, 1, false);
                    id.DrawPlus(Color.Lime, j, i, 1, false);
                    id.DrawCircle(Color.Lime, j, i, 8, true);
                    id.DrawSquare(Color.Lime, j, i, 8, true);
                    id.DrawCross(Color.Lime, j, i, 8, true);
                    id.DrawPlus(Color.Lime, j, i, 8, true);
                }
            }
        }

        private void ImageGraphicsRepeat(ImageGraphics ig) {
            for (int i = 0; i < 100; i++) {
                for (int j = 0; j < 100; j++) {
                    ig.DrawCircle(Pens.Lime, j, i, 1, false);
                    ig.DrawSquare(Pens.Lime, j, i, 1, false);
                    ig.DrawCross(Pens.Lime, j, i, 1, false);
                    ig.DrawPlus(Pens.Lime, j, i, 1, false);
                    ig.DrawCircle(Pens.Lime, j, i, 8, true);
                    ig.DrawSquare(Pens.Lime, j, i, 8, true);
                    ig.DrawCross(Pens.Lime, j, i, 8, true);
                    ig.DrawPlus(Pens.Lime, j, i, 8, true);
                }
            }
        }

        private void ImageDrawingShape(ImageDrawing id) {
            id.DrawLine(Color.Red, 0, 0, 8, 8);
            id.DrawRectangle(Color.Red, 8, 8, 4, 4);
            id.DrawRectangle(Color.Red, 16.5f, 16.5f, 4f, 4f);

            var text = tbxExample.Text;
            var font = (IFont)cbxFont.SelectedValue;
            id.DrawString(text, font, Color.Blue, 50, 50);
            id.DrawString(text, font, Color.Blue, 200, 200, Color.Yellow);
        }

        private void ImageGraphicsShape(ImageGraphics ig) {
            ig.DrawLine(Pens.Red, 0, 0, 8, 8);
            ig.DrawRectangle(Pens.Red, 8, 8, 4, 4);
            ig.DrawRectangle(Pens.Red, 16.5f, 16.5f, 4f, 4f);

            var text = tbxExample.Text;
            ig.DrawString(text, Font, Brushes.Blue, 50, 50);
            ig.DrawString(text, Font, Brushes.Blue, 200, 200, Brushes.Yellow);
        }

        private void btnResetZoom_Click(object sender, EventArgs e) {
            imgBox.ResetZoom();
            imgBox.Invalidate();
        }

        IntPtr imgBuf = IntPtr.Zero;
        int bw = 0;
        int bh = 0;
        int bytepp = 0;

        private void SetImage(Bitmap bmp) {
            PixelFormat[] supportedFormats = {
                PixelFormat.Format8bppIndexed,
                PixelFormat.Format16bppGrayScale,
                PixelFormat.Format24bppRgb,
                PixelFormat.Format32bppArgb,
                PixelFormat.Format32bppPArgb,
                PixelFormat.Format32bppRgb,
            };
            if (supportedFormats.Contains(bmp.PixelFormat) == false)
                return;
            Util.FreeBuffer(ref imgBuf);
            Util.BitmapToImageBuffer(bmp, ref imgBuf, ref bw, ref bh, ref bytepp);
            imgBox.SetImageBuffer(imgBuf, bw, bh, bytepp, false);
            imgBox.Invalidate();
        }

        private void SetImage_toFloat(Bitmap bmp) {
            Util.FreeBuffer(ref imgBuf);
            ImageUtil.BitmapToGrayImageBuffer(bmp, ref imgBuf, ref bw, ref bh, ref bytepp);

            // byte -> float
            unsafe {
                var bufOld = imgBuf;
                imgBuf = Marshal.AllocHGlobal(bw * bh * 4);
                for (int y = 0; y < bh; y++) {
                    var pbyte = (byte*)bufOld + y * bw;
                    var pfloat = (float*)imgBuf + y * bw;
                    for (int x = 0; x < bw; x++, pbyte++, pfloat++) {
                        *pfloat = (float)*pbyte / 255;
                    }
                }
                Marshal.FreeHGlobal(bufOld);
            }

            bytepp = 4;
            imgBox.SetImageBuffer(imgBuf, bw, bh, bytepp, true);
            imgBox.Invalidate();
        }

        private void btnLenna8_Click(object sender, EventArgs e) {
            SetImage(Resources.Lenna_8);
        }

        private void btnLenna24_Click(object sender, EventArgs e) {
            SetImage(Resources.Lenna_24);
        }

        private void btnLenna32_Click(object sender, EventArgs e) {
            SetImage(Resources.Lenna_32);
        }

        private void btnChess_Click(object sender, EventArgs e) {
            SetImage(Resources.Chess);
        }

        private void btnLenna8ToFloat_Click(object sender, EventArgs e) {
            SetImage_toFloat(Resources.Lenna_8);
        }

        private void btnOpen_Click(object sender, EventArgs e) {
            if (dlgOpen.ShowDialog() != DialogResult.OK)
                return;
            using (var img = Image.FromFile(dlgOpen.FileName)) {
                using (Bitmap bmp = new Bitmap(img)) {
                    SetImage(bmp);
                }
            }
        }

        private void btnPaste_Click(object sender, EventArgs e) {
            if (Clipboard.ContainsImage() == false)
                return;
            using (var img = Clipboard.GetImage()) {
                using (var bmp = new Bitmap(img)) {
                    SetImage(bmp);
                }
            }
        }

        private void tbxExample_TextChanged(object sender, EventArgs e) {
            imgBox.Invalidate();
        }

        private void cbxFont_SelectedIndexChanged(object sender, EventArgs e) {
            imgBox.Invalidate();
        }

        private string GetDragDataImageFile(IDataObject data) {
            string[] extList = { ".bmp", ".jpg", ".png", ".hra", ".tif" };

            string[] files = (string[])data.GetData(DataFormats.FileDrop);
            if (files.Length != 1)
                return null;
            
            string file = files[0];
            FileAttributes attr = File.GetAttributes(file);
            if (attr.HasFlag(FileAttributes.Directory))
                return null;

            string ext = Path.GetExtension(file).ToLower();
            if (extList.Contains(ext) == false)
                return null;

            return file;
        }

        private void imgBox_DragEnter(object sender, DragEventArgs e) {
            string imageFile = GetDragDataImageFile(e.Data);
            if (imageFile == null) {
                e.Effect = DragDropEffects.None;
                return;
            }
            e.Effect = DragDropEffects.Copy;
        }

        private void imgBox_DragDrop(object sender, DragEventArgs e) {
            string filePath = GetDragDataImageFile(e.Data);
            if (filePath == null)
                return;

            using (var img = Image.FromFile(filePath)) {
                using (Bitmap bmp = new Bitmap(img)) {
                    SetImage(bmp);
                }
            }
        }

        private void lbxDrawTest_SelectedIndexChanged(object sender, EventArgs e) {
            this.imgBox.Invalidate();
        }
    }
}
