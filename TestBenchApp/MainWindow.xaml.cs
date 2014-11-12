using ias.andonmanager;
using Printer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Timers;
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

        List<Plan> FramePlans,BodyPlans;

        Plan CurrentFramePlan = null;
        Plan CurrentBodyPlan = null;

        Timer tickTimer;
        bool FBypass = false;
        bool CBypass = false;

        Queue<String> FCodeQ;
        Queue<String> BCodeQ;
        Queue<String> CCodeQ;

        bool APPSimulation = false;

        public MainWindow()
        {
            InitializeComponent();

            _dbConnectionString = ConfigurationSettings.AppSettings["DBConStr"];

            DataAccess.conStr = _dbConnectionString;
            dataAccess = new DataAccess();

            String mode = ConfigurationSettings.AppSettings["MODE"];
            Mode = (mode == "MASTER") ? AndonManager.MODE.MASTER : AndonManager.MODE.SLAVE;

            comLayers = ConfigurationSettings.AppSettings["COM_LAYERS"].Split(',');

            String combPrinterName = ConfigurationSettings.AppSettings["COMBINATION_BARCODE_PRINTER_NAME"];

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
            combinationPM = new PrinterManager{ Port = port, IPAddress = combinationIpAddr, BarcodeFileName = combinationBarcodeFile, combBarcodePrinterName = combPrinterName };

            mainBodyPM.SetupDriver();
            mainFramePM.SetupDriver();
            combinationPM.SetupDriver();

            updatePlan();

            tickTimer = new Timer(1000);
            tickTimer.AutoReset = false;
            tickTimer.Elapsed += tickTimer_Elapsed;

            if (ConfigurationSettings.AppSettings["PBSIMULATION"] == "Yes")
            {
                Simulation = true;
                BaseWindow.KeyDown += Window_KeyDown;

                FCodeQ = new Queue<string>();
                BCodeQ = new Queue<string>();
                CCodeQ = new Queue<string>();



            }
            else
            {
                Simulation = false;
                andonManager.start();
            }


            
            tickTimer.Start();
        }

        void tickTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            tickTimer.Stop();
            updatePlan();
            tickTimer.Start();
        }

        private void updatePlan()
        {
            FramePlans = dataAccess.GetPlans();
            BodyPlans = dataAccess.GetPlans();

            if (FramePlans.Count > 0)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                              new Action(() =>
                              {
                                  FrameModelPanel1.DataContext = FramePlans[0];
                                  BodyModelPanel1.DataContext = BodyPlans[0];
                              }));

            }
            if (FramePlans.Count > 1)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                             new Action(() =>
                             {
                                 FrameModelPanel2.DataContext = FramePlans[1];
                                 BodyModelPanel2.DataContext = BodyPlans[1];
                             }));
            }
            if (FramePlans.Count > 2)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                             new Action(() =>
                             {
                                 FrameModelPanel3.DataContext = FramePlans[2];
                                 BodyModelPanel3.DataContext = BodyPlans[2];
                             }));

            }
            if (FramePlans.Count > 3)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                             new Action(() =>
                             {
                                 FrameModelPanel4.DataContext = FramePlans[3];
                                 BodyModelPanel4.DataContext = BodyPlans[3];
                             }));

            }

            int Total = 0;
            int Actual = 0;
            int FSerial = 0;
            int CSerial = 0;
            foreach (Plan p in FramePlans)
            {
                Total += p.Quantity;
                Actual += p.Actual;
                FSerial += p.FSerialNo;
                CSerial += p.CombinationSerialNo;
            }
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                             new Action(() =>
                             {
                                 FrametbTotalPlan.Text = Total.ToString();
                                 FrametbTotalAct.Text = Actual.ToString();
                                 FrametbTotalFserial.Text = FSerial.ToString();
                                 FrametbTotalCserial.Text = CSerial.ToString();
                             }));

            Total = 0;
            Actual = 0;
            int BSerial = 0;
            foreach (Plan p in BodyPlans)
            {
                Total += p.Quantity;
                Actual += p.Actual;
                BSerial += p.BSerialNo;
            }
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                             new Action(() =>
                             {
                                 BodyTotalPlan.Text = Total.ToString();
                                 BodyTotalAct.Text = BSerial.ToString();
                             }));

        }


        //Code added on 11 Nov
        private void andonManager_actQtyAlertEvent(object sender, actQtyScannerEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void andonManager_combStickerAlertEvent(object sender, CSScannerEventArgs e)
        {
            String barcode = e.ModelNumber + e.Timestamp + e.SerialNo.ToString("D4");
            
            String assocationBarcode = String.Empty;
            if (e.ModelNumber.Contains("A")) // if body
            {
               
                if (FBypass == false)  //if not bypassed
                {
                    if (dataAccess.CheckOKStatus(barcode) == false) // if not ok
                        return; //do nothing
                }

                assocationBarcode = dataAccess.UnitAssociated( Model.Type.BODY);

               
                if (assocationBarcode != String.Empty) // if association exists
                {
                    dataAccess.UpdateAssociation(barcode, Model.Type.BODY, assocationBarcode);
                    generateCombinationCode(assocationBarcode);
                }
                else
                {
                    dataAccess.InsertUnitAssociation(barcode, Model.Type.BODY);
                    dataAccess.UpdateUnit(barcode);
                }
            }
            else
            {
               
               
                if (CBypass == false)  //if not bypassed
                {
                    if (dataAccess.CheckOKStatus(barcode) == false) // if not ok
                        return; //do nothing
                }

                assocationBarcode = dataAccess.UnitAssociated( Model.Type.FRAME);

              
                if (assocationBarcode != String.Empty) // if association exists
                {
                    dataAccess.UpdateAssociation(barcode, Model.Type.FRAME, assocationBarcode);
                    generateCombinationCode(barcode);
                }
                else
                {
                    dataAccess.InsertUnitAssociation(barcode, Model.Type.FRAME);

                }

            }
            

        }

        private void generateCombinationCode(string assocationBarcode)

        {
            Plan tempPlan = null;
            tempPlan = new Plan();

           // tempPlan.BSerialNo = e.SerialNo;

            

            

        }

        void andonManager_barcodeAlertEvent(object sender, BCScannerEventArgs e)
        {

            String barcode = e.ModelNumber + e.Timestamp + e.SerialNo.ToString("D4");

            dataAccess.UpdateUnit(barcode);

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
                        if (CurrentBodyPlan.BSerialNo > CurrentBodyPlan.Quantity)
                        {
                            MessageBox.Show("Current Plan Completed. Please Modify plan or Select another plan to continue",
                                "Application Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            CurrentBodyPlan.BSerialNo++;
                            if (Simulation)
                            {
                                String bcode =  CurrentBodyPlan.ModelCode + "A" + DateTime.Now.ToString("yyMMdd")
                                    + CurrentBodyPlan.BSerialNo.ToString("D4");
                                mainFramePM.PrintBarcode(CurrentBodyPlan.ModelName,
                                  bcode );

                                BCodeQ.Enqueue(bcode);
                            }
                            else
                            {
                                mainBodyPM.PrintBarcode(CurrentBodyPlan.ModelName,
                                    CurrentBodyPlan.ModelCode + "A" + DateTime.Now.ToString("yyMMdd")
                                    + CurrentBodyPlan.BSerialNo.ToString("D4"));
                            }
                            dataAccess.InsertUnit(CurrentBodyPlan.ModelCode, Model.Type.BODY, 
                                CurrentBodyPlan.BSerialNo);
                            dataAccess.UpdateBSerial(CurrentBodyPlan);


                            
                        }
                    }
                    else if (e.StationId == 1)
                    {
                        if (CurrentFramePlan.FSerialNo > CurrentFramePlan.Quantity)
                        {
                            MessageBox.Show("Current Plan Completed. Please Modify plan or Select another plan to continue",
                                "Application Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                           
                            CurrentFramePlan.FSerialNo++;
                            String fcode = CurrentFramePlan.ModelCode + DateTime.Now.ToString("yyMMdd")
                               + CurrentFramePlan.FSerialNo.ToString("D4");

                            mainFramePM.PrintBarcode(CurrentFramePlan.ModelName,
                                fcode);

                            if (Simulation)
                                FCodeQ.Enqueue(fcode);

                            dataAccess.InsertUnit(CurrentFramePlan.ModelCode, Model.Type.FRAME, CurrentFramePlan.FSerialNo);
                            dataAccess.UpdateFSerial(CurrentFramePlan);
                            
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
                                                    DashBoardView db = new DashBoardView(Users, CurrentUser.Name, combinationPM);
                                                    BaseGrid.Children.Add(db);
                                                }));
            
           
        }




        private void Framecb1_Click(object sender, RoutedEventArgs e)
        {
            CheckBox c = (CheckBox)sender;

            Plan p = (Plan)c.DataContext;
            if (p == null)
            {
                c.IsChecked = false;
                return;
            }
            foreach (Plan p1 in FramePlans)
            {
                if (p.ModelCode == p1.ModelCode)
                {
                    CurrentFramePlan = p;
                    dataAccess.UpdateFPlanStatus(CurrentFramePlan);
                    continue;
                }
                else
                {
                    p1.FStatus = false;
                    dataAccess.UpdateFPlanStatus(p1);
                }
            }
            
        }

        private void Framecb2_Click(object sender, RoutedEventArgs e)
        {
            CheckBox c = (CheckBox)sender;

            Plan p = (Plan)c.DataContext;
            if (p == null)
            {
                c.IsChecked = false;
                return;
            }
            foreach (Plan p1 in FramePlans)
            {
                if (p.ModelCode == p1.ModelCode)
                {
                    CurrentFramePlan = p;
                    dataAccess.UpdateFPlanStatus(CurrentFramePlan);
                    continue;
                }
                else
                {
                    p1.FStatus = false;
                    dataAccess.UpdateFPlanStatus(p1);
                }
            }

        }

        private void Framecb3_Click(object sender, RoutedEventArgs e)
        {
            CheckBox c = (CheckBox)sender;

            Plan p = (Plan)c.DataContext;
            if (p == null)
            {
                c.IsChecked = false;
                return;
            }
            foreach (Plan p1 in FramePlans)
            {
                if (p.ModelCode == p1.ModelCode)
                {
                    CurrentFramePlan = p;
                    dataAccess.UpdateFPlanStatus(CurrentFramePlan);
                    continue;
                }
                else
                {
                    p1.FStatus = false;
                    dataAccess.UpdateFPlanStatus(p1);
                }
            }
        }

        private void Framecb4_Click(object sender, RoutedEventArgs e)
        {
            CheckBox c = (CheckBox)sender;

            Plan p = (Plan)c.DataContext;
            if (p == null)
            {
                c.IsChecked = false;
                return;
            }
            foreach (Plan p1 in FramePlans)
            {
                if (p.ModelCode == p1.ModelCode)
                {
                    CurrentFramePlan = p;
                    dataAccess.UpdateFPlanStatus(CurrentFramePlan);
                    continue;
                }
                else
                {
                    p1.FStatus = false;
                    dataAccess.UpdateFPlanStatus(p1);
                }
            }
            
        }

    



        private void Bodycb1_Click(object sender, RoutedEventArgs e)
        {
            CheckBox c = (CheckBox)sender;

            Plan p = (Plan)c.DataContext;
            if (p == null)
            {
                c.IsChecked = false;
                return;
            }
            foreach (Plan p1 in BodyPlans)
            {
                if (p.ModelCode == p1.ModelCode)
                {
                    CurrentBodyPlan = p;
                    dataAccess.UpdateBPlanStatus(CurrentBodyPlan);
                    continue;
                }
                else
                {
                    p1.BStatus = false;
                    dataAccess.UpdateBPlanStatus(p1);
                }
            }

        }

        private void Bodycb2_Click(object sender, RoutedEventArgs e)
        {
            CheckBox c = (CheckBox)sender;

            Plan p = (Plan)c.DataContext;
            if (p == null)
            {
                c.IsChecked = false;
                return;
            }
            foreach (Plan p1 in BodyPlans)
            {
                if (p.ModelCode == p1.ModelCode)
                {
                    CurrentBodyPlan = p;
                    dataAccess.UpdateBPlanStatus(CurrentBodyPlan);
                    continue;
                }
                else
                {
                    p1.BStatus = false;
                    dataAccess.UpdateBPlanStatus(p1);
                }
            }

        }

        private void Bodycb3_Click(object sender, RoutedEventArgs e)
        {
            CheckBox c = (CheckBox)sender;

            Plan p = (Plan)c.DataContext;
            if (p == null)
            {
                c.IsChecked = false;
                return;
            }
            foreach (Plan p1 in BodyPlans)
            {
                if (p.ModelCode == p1.ModelCode)
                {
                    CurrentBodyPlan = p;
                    dataAccess.UpdateBPlanStatus(CurrentBodyPlan);
                    continue;
                }
                else
                {
                    p1.BStatus = false;
                    dataAccess.UpdateBPlanStatus(p1);
                }
            }
        }

        private void Bodycb4_Click(object sender, RoutedEventArgs e)
        {
            CheckBox c = (CheckBox)sender;

            Plan p = (Plan)c.DataContext;
            if (p == null)
            {
                c.IsChecked = false;
                return;
            }
            foreach (Plan p1 in BodyPlans)
            {
                if (p.ModelCode == p1.ModelCode)
                {
                    CurrentBodyPlan = p;
                    dataAccess.UpdateBPlanStatus(CurrentBodyPlan);
                    continue;
                }
                else
                {
                    p1.BStatus = false;
                    dataAccess.UpdateBPlanStatus(p1);
                }
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

        private void FrametbPq4_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void FrametbF1_1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

    }
}
