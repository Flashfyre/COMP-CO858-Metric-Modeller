using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetricModeller {
    static class Program {

        private const string langFilePath = "../../../language_prod.csv";
        private const string histFilePath = "../../../history.csv";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Dictionary<string, Tuple<decimal, int>> langData;
            Dictionary<string, double[]> histData;
            if (TryReadFiles(out langData, out histData)) {
                int n = langData.Count;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain(langData, histData));
            }
        }

        private static bool TryReadFiles(out Dictionary<string, Tuple<decimal, int>> langData,
            out Dictionary<string, double[]> histData) {

            langData = new Dictionary<string, Tuple<decimal, int>>();
            histData = new Dictionary<string, double[]>();

            try {

                bool isHeaderLine = true;

                foreach (String curLine in File.ReadAllLines(langFilePath)) {
                    string[] values = curLine.Split(',');
                    if (isHeaderLine)
                        isHeaderLine = false;
                    else {
                        while (values.Length > 3)
                            values = new string[] { values[0] + values[1], values[2], values[3] };
                        langData.Add(values[0], new Tuple<decimal, int>(decimal.Parse(values[1]),
                            int.Parse(values[2])));
                    }
                }

                isHeaderLine = true;

                foreach (String curLine in File.ReadAllLines(histFilePath)) {
                    string[] values = curLine.Split(',');
                    if (isHeaderLine)
                        isHeaderLine = false;
                    else {
                        while (values.Length > 7)
                            values = new string[] { values[0] + values[1], values[2], values[3],
                                values[4], values[5], values[6] };
                        histData.Add(values[0], new double[] { double.Parse(values[1]),
                            double.Parse(values[2]), double.Parse(values[3]),
                            double.Parse(values[4]), double.Parse(values[5]),
                            double.Parse(values[6]) });
                    }
                }

                return true;

            } catch (IOException e) {
                // File not found -> Display an error and cancel the read-in       
                MessageBox.Show("Error: One or more dependency files (language, history) could not be found. The program will now exit.");
            }

            return false;
        }

    }
}
