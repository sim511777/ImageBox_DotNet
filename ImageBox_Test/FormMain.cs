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
using System.Threading.Tasks;

namespace ImageBox_Test {
    public partial class FormMain : Form {
        public FormMain(string[] args) {
            InitializeComponent();
            LoadBitmapFonts(string.Empty);
            if (args.Length > 0) {
                LoadImageFile(args[0]);
            }
        }
        
        private void LoadBitmapFonts(string bdfDir) {
            //cbxFont.DisplayMember = "Key";
            //cbxFont.ValueMember = "Value";
            //cbxFont.DataSource = new BindingSource(Fonts.dic, null);
            
            List<Tuple<string, IFont>> fontList = new List<Tuple<string, IFont>>();
            foreach (var kv in Fonts.dic) {
                var tuple = Tuple.Create(kv.Key.ToString(), kv.Value);
                fontList.Add(tuple);
            }

            if (Directory.Exists(bdfDir)) {
                var bdfFiles = Directory.GetFiles(bdfDir);
                foreach (var bdfFile in bdfFiles) {
                    if (Path.GetExtension(bdfFile).ToLower() != ".bdf")
                        continue;
                    var name = Path.GetFileNameWithoutExtension(bdfFile);
                    var bdfText = File.ReadAllText(bdfFile);
                    var font = new BdfFont(bdfText);
                    var tuple = Tuple.Create(name, (IFont)font);
                    fontList.Add(tuple);
                }
            }

            cbxFont.DisplayMember = "Item1";
            cbxFont.ValueMember = "Item2";
            cbxFont.DataSource = fontList;
            cbxFont.SelectedIndex = cbxFont.Items.Count - 1;
            this.lblSystemFont.Text = dlgFont.Font.ToString();
        }

        private void SaveFontSampleImages(IEnumerable<Tuple<string, IFont>> fontTupleList) {
            string text = tbxExample.Text;
            foreach (var fontTuple in fontTupleList) {
                var name = fontTuple.Item1;
                var font = fontTuple.Item2;
                var size = font.MeasureString(text);
                using (var bmp = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppPArgb)) {
                    var bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                    font.DrawString(text, bd.Scan0, size.Width, size.Height, 0, 0, Color.White);
                    bmp.UnlockBits(bd);
                    bmp.Save(name + ".bmp", ImageFormat.Bmp);
                }
            }
        }

        private void imgBox_PaintBackBuffer(object sender, IntPtr buf, int bw, int bh) {
            ImageDrawing id = imgBox.GetImageDrawing(buf, bw, bh);
            int testIdx = lbxDrawTest.SelectedIndex;
            if (testIdx == 1) {         // Drawing Shapes
                id.DrawLine(Color.Red, 0, 0, 8, 8);
                id.DrawRectangle(Color.Red, 8, 8, 4, 4);
                id.DrawRectangle(Color.Red, 16.5f, 16.5f, 4f, 4f);
            } else if (testIdx == 2) {  // Drawing Text
                var font = (IFont)cbxFont.SelectedValue;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(font.GetType().Name);
                sb.AppendLine(cbxFont.Text);
                sb.Append(tbxExample.Text);
                var text = sb.ToString();
                id.DrawString(text, font, Color.Blue, 127.5f, 127.5f, Color.Yellow);
            } else if (testIdx == 3) {  // Drawing Repeat
                Action<int> iAction = (i) => {
                    for (int j = 0; j < 100; j++) {
                        id.DrawCircle(Color.Lime, j, i, 1, false);
                        id.DrawSquare(Color.Lime, j, i, 1, false);
                        id.DrawCross(Color.Lime, j, i, 1, false);
                        id.DrawPlus(Color.Lime, j, i, 1, false);
                    }
                };
                if (chkDrawingRepeatParallel.Checked)
                    Parallel.For(0, 100, iAction);
                else
                    for (int i = 0; i < 100; i++) iAction(i);
            } else if (testIdx == 4) {  // Drawing All Charactors
                var font = (IFont)cbxFont.SelectedValue;
                Action<int> yAction = (y) => {
                    for (int x = 0; x < 256; x++) {
                        char ch = (char)(y * 256 + x);
                        string str = new string(ch, 1);
                        id.DrawString(str, font, Color.Black, x - 0.5f, y - 0.5f);
                    }
                };
                if (chkDrawingRepeatParallel.Checked)
                    Parallel.For(0, 256, yAction);
                else
                    for (int y = 0; y < 256; y++) yAction(y);
            }
        }

