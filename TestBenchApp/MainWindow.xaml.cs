using ias.andonmanager;
using Printer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TestBenchApp.DashBoard;
using TestBenchApp.Entity;
using TestBenchApp.Line;


namespace TestBenchApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AndonManager andonManager = null;
        AndonManager barcodeAndonManager = null;
        String _dbConnectionString = String.Empty;
        DataAccess dataAccess = null;
        Queue<int> deviceQ = null;
        String[] comLayers;
        AndonManager.MODE Mode = AndonManager.MODE.NONE;
        Users Users;
        User CurrentUser;

        PrinterManager mainBodyPM;
        PrinterManager mainFramePM;
        PrinterManager combinationPM;

        List<Plan> Plans;

        Plan CurrentPlan = null;

        public MainWindow()
        {
            InitializeComponent();

            _dbConnectionString = ConfigurationSettings.AppSettings["DBConStr"];

            DataAccess.conStr = _dbConnectionString;
            dataAccess = new DataAccess();

            String mode = ConfigurationSettings.AppSettings["MODE"];
            Mode = (mode == "MASTER") ? AndonManager.MODE.MASTER : AndonManager.MODE.SLAVE;

            comLayers = ConfigurationSettings.AppSettings["COM_LAYERS"].Split(',');

            deviceQ = dataAccess.getDeviceQ();
            andonManager = new AndonManager(deviceQ, null, Mode);
            andonManager.andonAlertEvent += andonManager_andonAlertEvent;

            //Code added on 11 Nov
            andonManager.barcodeAlertEvent += andonManager_barcodeAlertEvent; 
            andonManager.combStickerAlertEvent +=andonManager_combStickerAlertEvent;
            andonManager.actQtyAlertEvent +=andonManager_actQtyAlertEvent;


            int port = Convert.ToInt32(ConfigurationSettings.AppSettings["PRINTER_PORT"]);
            IPAddress mainBodyipAddr = IPAddress.Parse(ConfigurationSettings.AppSettings["MAINBODY_PRINTER_IP"]);
            IPAddress mainFrameipAddr = IPAddress.Parse(ConfigurationSettings.AppSettings["MAINFRAME_PRINTER_IP"]);
            IPAddress combinationIpAddr = IPAddress.Parse(ConfigurationSettings.AppSettings["COMBINATION_PRINTER_IP"]);

            string mainBodybarcodeFile = ConfigurationSettings.AppSettings["MAINBODY_BARCODE_TEMPLATE"];
            string mainFrameBarcodeFile = ConfigurationSettings.AppSettings["MAINFRAME_BARCODE_TEMPLATE"];
            string combinationBarcodeFile = ConfigurationSettings.AppSettings["COMBINATION_BARCODE_TEMPLATE"];

            mainBodyPM = new PrinterManager { Port = port, IPAddress = mainBodyipAddr, BarcodeFileName = mainBodybarcodeFile };
            mainFramePM = new PrinterManager { Port = port, IPAddress = mainFrameipAddr, BarcodeFileName = mainFrameBarcodeFile };
            combinationPM = new PrinterManager{ Port = port, IPAddress = combinationIpAddr, BarcodeFileName = combinationBarcodeFile };

            mainBodyPM.SetupDriver();
            mainFramePM.SetupDriver();
            combinationPM.SetupDriver();

            Plans = dataAccess.GetPlans();

            if (Plans.Count > 0)
            {
                ModelPanel1.DataContext = Plans[0];

            }
            if (Plans.Count > 1)
            {
                ModelPanel2.DataContext = Plans[1];

            }
            if (Plans.Count > 2)
            {
                ModelPanel3.DataContext = Plans[2];

            }
            if (Plans.Count > 3)
            {
                ModelPanel4.DataContext = Plans[3];

            }


   //         andonManager.start();
        }


        //Code added on 11 Nov
        private void andonManager_actQtyAlertEvent(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void andonManager_combStickerAlertEvent(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void andonManager_barcodeAlertEvent(object sender, BCScannerEventArgs e)
        {
           

        }

       
       

        void andonManager_andonAlertEvent(object sender, AndonAlertEventArgs e)
        {
            try
            {
                foreach (LogEntry lg in e.StationLog)
                {

                    DeviceAssociation deviceAssociation = dataAccess.getDeviceAssociation(e.StationId);

                    if (deviceAssociation == null) return;

                    String logMsg = String.Empty;

                    logMsg += deviceAssociation.Header + ":";
                    String lineName = deviceAssociation.LineName;
                    String stationName = deviceAssociation.StationName;

                    logMsg += lineName + ":" + stationName;

                    logMsg += "--Request Raised" + "----at: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    if (e.StationId == 2)
                    {
                        if (CurrentPlan.BSerialNo > CurrentPlan.Quantity)
                        {
                            MessageBox.Show("Current Plan Completed. Please Modify plan or Select another plan to continue",
                                "Application Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            mainBodyPM.PrintBarcode(CurrentPlan.ModelName,
                                CurrentPlan.ModelNumber + "A" + DateTime.Now.ToString("yyMMdd") + CurrentPlan.BSerialNo.ToString("D4"));
                            dataAccess.InsertUnit(CurrentPlan.ModelNumber, Model.Type.BODY, CurrentPlan.BSerialNo);

                            CurrentPlan.BSerialNo++;
                        }
                    }
                    else if (e.StationId == 1)
                    {
                        if (CurrentPlan.BSerialNo > CurrentPlan.Quantity)
                        {
                            MessageBox.Show("Current Plan Completed. Please Modify plan or Select another plan to continue",
                                "Application Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            mainFramePM.PrintBarcode(CurrentPlan.ModelName,
                                CurrentPlan.ModelNumber + DateTime.Now.ToString("yyMMdd") + CurrentPlan.FSerialNo.ToString("D4"));

                            dataAccess.InsertUnit(CurrentPlan.ModelNumber, Model.Type.FRAME, CurrentPlan.FSerialNo);
                            CurrentPlan.FSerialNo++;
                        }
                    }

                    tbMsg.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                            new Action(() => { tbMsg.Text += logMsg + Environment.NewLine; }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK);
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            


        }

        private void tabPlan_Loaded(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
          
                Users = dataAccess.GetUsers();
                tabPlan.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                                new Action(() =>
                                                {
                                                    BaseGrid.Children.Clear();
                                                    LoginPage lp = new LoginPage(Users);
                                                    lp.LoginEvent += lp_LoginEvent;
                                                    BaseGrid.Children.Add(lp);
                                                }));
           
        }

        void lp_LoginEvent(object sender, LoginEventArgs e)
        {
             
                CurrentUser = e.User;

                BaseGrid.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                                new Action(() =>
                                                {
                                                    BaseGrid.Children.Clear();
                                                    DashBoardView db = new DashBoardView(Users,CurrentUser.Name);
                                                    BaseGrid.Children.Add(db);
                                                }));
            
           
        }

       

      
        private void cb1_Click(object sender, RoutedEventArgs e)
        {
            CheckBox c = (CheckBox)sender;

            Plan p = (Plan)c.DataContext;
            if (p == null)
            {
                c.IsChecked = false;
                return;
            }
            foreach (Plan p1 in Plans)
            {
                if (p.ModelNumber == p1.ModelNumber)
                {
                    CurrentPlan = p;
                  
                    continue;
                }
                    
                else
                    p1.Status = false;
            }
            
        }

        private void cb4_Click(object sender, RoutedEventArgs e)
        {
            CheckBox c = (CheckBox)sender;

            Plan p = (Plan)c.DataContext;
            if (p == null)
            {
                c.IsChecked = false;
                return;
            }
            foreach (Plan p1 in Plans)
            {
                if (p.ModelNumber == p1.ModelNumber)
                {
                    CurrentPlan = p;

                    continue;
                }
                else
                    p1.Status = false;
            }
            
        }

        private void cb3_Click(object sender, RoutedEventArgs e)
        {
            CheckBox c = (CheckBox)sender;

            Plan p = (Plan)c.DataContext;
            if (p == null)
            {
                c.IsChecked = false;
                return;
            }
            foreach (Plan p1 in Plans)
            {
                if (p.ModelNumber == p1.ModelNumber)
                {
                    CurrentPlan = p;

                    continue;
                }
                else
                    p1.Status = false;
            }
        }

        private void cb2_Click(object sender, RoutedEventArgs e)
        {
            CheckBox c = (CheckBox)sender;

            Plan p = (Plan)c.DataContext;
            if (p == null)
            {
                c.IsChecked = false;
                return;
            }
            foreach (Plan p1 in Plans)
            {
                if (p.ModelNumber == p1.ModelNumber)
                {
                    CurrentPlan = p;

                    continue;
                }
                else
                    p1.Status = false;
            }
            
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.Source is TabItem)
            {
                if (this.IsLoaded)
                {
                    Users = dataAccess.GetUsers();
                    tabPlan.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                                    new Action(() =>
                                                    {
                                                        BaseGrid.Children.Clear();
                                                        LoginPage lp = new LoginPage(Users);
                                                        lp.LoginEvent += lp_LoginEvent;
                                                        BaseGrid.Children.Add(lp);
                                                    }));
                }
            }

           
        }

    }
}
