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

            //double.TryParse(txtAvgSalary.Text, out salary);
            salary = 32D;
            int.TryParse(txtLinesPerHour.Text, out linesPerHour);
            int.TryParse(txtNumOfPeople.Text, out numOfPeople);

            languageAvg = langData[cbLang.SelectedItem.ToString()].Item2;

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
            // Previous formula: ab * (KLOC) * bb
            // Current formula: a * kloc^b
            return projectComplexity[cbComplexity.SelectedIndex][0] *
                Math.Pow((totalLines * 0.001), projectComplexity[cbComplexity.SelectedIndex][1]);
        }

        private double calculateDuration(double effort)
        {
            // Previous formula: cb * (effort) & db
            // Current formula: 2.5 * effort^EX
            /*projectComplexity[cbComplexity.SelectedIndex][0] * (effort) *
              projectComplexity[cbComplexity.SelectedIndex][1];*/
            return projectComplexity[cbComplexity.SelectedIndex][2] *
                Math.Pow(effort, projectComplexity[cbComplexity.SelectedIndex][3]);
        }

        private int calculateTotalLines(double functionPoints, double languageAvg)
        {
            return (int)(functionPoints * languageAvg);
        }

        private double calculateFunctionPoints()
        {
            return (calculateUFP() * calculateTCF()) * getFrameworkProductivityMultiplier();
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

        private double calculateCost(double effort, double salary)
        {
            //manMonths * (salary / 12)
            return effort * (salary * 120);
        }

        private double getFrameworkProductivityMultiplier() {
            double multiplier = 1.0D;

            if (chkFramework.Checked) {
                if (chkFrameworkPercentage.Checked)
                    multiplier -= (trkFramework.Value * 0.025D);
                else {
                    switch (cbFramework.SelectedIndex) {
                        case 1:
                            multiplier -= 0.075D;
                            break;
                        case 2:
                            multiplier -= 0.125D;
                            break;
                        case 3:
                            multiplier -= 0.175D;
                            break;
                        case 4:
                            multiplier -= 0.25D;
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

        private void btnTest_Click(object sender, EventArgs e) {
            cbLang.SelectedIndex = 70;
            txtInput.Text = "2";
            cbInput.SelectedIndex = 0;
            txtOutput.Text = "2";
            cbOutput.SelectedIndex = 0;
            txtInquiry.Text = "2";
            cbInquiry.SelectedIndex = 0;
            txtMasterFiles.Text = "2";
            cbMasterFiles.SelectedIndex = 0;
            txtInterfaces.Text = "2";
            cbInterfaces.SelectedIndex = 0;
            cbDataComm.SelectedIndex = 0;
            cbDistributedData.SelectedIndex = 0;
            cbPerformanceCriteria.SelectedIndex = 0;
            cbHeavyHardwareUsage.SelectedIndex = 0;
            cbHighTransactionRates.SelectedIndex = 0;
            cbOnlineDataEntry.SelectedIndex = 0;
            cbOnlineUpdating.SelectedIndex = 0;
            cbComplexComputations.SelectedIndex = 0;
            cbEaseOfInstallation.SelectedIndex = 0;
            cbEaseOfOperation.SelectedIndex = 0;
            cbPortability.SelectedIndex = 0;
            cbMaintainability.SelectedIndex = 0;
            cbEndUserEfficiency.SelectedIndex = 0;
            cbReusability.SelectedIndex = 0;
            cbComplexity.SelectedIndex = 0;
            txtNumOfPeople.Text = "6";
            cbTeamCohesion.SelectedIndex = 0;
        }


        
        private void chkExperienceFactor_CheckedChanged(object sender, EventArgs e)
        {
            
            if (chkExperienceFactor.Checked)
            {
                txtIntermediateExpert.Visible = true;
                txtStudentsEntry.Visible = true;
                lblIntermediateExpert.Visible = true;
                lblPercent1.Visible = true;
                lblPercent2.Visible = true;
                lblStudentsEntry.Visible = true;                    
            }
           
            
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}
