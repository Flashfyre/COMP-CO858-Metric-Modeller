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
            populateLanguages();
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            /*
            int weightInput = weightingFactors[0][cbInput.SelectedIndex],
                weightOutput = weightingFactors[1][cbOutput.SelectedIndex],
                weightInquiry = weightingFactors[2][cbInquiry.SelectedIndex],
                weightMasterFiles = weightingFactors[3][cbMasterFiles.SelectedIndex],
                weightInterfaces = weightingFactors[4][cbInterfaces.SelectedIndex];
            */

            int numOfPeople;
            double totalLines;
            double linesPerHour;
            double manMonths;
            double cost;
            double salary;
            double functionPoints;

            double.TryParse(txtAvgSalary.Text,      out salary);
            double.TryParse(txtLinesPerHour.Text,   out linesPerHour);
            int.TryParse(txtNumOfPeople.Text,       out numOfPeople);

            functionPoints = calculateFunctionPoints();

            lblFP.Text = functionPoints.ToString();
            
            totalLines = calculateTotalLines(functionPoints);

            manMonths = calculateManMonths(totalLines, linesPerHour, numOfPeople);

            cost = calculateCost(manMonths, salary);
            
        }

        private double calculateTotalLines(double functionPoints)
        {
            //FP * language average
            return 0;
        }

        private int calculateFunctionPoints()
        {
            return (int) calculateTCF() * calculateUFP();
        }

        private double calculateTCF()
        {
            //0.65 + (.01 * Sum of 14 technical complexity factors)

            int dylan, gururaj, sam, anuj, terry;
            dylan = trackBar1.Value;
            

            //return 0.65 + (0.01 * (dylan + gururaj + anuj + sam + terry));

            return 0;
        }

        private int calculateUFP()
        {
            int input, wInput, output, wOutput, inquiry, wInquiry, masterFiles, wMasterFiles, interfaces, wInterfaces;

            int.TryParse(txtInput.Text,          out input);
            int.TryParse(txtOutput.Text,         out output);
            int.TryParse(txtInquiry.Text,        out inquiry);
            int.TryParse(txtMasterFiles.Text,    out masterFiles);
            int.TryParse(txtInterfaces.Text,     out interfaces);

            wInput = weightingFactors[0][cbInput.SelectedIndex];
            wInput = weightingFactors[0][cbInput.SelectedIndex];
            wOutput = weightingFactors[1][cbOutput.SelectedIndex];
            wInquiry = weightingFactors[2][cbInquiry.SelectedIndex];
            wMasterFiles = weightingFactors[3][cbMasterFiles.SelectedIndex];
            wInterfaces = weightingFactors[4][cbInterfaces.SelectedIndex];
            
            return (input * wInput) + (output * wOutput) + (inquiry * wInquiry) + (interfaces * wInterfaces);
        }

        private double calculateManMonths(double totalLines, double linesPerHour, int numOfPeople)
        {
            return totalLines / (linesPerHour * numOfPeople);
        }

        private double calculateCost(double manMonths, double salary)
        {
            return manMonths * (salary / 12);
        }

        private void populateLanguages() {
            Dictionary<string, Tuple<decimal, int>>.Enumerator langEnum = langData.GetEnumerator();
           
            while (langEnum.MoveNext()) {
                KeyValuePair<string, Tuple<decimal, int>> curLang = langEnum.Current;
                cbLang.Items.Add(curLang.Key);
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void cbInput_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbOutput_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        
    }
}
