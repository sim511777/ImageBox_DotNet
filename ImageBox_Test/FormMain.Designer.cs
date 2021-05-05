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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnUnifontSampleHex = new System.Windows.Forms.Button();
            this.btnUnifontHex = new System.Windows.Forms.Button();
            this.btnLenna8ToFloat = new System.Windows.Forms.Button();
            this.cbxFont = new System.Windows.Forms.ComboBox();
            this.tbxExample = new System.Windows.Forms.TextBox();
            this.btnPaste = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnChess = new System.Windows.Forms.Button();
            this.btnLenna32 = new System.Windows.Forms.Button();
            this.btnLenna24 = new System.Windows.Forms.Button();
            this.btnLenna8 = new System.Windows.Forms.Button();
            this.btnResetZoom = new System.Windows.Forms.Button();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.imgBox = new ShimLib.ImageBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnUnifontSampleHex);
            this.panel1.Controls.Add(this.btnUnifontHex);
            this.panel1.Controls.Add(this.btnLenna8ToFloat);
            this.panel1.Controls.Add(this.cbxFont);
            this.panel1.Controls.Add(this.tbxExample);
            this.panel1.Controls.Add(this.btnPaste);
            this.panel1.Controls.Add(this.btnOpen);
            this.panel1.Controls.Add(this.btnChess);
            this.panel1.Controls.Add(this.btnLenna32);
            this.panel1.Controls.Add(this.btnLenna24);
            this.panel1.Controls.Add(this.btnLenna8);
            this.panel1.Controls.Add(this.btnResetZoom);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(709, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(178, 568);
            this.panel1.TabIndex = 1;
            // 
            // btnUnifontSampleHex
            // 
            this.btnUnifontSampleHex.Location = new System.Drawing.Point(6, 282);
            this.btnUnifontSampleHex.Name = "btnUnifontSampleHex";
            this.btnUnifontSampleHex.Size = new System.Drawing.Size(164, 23);
            this.btnUnifontSampleHex.TabIndex = 12;
            this.btnUnifontSampleHex.Text = "Unifont Sample hex";
            this.btnUnifontSampleHex.UseVisualStyleBackColor = true;
            this.btnUnifontSampleHex.Click += new System.EventHandler(this.btnUnifontSampleHex_Click);
            // 
            // btnUnifontHex
            // 
            this.btnUnifontHex.Location = new System.Drawing.Point(6, 253);
            this.btnUnifontHex.Name = "btnUnifontHex";
            this.btnUnifontHex.Size = new System.Drawing.Size(164, 23);
            this.btnUnifontHex.TabIndex = 11;
            this.btnUnifontHex.Text = "Unifont hex";
            this.btnUnifontHex.UseVisualStyleBackColor = true;
            this.btnUnifontHex.Click += new System.EventHandler(this.btnUnifontHex_Click);
            // 
            // btnLenna8ToFloat
            // 
            this.btnLenna8ToFloat.Location = new System.Drawing.Point(6, 224);
            this.btnLenna8ToFloat.Name = "btnLenna8ToFloat";
            this.btnLenna8ToFloat.Size = new System.Drawing.Size(164, 23);
            this.btnLenna8ToFloat.TabIndex = 10;
            this.btnLenna8ToFloat.Text = "Lenna_8 to float";
            this.btnLenna8ToFloat.UseVisualStyleBackColor = true;
            this.btnLenna8ToFloat.Click += new System.EventHandler(this.btnLenna8ToFloat_Click);
            // 
            // cbxFont
            // 
            this.cbxFont.DisplayMember = "Item1";
            this.cbxFont.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxFont.FormattingEnabled = true;
            this.cbxFont.Location = new System.Drawing.Point(6, 459);
            this.cbxFont.Name = "cbxFont";
            this.cbxFont.Size = new System.Drawing.Size(164, 20);
            this.cbxFont.TabIndex = 9;
            this.cbxFont.ValueMember = "Item2";
            this.cbxFont.SelectedIndexChanged += new System.EventHandler(this.cbxFont_SelectedIndexChanged);
            // 
            // tbxExample
            // 
            this.tbxExample.Location = new System.Drawing.Point(6, 330);
            this.tbxExample.Multiline = true;
            this.tbxExample.Name = "tbxExample";
            this.tbxExample.Size = new System.Drawing.Size(164, 123);
            this.tbxExample.TabIndex = 7;
            this.tbxExample.Text = "Hello(안녕),\r\nWorlld(세상)";
            this.tbxExample.TextChanged += new System.EventHandler(this.tbxExample_TextChanged);
            // 
            // btnPaste
            // 
            this.btnPaste.Location = new System.Drawing.Point(6, 79);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(164, 23);
            this.btnPaste.TabIndex = 6;
            this.btnPaste.Text = "Past from clipboard";
            this.btnPaste.UseVisualStyleBackColor = true;
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(6, 50);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(164, 23);
            this.btnOpen.TabIndex = 6;
            this.btnOpen.Text = "Open...";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnChess
            // 
            this.btnChess.Location = new System.Drawing.Point(6, 195);
            this.btnChess.Name = "btnChess";
            this.btnChess.Size = new System.Drawing.Size(164, 23);
            this.btnChess.TabIndex = 5;
            this.btnChess.Text = "Chess";
            this.btnChess.UseVisualStyleBackColor = true;
            this.btnChess.Click += new System.EventHandler(this.btnChess_Click);
            // 
            // btnLenna32
            // 
            this.btnLenna32.Location = new System.Drawing.Point(6, 166);
            this.btnLenna32.Name = "btnLenna32";
            this.btnLenna32.Size = new System.Drawing.Size(164, 23);
            this.btnLenna32.TabIndex = 4;
            this.btnLenna32.Text = "Lenna_32";
            this.btnLenna32.UseVisualStyleBackColor = true;
            this.btnLenna32.Click += new System.EventHandler(this.btnLenna32_Click);
            // 
            // btnLenna24
            // 
            this.btnLenna24.Location = new System.Drawing.Point(6, 137);
            this.btnLenna24.Name = "btnLenna24";
            this.btnLenna24.Size = new System.Drawing.Size(164, 23);
            this.btnLenna24.TabIndex = 3;
            this.btnLenna24.Text = "Lenna_24";
            this.btnLenna24.UseVisualStyleBackColor = true;
            this.btnLenna24.Click += new System.EventHandler(this.btnLenna24_Click);
            // 
            // btnLenna8
            // 
            this.btnLenna8.Location = new System.Drawing.Point(6, 108);
            this.btnLenna8.Name = "btnLenna8";
            this.btnLenna8.Size = new System.Drawing.Size(164, 23);
            this.btnLenna8.TabIndex = 2;
            this.btnLenna8.Text = "Lenna_8";
            this.btnLenna8.UseVisualStyleBackColor = true;
            this.btnLenna8.Click += new System.EventHandler(this.btnLenna8_Click);
            // 
            // btnResetZoom
            // 
            this.btnResetZoom.Location = new System.Drawing.Point(6, 12);
            this.btnResetZoom.Name = "btnResetZoom";
            this.btnResetZoom.Size = new System.Drawing.Size(164, 23);
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
            // imgBox
            // 
            this.imgBox.BackColor = System.Drawing.Color.Gray;
            this.imgBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imgBox.FloatValueMax = 1D;
            this.imgBox.Location = new System.Drawing.Point(0, 0);
            this.imgBox.Name = "imgBox";
            this.imgBox.Size = new System.Drawing.Size(709, 568);
            this.imgBox.TabIndex = 2;
            this.imgBox.Text = "imageBox1";
            this.imgBox.UseDrawCenterLine = true;
            this.imgBox.UseDrawCursorInfo = true;
            this.imgBox.UseDrawDebugInfo = true;
            this.imgBox.UseDrawPixelValue = true;
            this.imgBox.PaintBackBuffer += new ShimLib.PaintBackbufferEventHandler(this.imgBox_PaintBackBuffer);
            this.imgBox.Paint += new System.Windows.Forms.PaintEventHandler(this.imageBox_Paint);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(887, 568);
            this.Controls.Add(this.imgBox);
            this.Controls.Add(this.panel1);
            this.Name = "FormMain";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private ShimLib.ImageBox imgBox;
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
        private System.Windows.Forms.Button btnUnifontSampleHex;
        private System.Windows.Forms.Button btnUnifontHex;
        private System.Windows.Forms.Button btnPaste;
    }
}

