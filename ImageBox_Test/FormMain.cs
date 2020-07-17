using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageBox_Test {
    public partial class FormMain : Form {
        public FormMain() {
            InitializeComponent();
        }

        private void imageBox_Paint(object sender, PaintEventArgs e) {
            e.Graphics.DrawLine(Pens.Red, 0, 0, 100, 100);
        }
    }
}
