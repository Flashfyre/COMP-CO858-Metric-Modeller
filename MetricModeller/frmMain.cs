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
        private readonly double[][] projectComplexity = new double[][]
        {
            new double[] {2.4, 1.05, 2.5, 0.38},
            new double[] {3.0, 1.12, 2.5, 0.35},
            new double[] {3.6, 1.20, 2.5, 0.32}
        };

        public frmMain(Dictionary<string, Tuple<decimal, int>> langData) {
            this.langData = langData;
            InitializeComponent();
            populateLanguages();
        }
        //Anuj
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
            double effort;
            double duration;

            double.TryParse(txtAvgSalary.Text, out salary);
            int.TryParse(txtLinesPerHour.Text, out linesPerHour);
            int.TryParse(txtNumOfPeople.Text, out numOfPeople);

            languageAvg = 16;   //TOFIX: REPLACE 16 WITH ACTUAL LANGUAGE AVERAGE

            functionPoints = calculateFunctionPoints();

            totalLines = calculateTotalLines(functionPoints, languageAvg);

            manMonths = calculateManMonths(totalLines, linesPerHour, numOfPeople);

            cost = calculateCost(manMonths, salary);

            effort = calculateEffort(totalLines);

            duration = calculateDuration(effort);

            Debug.WriteLine("Function Points: " + functionPoints);
            Debug.WriteLine("Total Lines: " + totalLines);
            Debug.WriteLine("Man Months: " + manMonths);
            Debug.WriteLine("Cost: $" + cost);
        }

        private double calculateEffort(int totalLines)
        {
            //ab * (KLOC) * bb
            return projectComplexity[cbComplexity.SelectedIndex][0] * (totalLines * 1000) *
                projectComplexity[cbComplexity.SelectedIndex][1] * getFrameworkProductivityMultiplier();
        }

        private double calculateDuration(double effort)
        {
            //cb * (effort) & db
            return projectComplexity[cbComplexity.SelectedIndex][0] * (effort) * projectComplexity[cbComplexity.SelectedIndex][1];
        }

        private int calculateTotalLines(double functionPoints, double languageAvg)
        {
            return (int)(functionPoints * languageAvg);
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

            foreach (int i in listOfFactors)
            {
                sum += i;
            }

            return 0.65 + (0.01 * sum);
        }

        private double calculateUFP()
        {
            int input, wInput, output, wOutput, inquiry, wInquiry, masterFiles, wMasterFiles, interfaces, wInterfaces;

            int.TryParse(txtInput.Text, out input);
            int.TryParse(txtOutput.Text, out output);
            int.TryParse(txtInquiry.Text, out inquiry);
            int.TryParse(txtMasterFiles.Text, out masterFiles);
            int.TryParse(txtInterfaces.Text, out interfaces);

            wInput = weightingFactors[0][cbInput.SelectedIndex];
            wInput = weightingFactors[0][cbInput.SelectedIndex];
            wOutput = weightingFactors[1][cbOutput.SelectedIndex];
            wInquiry = weightingFactors[2][cbInquiry.SelectedIndex];
            wMasterFiles = weightingFactors[3][cbMasterFiles.SelectedIndex];
            wInterfaces = weightingFactors[4][cbInterfaces.SelectedIndex];

            return (input * wInput) + (output * wOutput) + (inquiry * wInquiry) + (masterFiles * wMasterFiles) + (interfaces * wInterfaces);
        }
        private double calculateTeamCohesion(int numOfPeople)
        {
            string coheSelect = cbTeamCohesion.SelectedText;
            int cohesionVal = 2;
            switch (coheSelect)
            {
                case "No Past Experience":
                    cohesionVal = 1;
                    break;
                case "Some Team Experience":
                    cohesionVal = 2;
                    break;
                case "Experienced Team":
                    cohesionVal = 3;
                    break;
            }
            return numOfPeople / 2 * cohesionVal;
        }

        private double calculateManMonths(double totalLines, int linesPerHour, int numOfPeople)
        {
            return totalLines / (linesPerHour * calculateTeamCohesion(numOfPeople));
        }

        private double calculateCost(double manMonths, double salary)
        {
            return manMonths * (salary / 12);
        }

        private double getFrameworkProductivityMultiplier() {
            double multiplier = 1.0D;

            if (chkFramework.Checked) {
                if (chkFrameworkPercentage.Checked)
                    multiplier += (trkFramework.Value * 0.05D);
                else {
                    switch (cbFramework.SelectedIndex) {
                        case 1:
                            multiplier += 0.15D;
                            break;
                        case 2:
                            multiplier += 0.25D;
                            break;
                        case 3:
                            multiplier += 0.35D;
                            break;
                        case 4:
                            multiplier += 0.5D;
                            break;
                    }
                }
            }

            return multiplier;
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

        private void chkFramework_CheckedChanged(object sender, EventArgs e) {
            bool isChecked = chkFramework.Checked;
            cbFramework.Enabled = isChecked;
            chkFrameworkPercentage.Enabled = isChecked;
            trkFramework.Enabled = isChecked;
        }

        private void chkFrameworkPercentage_CheckedChanged(object sender, EventArgs e) {
            bool isChecked = chkFrameworkPercentage.Checked;
            cbFramework.Visible = !isChecked;
            lblFrameworkPercentage.Visible = isChecked;
            trkFramework.Visible = isChecked;
            lblFrameworkPercentageScale.Visible = isChecked;
        }
    }
}
