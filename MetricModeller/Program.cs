using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetricModeller {
    static class Program {

        private const string langFilePath = "../../../language_prod.csv";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Dictionary<string, Tuple<decimal, int>> langData;
            if (TryReadFiles(out langData)) {
                int n = langData.Count;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain(langData));
            }
        }

        private static bool TryReadFiles(out Dictionary<string, Tuple<decimal, int>> langData) {

            langData = new Dictionary<string, Tuple<decimal, int>>();

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

                return true;

            } catch (IOException e) {
                // File not found -> Display an error and cancel the read-in       
                MessageBox.Show("Error: One or more dependency files (language, history) could not be found. The program will now exit.");
            }

            return false;
        }

    }
}
