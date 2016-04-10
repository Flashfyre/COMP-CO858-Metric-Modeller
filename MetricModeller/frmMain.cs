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
using System.Diagnostics;

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
            int numOfPeople;
            int linesPerHour;
            int totalLines;
            double functionPoints;
            double manMonths;
            double cost;
            double salary;
            double languageAvg;

            double.TryParse(txtAvgSalary.Text,      out salary);
            int.TryParse(txtLinesPerHour.Text,      out linesPerHour);
            int.TryParse(txtNumOfPeople.Text,       out numOfPeople);

            languageAvg = 16;   //TOFIX: REPLACE 16 WITH ACTUAL LANGUAGE AVERAGE

            functionPoints = calculateFunctionPoints();

            totalLines = calculateTotalLines(functionPoints, languageAvg);

            manMonths = calculateManMonths(totalLines, linesPerHour, numOfPeople);

            cost = calculateCost(manMonths, salary);

            Debug.WriteLine("Function Points: " + functionPoints);
            Debug.WriteLine("Total Lines: " + totalLines);
            Debug.WriteLine("Man Months: " + manMonths);
            Debug.WriteLine("Cost: $" + cost);
        }

        private int calculateTotalLines(double functionPoints, double languageAvg)
        {
            return (int) (functionPoints * languageAvg);
        }

        private double calculateFunctionPoints()
        {
            return (calculateUFP() * calculateTCF());
        }

        private double calculateTCF()
        {
            int[] listOfFactors = new int[]
            {
                cbDataComm.SelectedIndex,
                cbDistributedData.SelectedIndex,
                cbPerformanceCriteria.SelectedIndex,
                cbHeavyHardwareUsage.SelectedIndex,
                cbHighTransactionRates.SelectedIndex,
                cbOnlineDataEntry.SelectedIndex,
                cbOnlineUpdating.SelectedIndex,
                cbComplexComputations.SelectedIndex,
                cbEaseOfInstallation.SelectedIndex,
                cbEaseOfOperation.SelectedIndex,
                cbPortability.SelectedIndex,
                cbMaintainability.SelectedIndex,
                cbEndUserEfficiency.SelectedIndex,
                cbReusability.SelectedIndex
            };

            double sum = 0;         

            foreach(int i in listOfFactors)
            {
                sum += i;
            }

            return 0.65 + (0.01 * sum);
        }

        private double calculateUFP()
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
            
            return (input * wInput) + (output * wOutput) + (inquiry * wInquiry) + (masterFiles * wMasterFiles) + (interfaces * wInterfaces);
        }

        private double calculateManMonths(double totalLines, int linesPerHour, int numOfPeople)
        {
            return totalLines / (linesPerHour * numOfPeople);
        }

        private double calculateCost(double manMonths, double salary)
        {
            return manMonths * (salary / 12);
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