        private void imageBox_Paint(object sender, PaintEventArgs e) {
            ImageGraphics ig = imgBox.GetImageGraphics(e.Graphics);
            int testIdx = lbxDrawTest.SelectedIndex;
            if (testIdx == 5) {         // Graphics Shapes
                ig.DrawLine(Pens.Red, 0, 0, 8, 8);
                ig.DrawRectangle(Pens.Red, 8, 8, 4, 4);
                ig.DrawRectangle(Pens.Red, 16.5f, 16.5f, 4f, 4f);
            } else if (testIdx == 6) {  // Graphics Text
                var font = dlgFont.Font;
                var fontName = font.Name;
                var text = fontName + Environment.NewLine + tbxExample.Text;
                ig.DrawString(text, font, Brushes.Blue, 127.5f, 127.5f, Brushes.Yellow);
            } else if (testIdx == 7) {  // Graphics Repeat
                for (int i = 0; i < 100; i++) {
                    for (int j = 0; j < 100; j++) {
                        ig.DrawCircle(Pens.Lime, j, i, 1, false);
                        ig.DrawSquare(Pens.Lime, j, i, 1, false);
                        ig.DrawCross(Pens.Lime, j, i, 1, false);
                        ig.DrawPlus(Pens.Lime, j, i, 1, false);
                    }
                }
            } else if (testIdx == 8) {  // Graphics All Charactors
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
            ImageUtil.BitmapToImageBuffer(bmp, ref imgBuf, ref bw, ref bh, ref bytepp);
            imgBox.SetImageBuffer(imgBuf, bw, bh, bytepp, false);
            imgBox.Invalidate();
        }

        private void SetImage_toFloat(Bitmap bmp) {
            Util.FreeBuffer(ref imgBuf);
            ImageUtil.BitmapToGrayImageBuffer(bmp, ref imgBuf, ref bw, ref bh, ref bytepp);

            // byte -> float
            bytepp = 4;
            unsafe {
                var bufOld = imgBuf;
                imgBuf = Marshal.AllocHGlobal(bw * bh * bytepp);
                for (int y = 0; y < bh; y++) {
                    var pbyte = (byte*)bufOld + y * bw;
                    var pfloat = (float*)imgBuf + y * bw;
                    for (int x = 0; x < bw; x++, pbyte++, pfloat++) {
                        *pfloat = (float)*pbyte / 255;
                    }
                }
                Marshal.FreeHGlobal(bufOld);
            }

            imgBox.SetImageBuffer(imgBuf, bw, bh, bytepp, true);
            imgBox.Invalidate();
        }

        private void SetImage_toGray16(Bitmap bmp) {
            Util.FreeBuffer(ref imgBuf);
            ImageUtil.BitmapToGrayImageBuffer(bmp, ref imgBuf, ref bw, ref bh, ref bytepp);

            // byte -> ushort
            bytepp = 2;
            unsafe {
                var bufOld = imgBuf;
                imgBuf = Marshal.AllocHGlobal(bw * bh * bytepp);
                for (int y = 0; y < bh; y++) {
                    var pbyte = (byte*)bufOld + y * bw;
                    var pushort = (ushort*)imgBuf + y * bw;
                    for (int x = 0; x < bw; x++, pbyte++, pushort++) {
                        *pushort = (ushort)(*pbyte << 8);
                    }
                }
                Marshal.FreeHGlobal(bufOld);
            }

            imgBox.SetImageBuffer(imgBuf, bw, bh, bytepp, false);
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

        private void btnWhiteBig_Click(object sender, EventArgs e) {
            Util.FreeBuffer(ref imgBuf);
            bw = 256;
            bh = 256;
            bytepp = 1;
            imgBuf = Util.AllocBuffer(bw * bh, 255);
            imgBox.SetImageBuffer(imgBuf, bw, bh, bytepp, false);
            imgBox.Invalidate();
        }

        private void btnOpen_Click(object sender, EventArgs e) {
            if (dlgOpen.ShowDialog() != DialogResult.OK)
                return;
            LoadImageFile(dlgOpen.FileName);
        }

        private void LoadImageFile(string fileName) {
            try {
                using (var bmp = new Bitmap(fileName)) {
                    SetImage(bmp);
                }
            } catch {}
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

            LoadImageFile(filePath);
        }

        private void lbxDrawTest_SelectedIndexChanged(object sender, EventArgs e) {
            this.imgBox.Invalidate();
        }

        private void btnFont_Click(object sender, EventArgs e) {
            var dr = dlgFont.ShowDialog(this);
            if (dr != DialogResult.OK)
                return;
            this.lblSystemFont.Text = dlgFont.Font.ToString();
            this.imgBox.Invalidate();
        }

        private void btnLenna8ToGray16_Click(object sender, EventArgs e) {
            SetImage_toGray16(Resources.Lenna_8);
        }

        private void btnBitmapFont_Click(object sender, EventArgs e) {
            if (dlgFolder.ShowDialog(this) != DialogResult.OK)
                return;
            LoadBitmapFonts(dlgFolder.SelectedPath);
            imgBox.Invalidate();
        }

        private void btnSaveSampleImages_Click(object sender, EventArgs e) {
            var fontTupleList = cbxFont.DataSource as List<Tuple<string, IFont>>;
            SaveFontSampleImages(fontTupleList);
        }
    }
}
