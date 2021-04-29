using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShimLib {
    public partial class FormAbout : Form {
        private ImageBox pbx;
        public FormAbout(ImageBox pbx) {
            this.pbx = pbx;
            InitializeComponent();
        }

        ImageBoxOption optBackup;

        private void FormAbout_Load(object sender, EventArgs e) {
            optBackup = new ImageBoxOption();
            optBackup.FromImageBox(pbx);
            ImageBoxOption option = new ImageBoxOption();
            option.FromImageBox(pbx);
            grdOption.SelectedObject = option;
        }

        private void FormAbout_FormClosed(object sender, FormClosedEventArgs e) {
            if (this.DialogResult == DialogResult.Cancel) {
                optBackup.ToImageBox(pbx);
                pbx.Invalidate();
                return;
            }
        }

        private void btnSave_Click(object sender, EventArgs e) {
            //if (pbx.ImgBuf == IntPtr.Zero) {
            //    MessageBox.Show(this, "pbx.ImgBuf == IntPtr.Zero");
            //    return;
            //}

            //if (pbx.BufIsFloat) {
            //    MessageBox.Show(this, "Floating point image buffer can not be converted to Bitmap object.");
            //    return;
            //}

            //var ok = dlgSaveFile.ShowDialog(this);
            //if (ok != DialogResult.OK)
            //    return;

            //ImageUtil.SaveBmpFile(dlgSaveFile.FileName, pbx.ImgBuf, pbx.ImgBW, pbx.ImgBH, pbx.ImgBytepp);
        }

        private void btnCopy_Click(object sender, EventArgs e) {
            //if (pbx.ImgBuf == IntPtr.Zero) {
            //    return;
            //}

            //if (pbx.BufIsFloat) {
            //    return;
            //}

            //var bmp = ImageUtil.ImageBufferToBitmap(pbx.ImgBuf, pbx.ImgBW, pbx.ImgBH, pbx.ImgBytepp);
            //Clipboard.SetImage(bmp);
            //bmp.Dispose();
        }

        private void grdOption_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
            var option = grdOption.SelectedObject as ImageBoxOption;
            option.ToImageBox(pbx);
            pbx.Invalidate();
        }
    }

    public class ImageBoxOption {
        // 화면 표시 옵션
        public bool UseDrawPixelValue { get; set; } = true;
        public bool UseDrawCenterLine { get; set; } = true;
        public bool UseDrawCursorInfo { get; set; } = true;
        public bool UseDrawDebugInfo { get; set; } = true;

        public void FromImageBox(ImageBox pbx) {
            this.UseDrawPixelValue = pbx.UseDrawPixelValue;
            this.UseDrawCursorInfo = pbx.UseDrawCursorInfo;
            this.UseDrawCursorInfo = pbx.UseDrawCursorInfo;
            this.UseDrawDebugInfo = pbx.UseDrawDebugInfo;
        }

        public void ToImageBox(ImageBox pbx) {
            pbx.UseDrawPixelValue = this.UseDrawPixelValue;
            pbx.UseDrawCursorInfo = this.UseDrawCursorInfo;
            pbx.UseDrawCursorInfo = this.UseDrawCursorInfo;
            pbx.UseDrawDebugInfo = this.UseDrawDebugInfo;
        }
    }
}
