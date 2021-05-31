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
using ShimLib.Properties;

namespace ShimLib {
    public partial class FormAbout : Form {
        public FormAbout(ImageBox pbx) {
            this.pbx = pbx;
            InitializeComponent();
        }

        private ImageBox pbx;
        ImageBoxOption optBackup;

        private void FormAbout_Load(object sender, EventArgs e) {
            optBackup = pbx.Option.Copy();
            var option = pbx.Option.Copy();
            grdOption.SelectedObject = option;

            UpdateRoiList();

            tbxVersion.Text = Resource.VersionInfo;
        }

        public void UpdateRoiList() {
            lbxRoi.Items.Clear();
            lbxRoi.Items.AddRange(pbx.RoiList.Cast<object>().ToArray());
        }

        private void FormAbout_FormClosed(object sender, FormClosedEventArgs e) {
            if (this.DialogResult == DialogResult.Cancel) {
                pbx.Option = optBackup.Copy();
                pbx.Redraw();
                return;
            }
        }

        private void btnCopy_Click(object sender, EventArgs e) {
            var bmp = pbx.GetBitmap();
            if (bmp != null) {
                Clipboard.SetImage(bmp);
                bmp.Dispose();
            }
        }

        private void grdOption_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
            var option = grdOption.SelectedObject as ImageBoxOption;
            pbx.Option = option.Copy();
            pbx.Redraw();
        }

        private void btnRoiDelete_Click(object sender, EventArgs e) {
            var selIdxs = lbxRoi.SelectedIndices;
            var idxs = selIdxs.Cast<int>().OrderByDescending(i => i);
            foreach (var idx in idxs) {
                pbx.RoiList.RemoveAt(idx);
                lbxRoi.Items.RemoveAt(idx);
            }
            pbx.Redraw();
        }

        private void btnRoiListClear_Click(object sender, EventArgs e) {
            pbx.RoiList.Clear();
            UpdateRoiList();
            pbx.Redraw();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
