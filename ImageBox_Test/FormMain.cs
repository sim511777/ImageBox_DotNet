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
using ImageBox;

namespace ImageBox_Test {
    public partial class FormMain : Form {
        public FormMain() {
            InitializeComponent();
        }

        private void imageBox_Paint(object sender, PaintEventArgs e) {
            ImageGraphics ig = new ImageGraphics(imgBox, e.Graphics);
            ig.DrawLine(Pens.Red, 0, 0, 32, 32);
            ig.DrawLine(Pens.Red, 32 - 0.5f, 32 - 0.5f, 48 - 0.5f, 32 - 0.5f);
            ig.DrawLine(Pens.Red, 48 - 0.5f, 32 - 0.5f, 48 - 0.5f, 48 - 0.5f);
            ig.DrawLine(Pens.Red, 48 - 0.5f, 48 - 0.5f, 32 - 0.5f, 48 - 0.5f);
            ig.DrawLine(Pens.Red, 32 - 0.5f, 48 - 0.5f, 32 - 0.5f, 32 - 0.5f);
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
            imgBox.SetImageBuffer(imgBuf, bw, bh, bytepp);
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
