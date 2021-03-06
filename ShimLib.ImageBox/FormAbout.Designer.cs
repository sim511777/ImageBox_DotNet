﻿namespace ShimLib {
    partial class FormAbout {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnRoiListClear = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grdOption = new System.Windows.Forms.PropertyGrid();
            this.tbxVersion = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dlgSaveFile = new System.Windows.Forms.SaveFileDialog();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.lbxRoi = new System.Windows.Forms.ListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnRoiDelete = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCopy);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 330);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(429, 34);
            this.panel1.TabIndex = 0;
            // 
            // btnRoiListClear
            // 
            this.btnRoiListClear.Location = new System.Drawing.Point(113, 6);
            this.btnRoiListClear.Name = "btnRoiListClear";
            this.btnRoiListClear.Size = new System.Drawing.Size(102, 23);
            this.btnRoiListClear.TabIndex = 4;
            this.btnRoiListClear.Text = "Clear All";
            this.btnRoiListClear.UseVisualStyleBackColor = true;
            this.btnRoiListClear.Click += new System.EventHandler(this.btnRoiListClear_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(5, 6);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(79, 23);
            this.btnCopy.TabIndex = 3;
            this.btnCopy.Text = "Copy Buffer";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(349, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grdOption
            // 
            this.grdOption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdOption.HelpVisible = false;
            this.grdOption.Location = new System.Drawing.Point(3, 3);
            this.grdOption.Name = "grdOption";
            this.grdOption.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.grdOption.Size = new System.Drawing.Size(429, 327);
            this.grdOption.TabIndex = 1;
            this.grdOption.ToolbarVisible = false;
            this.grdOption.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.grdOption_PropertyValueChanged);
            // 
            // tbxVersion
            // 
            this.tbxVersion.BackColor = System.Drawing.SystemColors.Window;
            this.tbxVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbxVersion.Location = new System.Drawing.Point(3, 3);
            this.tbxVersion.Multiline = true;
            this.tbxVersion.Name = "tbxVersion";
            this.tbxVersion.ReadOnly = true;
            this.tbxVersion.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbxVersion.Size = new System.Drawing.Size(429, 361);
            this.tbxVersion.TabIndex = 3;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(443, 393);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.grdOption);
            this.tabPage2.Controls.Add(this.panel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(435, 367);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Option";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tbxVersion);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(435, 367);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Version";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dlgSaveFile
            // 
            this.dlgSaveFile.Filter = "Bmp File(*.bmp)|*.bmp";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.lbxRoi);
            this.tabPage3.Controls.Add(this.panel2);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(435, 367);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "ROI";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // lbxRoi
            // 
            this.lbxRoi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxRoi.FormattingEnabled = true;
            this.lbxRoi.ItemHeight = 12;
            this.lbxRoi.Location = new System.Drawing.Point(3, 3);
            this.lbxRoi.Name = "lbxRoi";
            this.lbxRoi.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbxRoi.Size = new System.Drawing.Size(429, 327);
            this.lbxRoi.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnRoiDelete);
            this.panel2.Controls.Add(this.btnRoiListClear);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(3, 330);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(429, 34);
            this.panel2.TabIndex = 1;
            // 
            // btnRoiDelete
            // 
            this.btnRoiDelete.Location = new System.Drawing.Point(5, 6);
            this.btnRoiDelete.Name = "btnRoiDelete";
            this.btnRoiDelete.Size = new System.Drawing.Size(102, 23);
            this.btnRoiDelete.TabIndex = 5;
            this.btnRoiDelete.Text = "Delete Selected";
            this.btnRoiDelete.UseVisualStyleBackColor = true;
            this.btnRoiDelete.Click += new System.EventHandler(this.btnRoiDelete_Click);
            // 
            // FormAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 393);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "FormAbout";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ImageBox for .NET";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormAbout_FormClosed);
            this.Load += new System.EventHandler(this.FormAbout_Load);
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PropertyGrid grdOption;
        private System.Windows.Forms.TextBox tbxVersion;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.SaveFileDialog dlgSaveFile;
        private System.Windows.Forms.Button btnRoiListClear;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ListBox lbxRoi;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnRoiDelete;
    }
}