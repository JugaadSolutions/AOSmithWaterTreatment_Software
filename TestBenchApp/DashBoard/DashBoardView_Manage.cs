
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using System.Runtime.Serialization;
using System.Xml;

using System.Xml.Serialization;
using TestBenchApp.Entity;

using TestBenchApp.UIControls;
using shared;
using shared.Entity;

namespace TestBenchApp.DashBoard
{
    public partial class DashBoardView : UserControl
    {
        
        public String CurrentUser { get; set; }
        Users Users;
        public bool cbM1Checked = false;
        public bool cbF1Checked = false;
        public bool cbCSChecked = false;

        #region MANAGE_PASSWORD
        private void Password_Click_1(object sender, RoutedEventArgs e)
        {
            PasswordControl pwdControl = new PasswordControl(Users);
            pwdControl.PasswordChangeEvent += pwdControl_PasswordChangeEvent;
            pwdControl.PasswordCancelEvent += pwdControl_PasswordCancelEvent;

            Transient.Children.Clear();
            Transient.Children.Add(pwdControl);
            Keyboard.Focus(pwdControl);
        }

        void pwdControl_PasswordCancelEvent(object sender, PasswordCancelEventArgs e)
        {
            Transient.Children.Clear();
        }

        void pwdControl_PasswordChangeEvent(object sender, PasswordChangeEventArgs e)
        {
            dataAccess.ChangePassword(e.User);
            Users.ChangePassword(e.User);

          
           
            MessageBox.Show("Password Changed", "Application Info", MessageBoxButton.OK, MessageBoxImage.Information);
            Transient.Children.Clear();
        }

        #endregion

        #region MANAGE_PLAN
        private void SetPlan_Click(object sender, RoutedEventArgs e)
        {
            PlanManager PM = new PlanManager();


            Transient.Children.Clear();
            Transient.Children.Add(PM);
            Transient.Visibility = System.Windows.Visibility.Visible;
        }

        private void p_btnDoneClicked(object sender, EventArgs e)
        {
            Transient.Children.Clear();
        }
        #endregion

        #region MANAGE_MODELS
        private void Models_Click_1(object sender, RoutedEventArgs e)
        {
            ModelsManager c = new ModelsManager();
            c.CancelClicked += c_btnCancelClicked;
            c.TestPrintBtnClicked += c_TestPrintBtnClicked;
            Transient.Children.Clear();
            Transient.Children.Add(c);
            Transient.Visibility = System.Windows.Visibility.Visible;
        }

        void c_TestPrintBtnClicked(object sender, TestEventArgs e)
        {
            String slNo = null;

                    if (e.m.Name.Contains("Dummy"))
                    {
                        PrinterManager.PrintCombSticker(e.m, e.m.Code + DateTime.Now.ToString("yyMMdd") + "0000",
                            PrinterManager.TemplatePath + e.m.Name + ".prn");
                    }
                    else
                        PrinterManager.PrintCombSticker(e.m, e.m.Code + DateTime.Now.ToString("yyMMdd") + "0000",
                            PrinterManager.TemplatePath + "CS.prn");
                    
                
            
        }

      
        private void c_btnCancelClicked(object sender, EventArgs e)
        {
            Transient.Children.Clear();
        }

        #endregion

        #region MANAGE_REPRINT
        private void Reprint_Click_1(object sender, RoutedEventArgs e)
        {
            ReprintManager r = new ReprintManager();
            r.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            r.btnDoneClicked += r_btnDoneClicked;
            r.F1Reprint += r_F1Reprint;
            r.M1Reprint += r_M1Reprint;
            r.CSReprint += r_CSReprint;
            r.TOKReprint += r_TOKReprint;
            r.BatchPrint += r_BatchPrint;

            Transient.Children.Clear();
            Transient.Children.Add(r);
            Transient.Visibility = System.Windows.Visibility.Visible;

        }

