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
        }

        private void imageBox_Paint(object sender, PaintEventArgs e) {
            var g = e.Graphics;
            imgBox.DrawLine(g, Pens.Red, 0, 0, 8, 8);
            imgBox.DrawRectangle(g, Pens.Red, 8, 8, 4, 4);
            imgBox.DrawString(g, "Hello, World", Font, Brushes.Lime, 12, 12);
            imgBox.DrawRectangle(g, Pens.Red, 16.5f, 16.5f, 4f, 4f);
            for (int i = 0; i < 10; i++) {
                for (int j = 0; j < 10; j++) {
                    //imgBox.DrawCircle(g, Pens.Lime, j, i, 1, false);
                    //imgBox.DrawSquare(g, Pens.Lime, j, i, 1, false);
                    //imgBox.DrawCross(g, Pens.Lime, j, i, 1, false);
                    //imgBox.DrawPlus(g, Pens.Lime, j, i, 1, false);
                    //imgBox.DrawCircle(g, Pens.Lime, j, i, 8, true);
                    //imgBox.DrawSquare(g, Pens.Lime, j, i, 8, true);
                    //imgBox.DrawCross(g, Pens.Lime, j, i, 8, true);
                    //imgBox.DrawPlus(g, Pens.Lime, j, i, 8, true);
                }
            }
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
            Bitmap bmp = new Bitmap(dlgOpen.FileName);
            SetImage(bmp);
        }
    }
}
