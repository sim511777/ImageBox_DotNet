namespace ImageBox_Test {
    partial class FormMain {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent() {
            ShimLib.ImageBoxOption imageBoxOption1 = new ShimLib.ImageBoxOption();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblSystemFont = new System.Windows.Forms.Label();
            this.btnBitmapFont = new System.Windows.Forms.Button();
            this.btnLenna8ToGray16 = new System.Windows.Forms.Button();
            this.chkDrawingRepeatParallel = new System.Windows.Forms.CheckBox();
            this.btnFont = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbxDrawTest = new System.Windows.Forms.ListBox();
            this.btnLenna8ToFloat = new System.Windows.Forms.Button();
            this.cbxFont = new System.Windows.Forms.ComboBox();
            this.tbxExample = new System.Windows.Forms.TextBox();
            this.btnPaste = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnChess = new System.Windows.Forms.Button();
            this.btnWhiteBig = new System.Windows.Forms.Button();
            this.btnLenna32 = new System.Windows.Forms.Button();
            this.btnLenna24 = new System.Windows.Forms.Button();
            this.btnLenna8 = new System.Windows.Forms.Button();
            this.btnResetZoom = new System.Windows.Forms.Button();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.dlgFont = new System.Windows.Forms.FontDialog();
            this.imgBox = new ShimLib.ImageBox();
            this.dlgFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.btnSaveSampleImages = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnSaveSampleImages);
            this.panel1.Controls.Add(this.lblSystemFont);
            this.panel1.Controls.Add(this.btnBitmapFont);
            this.panel1.Controls.Add(this.btnLenna8ToGray16);
            this.panel1.Controls.Add(this.chkDrawingRepeatParallel);
            this.panel1.Controls.Add(this.btnFont);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.btnLenna8ToFloat);
            this.panel1.Controls.Add(this.cbxFont);
            this.panel1.Controls.Add(this.tbxExample);
            this.panel1.Controls.Add(this.btnPaste);
            this.panel1.Controls.Add(this.btnOpen);
            this.panel1.Controls.Add(this.btnChess);
            this.panel1.Controls.Add(this.btnWhiteBig);
            this.panel1.Controls.Add(this.btnLenna32);
            this.panel1.Controls.Add(this.btnLenna24);
            this.panel1.Controls.Add(this.btnLenna8);
            this.panel1.Controls.Add(this.btnResetZoom);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(793, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(286, 683);
            this.panel1.TabIndex = 1;
            // 
            // lblSystemFont
            // 
            this.lblSystemFont.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSystemFont.Location = new System.Drawing.Point(6, 603);
            this.lblSystemFont.Name = "lblSystemFont";
            this.lblSystemFont.Size = new System.Drawing.Size(268, 23);
            this.lblSystemFont.TabIndex = 18;
            this.lblSystemFont.Text = "label1";
            this.lblSystemFont.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnBitmapFont
            // 
            this.btnBitmapFont.Location = new System.Drawing.Point(6, 522);
            this.btnBitmapFont.Name = "btnBitmapFont";
            this.btnBitmapFont.Size = new System.Drawing.Size(84, 23);
            this.btnBitmapFont.TabIndex = 17;
            this.btnBitmapFont.Text = "Bitmap Font";
            this.btnBitmapFont.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBitmapFont.UseVisualStyleBackColor = true;
            this.btnBitmapFont.Click += new System.EventHandler(this.btnBitmapFont_Click);
            // 
            // btnLenna8ToGray16
            // 
            this.btnLenna8ToGray16.Location = new System.Drawing.Point(143, 99);
            this.btnLenna8ToGray16.Name = "btnLenna8ToGray16";
            this.btnLenna8ToGray16.Size = new System.Drawing.Size(131, 23);
            this.btnLenna8ToGray16.TabIndex = 16;
            this.btnLenna8ToGray16.Text = "Lenna_8 to gray16";
            this.btnLenna8ToGray16.UseVisualStyleBackColor = true;
            this.btnLenna8ToGray16.Click += new System.EventHandler(this.btnLenna8ToGray16_Click);
            // 
            // chkDrawingRepeatParallel
            // 
            this.chkDrawingRepeatParallel.AutoSize = true;
            this.chkDrawingRepeatParallel.Location = new System.Drawing.Point(12, 361);
            this.chkDrawingRepeatParallel.Name = "chkDrawingRepeatParallel";
            this.chkDrawingRepeatParallel.Size = new System.Drawing.Size(159, 16);
            this.chkDrawingRepeatParallel.TabIndex = 15;
            this.chkDrawingRepeatParallel.Text = "Drawing Repeat Parallel";
            this.chkDrawingRepeatParallel.UseVisualStyleBackColor = true;
            // 
            // btnFont
            // 
            this.btnFont.Location = new System.Drawing.Point(6, 577);
            this.btnFont.Name = "btnFont";
            this.btnFont.Size = new System.Drawing.Size(84, 23);
            this.btnFont.TabIndex = 14;
            this.btnFont.Text = "System Font";
            this.btnFont.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFont.UseVisualStyleBackColor = true;
            this.btnFont.Click += new System.EventHandler(this.btnFont_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbxDrawTest);
            this.groupBox1.Location = new System.Drawing.Point(6, 186);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 169);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Draw Test";
            // 
            // lbxDrawTest
            // 
            this.lbxDrawTest.FormattingEnabled = true;
            this.lbxDrawTest.ItemHeight = 12;
            this.lbxDrawTest.Items.AddRange(new object[] {
            "Not Draw",
            "Drawing Shapes",
            "Drawing Text",
            "Drawing Repeat",
            "Drawing All Charactors",
            "Graphics Shapes",
            "Graphics Text",
            "Graphics Repeat",
            "Graphics All Charactors"});
            this.lbxDrawTest.Location = new System.Drawing.Point(6, 20);
            this.lbxDrawTest.Name = "lbxDrawTest";
            this.lbxDrawTest.Size = new System.Drawing.Size(256, 136);
            this.lbxDrawTest.TabIndex = 0;
            this.lbxDrawTest.SelectedIndexChanged += new System.EventHandler(this.lbxDrawTest_SelectedIndexChanged);
            // 
            // btnLenna8ToFloat
            // 
            this.btnLenna8ToFloat.Location = new System.Drawing.Point(143, 70);
            this.btnLenna8ToFloat.Name = "btnLenna8ToFloat";
            this.btnLenna8ToFloat.Size = new System.Drawing.Size(131, 23);
            this.btnLenna8ToFloat.TabIndex = 10;
            this.btnLenna8ToFloat.Text = "Lenna_8 to float";
            this.btnLenna8ToFloat.UseVisualStyleBackColor = true;
            this.btnLenna8ToFloat.Click += new System.EventHandler(this.btnLenna8ToFloat_Click);
            // 
            // cbxFont
            // 
            this.cbxFont.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxFont.FormattingEnabled = true;
            this.cbxFont.Location = new System.Drawing.Point(6, 551);
            this.cbxFont.Name = "cbxFont";
            this.cbxFont.Size = new System.Drawing.Size(268, 20);
            this.cbxFont.TabIndex = 9;
            this.cbxFont.SelectedIndexChanged += new System.EventHandler(this.cbxFont_SelectedIndexChanged);
            // 
            // tbxExample
            // 
            this.tbxExample.Location = new System.Drawing.Point(6, 393);
            this.tbxExample.Multiline = true;
            this.tbxExample.Name = "tbxExample";
            this.tbxExample.Size = new System.Drawing.Size(268, 123);
            this.tbxExample.TabIndex = 7;
            this.tbxExample.Text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ\r\nabcdefghijklmnopqrstuvwxyz\r\n1234567890`-=[]\\;\',./\r\n~!" +
    "@#$%^&*()_+{}|:\"<>?\r\neng : hello, world!\r\nkor : 안녕, 세계!\r\nchn : 你好, 世界!\r\njpn : こん" +
    "にちは, せかい!";
            this.tbxExample.TextChanged += new System.EventHandler(this.tbxExample_TextChanged);
            // 
            // btnPaste
            // 
            this.btnPaste.Location = new System.Drawing.Point(143, 12);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(131, 23);
            this.btnPaste.TabIndex = 6;
            this.btnPaste.Text = "Paste from clipboard";
            this.btnPaste.UseVisualStyleBackColor = true;
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(6, 12);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(131, 23);
            this.btnOpen.TabIndex = 6;
            this.btnOpen.Text = "Open...";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnChess
            // 
            this.btnChess.Location = new System.Drawing.Point(143, 41);
            this.btnChess.Name = "btnChess";
            this.btnChess.Size = new System.Drawing.Size(131, 23);
            this.btnChess.TabIndex = 5;
            this.btnChess.Text = "Chess";
            this.btnChess.UseVisualStyleBackColor = true;
            this.btnChess.Click += new System.EventHandler(this.btnChess_Click);
            // 
            // btnWhiteBig
            // 
            this.btnWhiteBig.Location = new System.Drawing.Point(6, 128);
            this.btnWhiteBig.Name = "btnWhiteBig";
            this.btnWhiteBig.Size = new System.Drawing.Size(131, 23);
            this.btnWhiteBig.TabIndex = 4;
            this.btnWhiteBig.Text = "White Big";
            this.btnWhiteBig.UseVisualStyleBackColor = true;
            this.btnWhiteBig.Click += new System.EventHandler(this.btnWhiteBig_Click);
            // 
            // btnLenna32
            // 
            this.btnLenna32.Location = new System.Drawing.Point(6, 99);
            this.btnLenna32.Name = "btnLenna32";
            this.btnLenna32.Size = new System.Drawing.Size(131, 23);
            this.btnLenna32.TabIndex = 4;
            this.btnLenna32.Text = "Lenna_32";
            this.btnLenna32.UseVisualStyleBackColor = true;
            this.btnLenna32.Click += new System.EventHandler(this.btnLenna32_Click);
            // 
            // btnLenna24
            // 
            this.btnLenna24.Location = new System.Drawing.Point(6, 70);
            this.btnLenna24.Name = "btnLenna24";
            this.btnLenna24.Size = new System.Drawing.Size(131, 23);
            this.btnLenna24.TabIndex = 3;
            this.btnLenna24.Text = "Lenna_24";
            this.btnLenna24.UseVisualStyleBackColor = true;
            this.btnLenna24.Click += new System.EventHandler(this.btnLenna24_Click);
            // 
            // btnLenna8
            // 
            this.btnLenna8.Location = new System.Drawing.Point(6, 41);
            this.btnLenna8.Name = "btnLenna8";
            this.btnLenna8.Size = new System.Drawing.Size(131, 23);
            this.btnLenna8.TabIndex = 2;
            this.btnLenna8.Text = "Lenna_8";
            this.btnLenna8.UseVisualStyleBackColor = true;
            this.btnLenna8.Click += new System.EventHandler(this.btnLenna8_Click);
            // 
            // btnResetZoom
            // 
            this.btnResetZoom.Location = new System.Drawing.Point(6, 157);
            this.btnResetZoom.Name = "btnResetZoom";
            this.btnResetZoom.Size = new System.Drawing.Size(268, 23);
            this.btnResetZoom.TabIndex = 1;
            this.btnResetZoom.Text = "Reset Zoom";
            this.btnResetZoom.UseVisualStyleBackColor = true;
            this.btnResetZoom.Click += new System.EventHandler(this.btnResetZoom_Click);
            // 
            // dlgOpen
            // 
            this.dlgOpen.FileName = "openFileDialog1";
            this.dlgOpen.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG;*.TIF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIF";
            // 
            // dlgFont
            // 
            this.dlgFont.Font = new System.Drawing.Font("돋움체", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            // 
            // imgBox
            // 
            this.imgBox.AllowDrop = true;
            this.imgBox.BackColor = System.Drawing.Color.Gray;
            this.imgBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imgBox.Location = new System.Drawing.Point(0, 0);
            this.imgBox.Name = "imgBox";
            imageBoxOption1.CenterLineColor = System.Drawing.Color.Yellow;
            imageBoxOption1.FloatValueDigit = 3;
            imageBoxOption1.FloatValueMax = 1D;
            imageBoxOption1.InfoFont = ShimLib.EFont.unifont_13_0_06_bdf;
            imageBoxOption1.RoiRectangleColor = System.Drawing.Color.Blue;
            imageBoxOption1.TimeCheckCount = 100;
            imageBoxOption1.UseDrawCenterLine = true;
            imageBoxOption1.UseDrawCursorInfo = true;
            imageBoxOption1.UseDrawDebugInfo = true;
            imageBoxOption1.UseDrawPixelValue = true;
            imageBoxOption1.UseDrawRoiRectangles = true;
            imageBoxOption1.UseParallelToDraw = true;
            this.imgBox.Option = imageBoxOption1;
            this.imgBox.SzPan = new System.Drawing.Size(2, 2);
            this.imgBox.Size = new System.Drawing.Size(793, 683);
            this.imgBox.TabIndex = 2;
            this.imgBox.Text = "imageBox1";
            this.imgBox.ZoomLevel = 0;
            this.imgBox.PaintBackBuffer += new ShimLib.PaintBackbufferEventHandler(this.imgBox_PaintBackBuffer);
            this.imgBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.imgBox_DragDrop);
            this.imgBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.imgBox_DragEnter);
            this.imgBox.Paint += new System.Windows.Forms.PaintEventHandler(this.imageBox_Paint);
            // 
            // dlgFolder
            // 
            this.dlgFolder.ShowNewFolderButton = false;
            // 
            // btnSaveSampleImages
            // 
            this.btnSaveSampleImages.Location = new System.Drawing.Point(96, 522);
            this.btnSaveSampleImages.Name = "btnSaveSampleImages";
            this.btnSaveSampleImages.Size = new System.Drawing.Size(162, 23);
            this.btnSaveSampleImages.TabIndex = 19;
            this.btnSaveSampleImages.Text = "Save Font Sample Images";
            this.btnSaveSampleImages.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSaveSampleImages.UseVisualStyleBackColor = true;
            this.btnSaveSampleImages.Click += new System.EventHandler(this.btnSaveSampleImages_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1079, 683);
            this.Controls.Add(this.imgBox);
            this.Controls.Add(this.panel1);
            this.Name = "FormMain";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnResetZoom;
        private System.Windows.Forms.Button btnChess;
        private System.Windows.Forms.Button btnLenna32;
        private System.Windows.Forms.Button btnLenna24;
        private System.Windows.Forms.Button btnLenna8;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.OpenFileDialog dlgOpen;
        private System.Windows.Forms.TextBox tbxExample;
        private System.Windows.Forms.ComboBox cbxFont;
        private System.Windows.Forms.Button btnLenna8ToFloat;
        private System.Windows.Forms.Button btnPaste;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lbxDrawTest;
        private System.Windows.Forms.Button btnFont;
        private System.Windows.Forms.FontDialog dlgFont;
        private ShimLib.ImageBox imgBox;
        private System.Windows.Forms.CheckBox chkDrawingRepeatParallel;
        private System.Windows.Forms.Button btnLenna8ToGray16;
        private System.Windows.Forms.Label lblSystemFont;
        private System.Windows.Forms.Button btnBitmapFont;
        private System.Windows.Forms.FolderBrowserDialog dlgFolder;
        private System.Windows.Forms.Button btnWhiteBig;
        private System.Windows.Forms.Button btnSaveSampleImages;
    }
}

