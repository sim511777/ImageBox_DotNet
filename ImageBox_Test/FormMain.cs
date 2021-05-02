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

namespace ImageBox_Test {
    public partial class FormMain : Form {
        public FormMain() {
            InitializeComponent();
            var fonts = typeof(BitmapFonts).GetFields().Select(fi => Tuple.Create(fi.Name, fi.GetValue(null))).ToArray();
            cbxFont.DataSource = fonts;
            cbxFont.SelectedIndex = 14;
        }

        private void imgBox_PaintBackBuffer(object sender, IntPtr buf, int bw, int bh) {
            ImageDrawing id = imgBox.GetImageDrawing(buf, bw, bh);
            id.DrawLine(Color.Red, 0, 0, 8, 8);
            id.DrawRectangle(Color.Red, 8, 8, 4, 4);
            id.DrawRectangle(Color.Red, 16.5f, 16.5f, 4f, 4f);

            var text = tbxExample.Text;
            var font = (BitmapFont)cbxFont.SelectedValue;
            id.DrawString(text, font, Color.Blue, 50, 50);
            id.DrawString(text, font, Color.Blue, 200, 200, Color.Yellow);



            //for (int i = 0; i < 100; i++) {
            //    for (int j = 0; j < 100; j++) {
            //        id.DrawCircle(Color.Lime, j, i, 1, false);
            //        id.DrawSquare(Color.Lime, j, i, 1, false);
            //        id.DrawCross(Color.Lime, j, i, 1, false);
            //        id.DrawPlus(Color.Lime, j, i, 1, false);
            //        id.DrawCircle(Color.Lime, j, i, 8, true);
            //        id.DrawSquare(Color.Lime, j, i, 8, true);
            //        id.DrawCross(Color.Lime, j, i, 8, true);
            //        id.DrawPlus(Color.Lime, j, i, 8, true);
            //    }
            //}
        }

        private void imageBox_Paint(object sender, PaintEventArgs e) {
            //ImageGraphics ig = imgBox.GetImageGraphics(e.Graphics);
            //ig.DrawLine(Pens.Red, 0, 0, 8, 8);
            //ig.DrawRectangle(Pens.Red, 8, 8, 4, 4);
            //ig.DrawString("Hello, World", Font, Brushes.Lime, 12, 12);
            //ig.DrawRectangle(Pens.Red, 16.5f, 16.5f, 4f, 4f);

            //for (int i = 0; i < 100; i++) {
            //    for (int j = 0; j < 100; j++) {
            //        ig.DrawCircle(Pens.Lime, j, i, 1, false);
            //        ig.DrawSquare(Pens.Lime, j, i, 1, false);
            //        ig.DrawCross(Pens.Lime, j, i, 1, false);
            //        ig.DrawPlus(Pens.Lime, j, i, 1, false);
            //        ig.DrawCircle(Pens.Lime, j, i, 8, true);
            //        ig.DrawSquare(Pens.Lime, j, i, 8, true);
            //        ig.DrawCross(Pens.Lime, j, i, 8, true);
            //        ig.DrawPlus(Pens.Lime, j, i, 8, true);
            //    }
            //}
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
            Util.FreeBuffer(ref imgBuf);
            Util.BitmapToImageBuffer(bmp, ref imgBuf, ref bw, ref bh, ref bytepp);
            imgBox.SetImageBuffer(imgBuf, bw, bh, bytepp, false);
            imgBox.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e) {
            SetImage(Resources.Lenna_8);
        }

        private void button2_Click(object sender, EventArgs e) {
            SetImage(Resources.Lenna_24);
        }

        private void button3_Click(object sender, EventArgs e) {
            SetImage(Resources.Lenna_32);
        }

        private void button4_Click(object sender, EventArgs e) {
            SetImage(Resources.Chess);
        }

        private void btnOpen_Click(object sender, EventArgs e) {
            if (dlgOpen.ShowDialog() != DialogResult.OK)
                return;
            using (Bitmap bmp = new Bitmap(dlgOpen.FileName)) {
                SetImage(bmp);
            }
        }

        private void tbxExample_TextChanged(object sender, EventArgs e) {
            imgBox.Invalidate();
        }

        private void cbxFont_SelectedIndexChanged(object sender, EventArgs e) {
            imgBox.Invalidate();
        }
    }
}
