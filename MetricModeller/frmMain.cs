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

    public partial class frmMain : Form {

        #region Samuel's Code
        private Dictionary<string, Tuple<decimal, int>> langData;

        private Dictionary<string, double[]> histData;

        private readonly int[][] weightingFactors = new int[][] {
            new int[] { 3, 4, 6 },
            new int[] { 4, 5, 7 },
            new int[] { 3, 4, 6 },
            new int[] { 7, 10, 15 },
            new int[] { 5, 7, 10 }
        };
        #endregion Samuel's Code

        private readonly double[][] projectComplexity = new double[][]
        {
            new double[] {2.4, 1.05, 2.5, 0.38},
            new double[] {3.0, 1.12, 2.5, 0.35},
            new double[] {3.6, 1.20, 2.5, 0.32}
        };

        #region Samuel's Code
        public frmMain(Dictionary<string, Tuple<decimal, int>> langData,
            Dictionary<string, double[]> histData) {
            this.langData = langData;
            this.histData = histData;
            InitializeComponent();
            populateLanguages();
        }
        #endregion Samuel's Code

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            calculateOutput(chkHistory.Checked);
        }

        private void calculateOutput(bool useHistory) {
            int totalLines;
            int numOfPeople = 6;
            double functionPoints;
            double cost;
            double salary;
            double languageAvg;
            double effort;
            double duration;
            double docDuration;
            double docCost;
            double teamFactor;

            int.TryParse(txtNumOfPeople.Text, out numOfPeople);
            teamFactor = calculateTeamCohesion(numOfPeople) / numOfPeople;
            salary = getSalaryForTeam(numOfPeople, 32D);

            try {

                if (cbLang.SelectedIndex == -1)
                    return;
                
                languageAvg = langData[cbLang.SelectedItem.ToString()].Item2;

                functionPoints = calculateFunctionPoints();

                totalLines = calculateTotalLines(functionPoints, languageAvg);

                effort = calculateEffort(totalLines);

                cost = calculateCost(effort, salary);

                duration = getDurationForTeam(numOfPeople, calculateDuration(effort, numOfPeople));

                docCost = calculateDocCost();

                docDuration = calculateDocDuration();

                #region Samuel's Code
                if (useHistory) {
                    if (histData.ContainsKey(cbLang.Text)) {
                        double[] data = histData[cbLang.Text];
                        int histTotalLines = (int)data[5],
                        histNumOfPeople = (int)data[0],
                        histFunctionPoints = (int) data[1];
                        double histDuration = data[2],
                        histCost = data[3],
                        histEffort = data[4],
                        linesOfCodeProportion =
                            (totalLines / functionPoints) / (histTotalLines / histFunctionPoints),
                        durationProportionA = (functionPoints / duration / numOfPeople),
                        durationProportionB = (histFunctionPoints / histDuration / histNumOfPeople),
                        durationProportionDiff = (Math.Abs(durationProportionA - durationProportionB)),
                        durationProportion =
                            (durationProportionDiff + (durationProportionA / durationProportionB)) /
                            (durationProportionDiff + 1),
                        costProportion =
                            (cost / functionPoints) / (histCost / histFunctionPoints),
                        effortProportion =
                            (effort / functionPoints) / (histEffort / histFunctionPoints);
                        functionPoints = (int) ((functionPoints * 9) + (int)(functionPoints * ((functionPoints / numOfPeople) /
                            (histFunctionPoints / histNumOfPeople)))) / 10;
                        totalLines = (int)((double)totalLines * linesOfCodeProportion);
                        duration *= durationProportion;
                        cost *= costProportion;
                        effort *= effortProportion;
                    } else
                        MessageBox.Show(String.Format(
                            "Error: The selected language '%s' does not exist in the history file.", cbLang.Text));
                }
                MessageBox.Show(
                    "DEVELOPMENT\n" +
                    "Function Points: " + functionPoints + "\n" +
                    "Total Lines: " + totalLines + "\n" +
                    "Cost: $" + Math.Round(cost, 2) + "\n" +
                    "Effort: " + Math.Round(effort, 2) + " person-months\n" +
                    "Duration: " + Math.Round(duration, 2) + " month(s)\n\n" +
                    "DOCUMENTATION\n" +
                    "Documentation Duration: " + docDuration + " month(s)\n" +
                    "Documentation Cost: $" + docCost + "\n");
                #endregion Samuel's Code
            } catch (IndexOutOfRangeException e) {
            }
        }

        private double calculateEffort(int totalLines)
        {
            // Previous formula: ab * (KLOC) * bb
            // Current formula: a * kloc^b
            return projectComplexity[cbComplexity.SelectedIndex][0] *
                Math.Pow((totalLines * 0.001), projectComplexity[cbComplexity.SelectedIndex][1]);
        }

        private double calculateDuration(double effort, int numOfPeople)
        {
            // Previous formula: cb * (effort) & db
            // Current formula: 2.5 * effort^EX
            /*projectComplexity[cbComplexity.SelectedIndex][0] * (effort) *
              projectComplexity[cbComplexity.SelectedIndex][1];*/
            return (projectComplexity[cbComplexity.SelectedIndex][2] *
                Math.Pow(effort, projectComplexity[cbComplexity.SelectedIndex][3])) /
                (calculateTeamCohesion(numOfPeople));
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
            double[] listOfFactors = new double[]
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

            if (checkHighlyModular.Checked)
            {
                if(cbReusability.SelectedIndex == 0)
                {
                    if(listOfFactors[10] != 5)
                    {
                        //Bumps Portability up by 1
                        listOfFactors[10] += 1;
                    }

                    if(listOfFactors[11] != 5)
                    {
                        //Bumps Maintainability up by 1
                        listOfFactors[11] += 1;
                    }
                }
                else
                {
                    if ((0.5 * cbReusability.SelectedIndex + listOfFactors[10]) < 5)
                        listOfFactors[10] += (0.5 * cbReusability.SelectedIndex);
                    else
                        listOfFactors[10] = 5;

                    if ((0.5 * cbReusability.SelectedIndex + listOfFactors[11]) < 5)
                        listOfFactors[11] += (0.5 * cbReusability.SelectedIndex);

                    else
                        listOfFactors[11] = 5;
                }
                

                if (checkUnusedCode.Checked)
                {
                    if(cbReusability.SelectedIndex == 0)
                    {
                        //Bumps Complex Computations up by 1
                        listOfFactors[7] += 1;
                    }
                    else
                    {
                        if ((0.5 * cbReusability.SelectedIndex + listOfFactors[7]) < 5)
                            listOfFactors[7] += (0.5 * cbReusability.SelectedIndex);
                        else
                            listOfFactors[7] = 5;
                    }
                    
                }

                if (checkModuleTesting.Checked)
                {
                    if(cbReusability.SelectedIndex == 0)
                    {
                        listOfFactors[11] += 1;
                    }
                    else
                    {
                        //Bumps Maintainability up again by 1
                        listOfFactors[11] += 1 * (0.5 * cbReusability.SelectedIndex);
                    }
                    
                }
            }
                
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
            string coheSelect = cbTeamCohesion.Text;
            double cohesionVal = 0.75;
            switch (coheSelect)
            {
                case "No Past Experience":
                    cohesionVal = 0.6;
                    break;
                case "Some Team Experience":
                    cohesionVal = 0.75;
                    break;
                case "Experienced Team":
                    cohesionVal = 0.9;
                    break;
            }
            return numOfPeople * cohesionVal;
        }

        private double calculateDocDuration()
        {
           double pages = Convert.ToDouble(txtDocPages.Text);
           double writers = Convert.ToInt32(txtWriters.Text); ;

           double dur = (pages * 3) / writers;
            return Math.Round(dur/120, 2);
        }

        private double calculateDocCost()
        {
            int pages = Convert.ToInt32(txtDocPages.Text);
            int wage = 22;

            int dur = (pages * 3);
            int cost = dur * wage;
            return cost;
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

        #region Samuel's Code
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
        #endregion Samuel's Code

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void cbInput_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbOutput_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #region Samuel's Code
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
            cbLang.SelectedIndex = 1;
            txtInput.Text = "10";
            cbInput.SelectedIndex = 0;
            txtOutput.Text = "10";
            cbOutput.SelectedIndex = 0;
            txtInquiry.Text = "10";
            cbInquiry.SelectedIndex = 0;
            txtMasterFiles.Text = "10";
            cbMasterFiles.SelectedIndex = 0;
            txtInterfaces.Text = "10";
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
            cbTeamCohesion.SelectedIndex = 1;
            txtDocPages.Text = "80";
            txtWriters.Text = "4";
        }
        #endregion Samuel's Code

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
            else
            {
                txtIntermediateExpert.Visible = false;
                txtStudentsEntry.Visible = false;
                lblIntermediateExpert.Visible = false;
                lblPercent1.Visible = false;
                lblPercent2.Visible = false;
                lblStudentsEntry.Visible = false;
            }

        }

        private double getDurationForTeam(int numOfPeople, double duration)
        {
            int student_Entry_Factor_percent, intermediate_Expert_Factor_percent;
            if (chkExperienceFactor.Checked)
            {
                int.TryParse(txtStudentsEntry.Text, out student_Entry_Factor_percent);
                int.TryParse(txtIntermediateExpert.Text, out intermediate_Expert_Factor_percent);

                int numberOfStudents = (numOfPeople * student_Entry_Factor_percent) / 100;
                int numberOfExpert = (numOfPeople * intermediate_Expert_Factor_percent) / 100;

                double studentDuration = (duration * 1.25 * student_Entry_Factor_percent / 100);
                double expertDuration = (duration * 0.75 * intermediate_Expert_Factor_percent / 100);

                double totalDuration = studentDuration + expertDuration;

                return totalDuration;
            }

            return duration;
        }

        private double getSalaryForTeam(int numOfPeople, double salary) {
            int student_Entry_Factor_percent, intermediate_Expert_Factor_percent;
            if (chkExperienceFactor.Checked) {
                int.TryParse(txtStudentsEntry.Text, out student_Entry_Factor_percent);
                int.TryParse(txtIntermediateExpert.Text, out intermediate_Expert_Factor_percent);

                int numberOfStudents = (numOfPeople * student_Entry_Factor_percent) / 100;
                int numberOfExpert = (numOfPeople * intermediate_Expert_Factor_percent) / 100;

                double studentSalary = (numberOfStudents * 24);
                double expertSalary = (numberOfExpert * 40);
                double averageSalary = (studentSalary + expertSalary) / numOfPeople;

                return averageSalary;
            }

            return salary;
        }

        private void checkHighlyModular_CheckedChanged(object sender, EventArgs e)
        {
            if(checkHighlyModular.Checked)
            {
                checkModuleTesting.Visible = true;
                checkUnusedCode.Visible = true;
            }
            else
            {
                checkModuleTesting.Visible = false;
                checkUnusedCode.Visible = false;
            }
        }
    }
}
