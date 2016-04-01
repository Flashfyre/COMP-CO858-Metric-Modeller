using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetricModeller {

    public partial class frmMain : Form 
    {
        private Dictionary<string, Tuple<decimal, int>> langData;
        private readonly int[][] weightingFactors = new int[][] {
            new int[] { 3, 4, 6 },
            new int[] { 4, 5, 7 },
            new int[] { 3, 4, 6 },
            new int[] { 7, 10, 15 },
            new int[] { 5, 7, 10 }
        };
        
        public frmMain(Dictionary<string, Tuple<decimal, int>> langData) {
            this.langData = langData;
            InitializeComponent();
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            int weightInput = weightingFactors[0][cbInput.SelectedIndex],
                weightOutput = weightingFactors[1][cbOutput.SelectedIndex],
                weightInquiry = weightingFactors[2][cbInquiry.SelectedIndex],
                weightMasterFiles = weightingFactors[3][cbMasterFiles.SelectedIndex],
                weightInterfaces = weightingFactors[4][cbInterfaces.SelectedIndex];
        }
    }
}
