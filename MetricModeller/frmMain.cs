using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetricModeller {

    public partial class frmMain : Form 
    {
        public frmMain() {
            InitializeComponent();
        }

        private readonly int[] listOfWeights_input = new int[]{3, 4, 6};


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            int[] listOfWeights_input = new int[] { 3, 4, 6 };

            int num = listOfWeights_input[comboBox1.SelectedIndex];
        }
    }
}
