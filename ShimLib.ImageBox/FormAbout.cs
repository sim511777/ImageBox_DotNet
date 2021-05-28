﻿using System;
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
            optBackup = new ImageBoxOption();
            optBackup.FromImageBox(pbx);
            ImageBoxOption option = new ImageBoxOption();
            option.FromImageBox(pbx);
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
                optBackup.ToImageBox(pbx);
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
            option.ToImageBox(pbx);
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

    public class ImageBoxOption {
        // 화면 표시 옵션
        public bool UseDrawPixelValue { get; set; }
        public bool UseDrawCenterLine { get; set; }
        public bool UseDrawCursorInfo { get; set; }
        public bool UseDrawDebugInfo { get; set; }
        public bool UseDrawRoiRectangles { get; set; }
        public bool UseParallelToDraw { get; set; }

        public Color CenterLineColor { get; set; }
        public Color RoiRectangleColor { get; set; }
        public double FloatValueMax { get; set; }
        public int FloatValueDigit { get; set; }
        public EFont InfoFont { get; set; }

        public void FromImageBox(ImageBox pbx) {
            this.UseDrawPixelValue = pbx.UseDrawPixelValue;
            this.UseDrawCenterLine = pbx.UseDrawCenterLine;
            this.UseDrawCursorInfo = pbx.UseDrawCursorInfo;
            this.UseDrawDebugInfo = pbx.UseDrawDebugInfo;
            this.UseDrawRoiRectangles = pbx.UseDrawRoiRectangles;
            this.UseParallelToDraw = pbx.UseParallelToDraw;
            
            this.CenterLineColor = pbx.CenterLineColor;
            this.RoiRectangleColor = pbx.RoiRectangleColor;
            this.FloatValueMax = pbx.FloatValueMax;
            this.FloatValueDigit = pbx.FloatValueDigit;
            this.InfoFont = pbx.InfoFont;
        }

        public void ToImageBox(ImageBox pbx) {
            pbx.UseDrawPixelValue = this.UseDrawPixelValue;
            pbx.UseDrawCenterLine = this.UseDrawCenterLine;
            pbx.UseDrawCursorInfo = this.UseDrawCursorInfo;
            pbx.UseDrawDebugInfo = this.UseDrawDebugInfo;
            pbx.UseDrawRoiRectangles = this.UseDrawRoiRectangles;
            pbx.UseParallelToDraw = this.UseParallelToDraw;
            
            pbx.CenterLineColor = this.CenterLineColor;
            pbx.RoiRectangleColor = this.RoiRectangleColor;
            pbx.FloatValueMax = this.FloatValueMax;
            pbx.FloatValueDigit = this.FloatValueDigit;
            pbx.InfoFont = this.InfoFont;
        }
    }
}
