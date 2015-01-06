using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestBenchApp.UIControls
{
    /// <summary>
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class ReportManager : UserControl
    {
        DataAccess da;
        DataTable ReportTable;
        public ReportManager()
        {
            InitializeComponent();
            da = new DataAccess();
        }

        private void ReportGenerateButton_Click(object sender, RoutedEventArgs e)
        {
            ReportTable = da.GetReportData(dpFrom.SelectedDate.Value, dpTo.SelectedDate.Value);
            dgReportGrid.DataContext = ReportTable;
            dgReportGrid.Visibility = System.Windows.Visibility.Visible;
            
        }

        private void ReportExportButton_Click(object sender, RoutedEventArgs e)
        {
           
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV (.csv)|*.csv";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                try
                {
                    String filename = dlg.FileName;
                    FileInfo report = new FileInfo(filename);
                    StreamWriter sw = report.CreateText();

                    sw.Write("DATE TIME,MODEL,Main Frame Barcode,Main Body Barcode,Integrated Barcode,Combi(MFG) Barcode,RO Barcode," + Environment.NewLine);

                    for (int i = 0; i < ReportTable.Rows.Count; i++)
                    {
                        String dateTime = ReportTable.Rows[i]["Date-Time"] == DBNull.Value ? ("")
                                            : ((String)ReportTable.Rows[i]["Date-Time"]);
                        String model = ReportTable.Rows[i]["Model"] == DBNull.Value ? ("")
                                            : ((String)ReportTable.Rows[i]["Model"]);
                        String MainFrameCode = ReportTable.Rows[i]["Main Frame Barcode"] == DBNull.Value ? ("")
                                            : ((String)ReportTable.Rows[i]["Main Frame Barcode"]);

                        String MainBodyCode = ReportTable.Rows[i]["Main Body Barcode"] == DBNull.Value ? ("")
                                            : ((String)ReportTable.Rows[i]["Main Body Barcode"]);
                        String ICode = ReportTable.Rows[i]["Integrated Barcode"] == DBNull.Value ? ("")
                                            : ((String)ReportTable.Rows[i]["Integrated Barcode"]);
                        String CombiStatus = ReportTable.Rows[i]["Combination Barcode"] == DBNull.Value ? ("")
                                           : ((String)ReportTable.Rows[i]["Combination Barcode"]);
                        String ROCode = ReportTable.Rows[i]["RO Barcode"] == DBNull.Value ? ("")
                                          : ((String)ReportTable.Rows[i]["RO Barcode"]);

                        String reportEntry = dateTime + "," + model + "," + MainFrameCode + "," + MainBodyCode + ","
                            + ICode + "," + CombiStatus + "," + ROCode;
                        sw.Write(reportEntry);
                        sw.Write(Environment.NewLine);
                    }
                    sw.Close();
                    MessageBox.Show("Report Generation Successful", "Report Generation Message",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception exp)
                {
                    MessageBox.Show("Error Generating Report" + Environment.NewLine + exp.Message, "Report Generation Error",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        
        }
    }
}