        void r_BatchPrint(object sender, BatchPrintArgs e)
        {
            String fileName = String.Empty;
            switch (e.ReprintStage)
            {
                case REPRINT_STAGE.F1:
                    if (e.Model.Contains("Dummy"))
                    {
                        fileName = "DummyF1.prn";
                    }
                    else fileName = "F1.prn";
                    for (int i = e.SerialNo; i <= e.Quantity; i++)
                    {
                        PrinterManager.PrintBarcode("F1Printer", e.Model, e.Code, e.Date, i.ToString("D4"),
                            PrinterManager.TemplatePath+fileName);
                        
                    }
                    

                    break;
                case REPRINT_STAGE.M1:
                    if( e.Model.Contains("Dummy"))
                    {
                        fileName = "DummyM1.prn";
                    }
                    else fileName = "M1.prn";
                    for (int i = e.SerialNo; i <= e.Quantity; i++)
                    {
                         
                        PrinterManager.PrintBarcode("M1Printer", e.Model, e.Code + "A", e.Date, i.ToString("D4"),
                            PrinterManager.TemplatePath+fileName);
                        
                    }
                    
                    break;
                case REPRINT_STAGE.INTEGRATED:
                    if (e.Model.Contains("Dummy"))
                    {
                        fileName = "DummyIntegrated.prn";
                    }
                    else fileName = "Integrated.prn";
                    for (int i = e.SerialNo; i <= e.Quantity; i++)
                    {
                        PrinterManager.PrintBarcode("F2Printer", e.Model, e.Code, e.Date, i.ToString("D4"),
                            PrinterManager.TemplatePath+fileName);
                       
                    }
                    
                    break;
                case REPRINT_STAGE.COMBINATION:
                    for (int i = e.SerialNo; i <= e.Quantity; i++)
                    {
                        foreach (Model m in Models)
                        {
                            if (m.Code == e.Code)
                            {
                                if (m.Name.Contains("Dummy"))
                                {
                                    PrinterManager.PrintCombSticker(m, e.Code + e.Date + i.ToString("D4"),
                                        PrinterManager.TemplatePath+ m.Name + ".prn");
                                }
                                else
                                PrinterManager.PrintCombSticker(m, e.Code + e.Date + i.ToString("D4"));
                                break;
                            }
                        }
                        
                    }
                    
                    break;
                default:
                    break;
            }
        }

        void r_TOKReprint(object sender, ReprintArgs e)
        {
            String fileName = string.Empty;
            if (e.Model.Contains("Dummy"))
            {
                fileName = "DummyIntegrated.prn";
            }
            else fileName = "Integrated.prn";
            PrinterManager.PrintBarcode("F2Printer", e.Model, e.Code, e.Date, e.SerialNo,
                PrinterManager.TemplatePath+fileName);
        }

        void r_CSReprint(object sender, ReprintArgs e)
        {
            foreach (Model m in Models)
            {
                if (m.Code == e.Code)
                {
                    bool result = false;
                    int count = 0;
                    if (m.Name.Contains("Dummy"))
                    {
                        do
                        {
                           result = 
                               PrinterManager.PrintCombSticker(m, e.Code + e.Date + e.SerialNo, PrinterManager.TemplatePath + m.Name + ".prn");
                        } while ((result == false) && (count < 3));
                    }
                    else
                    {
                       
                        do
                        {
                            result = PrinterManager.PrintCombSticker(m, e.Code + e.Date + e.SerialNo);
                            count++;
                        } while ((result == false) && (count < 3));
                    }
                        
                    break;
                }
            }
            
        }

        void r_M1Reprint(object sender, ReprintArgs e)
        {
            String fileName = string.Empty;
            if (e.Model.Contains("Dummy"))
            {
                fileName = "DummyM1.prn";
            }
            else fileName = "M1.prn";
            PrinterManager.PrintBarcode("M1Printer", e.Model, e.Code, e.Date, e.SerialNo,
                PrinterManager.TemplatePath+fileName);
        }

        void r_F1Reprint(object sender, ReprintArgs e)
        {
            String fileName = string.Empty;
            if (e.Model.Contains("Dummy"))
            {
                fileName = "DummyF1.prn";
            }
            else fileName = "F1.prn";
            PrinterManager.PrintBarcode("F1Printer",e.Model,e.Code, e.Date,e.SerialNo,
                PrinterManager.TemplatePath+fileName);
        }

       

       

        private void r_btnDoneClicked(object sender, EventArgs e)
        {
            Transient.Children.Clear();
           
        }
        #endregion


        #region MANAGE_REPORTS
        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            ReportManager r = new ReportManager();
           

            Transient.Children.Clear();
            Transient.Children.Add(r);
            Transient.Visibility = System.Windows.Visibility.Visible;
        }
        #endregion

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            
        }









      
    }
}
