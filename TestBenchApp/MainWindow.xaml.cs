using ias.andonmanager;
using Printer;
using shared;
using shared.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Net;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TestBenchApp.DashBoard;
using TestBenchApp.Entity;
using ModbusTCP;


namespace TestBenchApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AndonManager andonManager = null;
       
        String _dbConnectionString = String.Empty;
        DataAccess dataAccess = null;
        Queue<int> deviceQ = null;
        String[] comLayers;
        AndonManager.MODE Mode = AndonManager.MODE.NONE;
        Users Users;
        User CurrentUser;

        PrinterManager PrinterManager;

        List<Plan> FramePlans, BodyPlans;

        ObservableCollection<Model> Models;

        Plan CurrentFramePlan = null;
        Plan CurrentBodyPlan = null;

        Timer tickTimer,modbusTimer;
        Timer F2Error1, F2Error2;
        Timer IError;
        Timer FGSuccess;
        bool FBypass = false;
        bool CBypass = false;

        Queue<String> FCodeQ;
        Queue<String> BCodeQ;
        Queue<String> ICodeQ;
        Queue<String> CCodeQ;
        Queue<String> ACodeQ;

        bool PrinterSimulation = false;

        bool ScannerSimulation = false;
        bool ControllerSimulation = false;

        DashBoardView dbView;

        List<UnitAssociation> Associations;
        int AssociationTimeout;

        int f1PrintCount = 0;
        int m1PrintCount = 0;

        string F1BarcodeFile = String.Empty;
        string M1BarcodeFile = String.Empty;
        string IntegratedBarcodeFile = String.Empty;

        string DummyF1BarcodeFile = String.Empty;
        string DummyM1BarcodeFile = String.Empty;
        string DummyIntegratedBarcodeFile = String.Empty;

        string templatePath = String.Empty;

        string CSDataFile = String.Empty;

        bool waitingforRO = false;
        String latestCombinationCode = String.Empty;

        Master modbusMaster = null;

        bool F1pressed = false;
        int F1Checkcount = 0;

        bool M1pressed = false;
        int M1Checkcount = 0;

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
            andonManager.combStickerAlertEvent += andonManager_combStickerAlertEvent;
            andonManager.actQtyAlertEvent += andonManager_actQtyAlertEvent;


            int port = Convert.ToInt32(ConfigurationSettings.AppSettings["PRINTER_PORT"]);
            IPAddress F1PrinterIPAddr = IPAddress.Parse(ConfigurationSettings.AppSettings["F1_PRINTER_IP"]);
            IPAddress M1PrinterIPAddr = IPAddress.Parse(ConfigurationSettings.AppSettings["M1_PRINTER_IP"]);
            IPAddress TOKPrinterIPAddr = IPAddress.Parse(ConfigurationSettings.AppSettings["TOK_PRINTER_IP"]);
            String PLCIPAddr = ConfigurationSettings.AppSettings["PLC_IP"];



            F1BarcodeFile = ConfigurationSettings.AppSettings["F1_BARCODE_TEMPLATE"];
            M1BarcodeFile = ConfigurationSettings.AppSettings["M1_BARCODE_TEMPLATE"];
            IntegratedBarcodeFile = ConfigurationSettings.AppSettings["INTEGRATED_BARCODE_TEMPLATE"];

            DummyF1BarcodeFile = ConfigurationSettings.AppSettings["DUMMY_F1_BARCODE_TEMPLATE"];
            DummyM1BarcodeFile = ConfigurationSettings.AppSettings["DUMMY_M1_BARCODE_TEMPLATE"];
            DummyIntegratedBarcodeFile = ConfigurationSettings.AppSettings["DUMMY_INTEGRATED_BARCODE_TEMPLATE"];

            templatePath = ConfigurationSettings.AppSettings["TEMPLATE_PATH"];

            CSDataFile = ConfigurationSettings.AppSettings["CS_BARCODE_TEMPLATE"];


            

            Models = dataAccess.GetModels();

            tickTimer = new Timer(3000);
            tickTimer.AutoReset = false;
            tickTimer.Elapsed += tickTimer_Elapsed;

            modbusTimer = new Timer(500);
            modbusTimer.AutoReset = false;
            modbusTimer.Elapsed += modbusTimer_Elapsed;

            F2Error1 = new Timer(2000);
            F2Error1.AutoReset = false;
            F2Error1.Elapsed += F2Error1_Elapsed;

            F2Error2 = new Timer(1000);
            F2Error2.AutoReset = false;
            F2Error2.Elapsed += F2Error2_Elapsed;


            IError = new Timer(1000);
            IError.AutoReset = false;
            IError.Elapsed += IError_Elapsed;

            FGSuccess = new Timer(1000);
            FGSuccess.AutoReset = false;
            FGSuccess.Elapsed += FGError_Elapsed;

            Associations = new List<UnitAssociation>();

            if (ConfigurationSettings.AppSettings["CONTROLLER_SIMULATION"] == "Yes")
            {
                ControllerSimulation = true;



            }

            if (ConfigurationSettings.AppSettings["SCANNER_SIMULATION"] == "Yes")
            {
                ScannerSimulation = true;
                FCodeQ = new Queue<string>();
                BCodeQ = new Queue<string>();
                ICodeQ = new Queue<string>();
                CCodeQ = new Queue<string>();
                ACodeQ = new Queue<string>();

            }

            if (ConfigurationSettings.AppSettings["PRINTER_SIMULATION"] == "Yes")
            {
                PrinterSimulation = true;

            }

            if (PrinterSimulation || ScannerSimulation || ControllerSimulation)
            {
                BaseWindow.KeyDown += Window_KeyDown;
            }

            if (!PrinterSimulation)
            {

                PrinterManager = new Printer.PrinterManager(templatePath);
                PrinterManager.SetupDriver("F1Printer", F1PrinterIPAddr, port, F1BarcodeFile);
                PrinterManager.SetupDriver("M1Printer", M1PrinterIPAddr, port, M1BarcodeFile);
                PrinterManager.SetupDriver("F2Printer", TOKPrinterIPAddr, port, IntegratedBarcodeFile);
                PrinterManager.CombinationPrinterName = combPrinterName;
                PrinterManager.CombinationTemplate = CSDataFile;

            }

            if (!ControllerSimulation)
            {
                andonManager.start();
            }

            AssociationTimeout = Convert.ToInt32(ConfigurationSettings.AppSettings["ASSOCIATION_TIMEOUT"]);

            modbusMaster = new Master(PLCIPAddr,(ushort) 502);
            modbusMaster.OnResponseData += new ModbusTCP.Master.ResponseData(MBmaster_OnResponseData);
            modbusMaster.OnException += new ModbusTCP.Master.ExceptionData(MBmaster_OnException);

            updatePlan();
            tickTimer.Start();
            modbusTimer.Start();
        }

        void FGError_Elapsed(object sender, ElapsedEventArgs e)
        {
            FGSuccess.Stop();
            byte[] values = { 0, 0 };
            modbusMaster.WriteMultipleRegister((ushort)3, (byte)0, (ushort)4, values);
        }

        void IError_Elapsed(object sender, ElapsedEventArgs e)
        {
            IError.Stop();
            byte[] values = { 0, 0 };
            modbusMaster.WriteMultipleRegister((ushort)3, (byte)0, (ushort)3, values);
        }

        void F2Error2_Elapsed(object sender, ElapsedEventArgs e)
        {
            F2Error2.Stop();
            byte[] values = { 0, 0 };
            modbusMaster.WriteMultipleRegister((ushort)3, (byte)0, (ushort)2, values);
        }

        void F2Error1_Elapsed(object sender, ElapsedEventArgs e)
        {
            F2Error1.Stop();
            byte[] values = { 0, 0 };
            modbusMaster.WriteMultipleRegister((ushort)3, (byte)0, (ushort)2, values);
        }

        void modbusTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            modbusTimer.Stop();
            modbusMaster.ReadHoldingRegister(2, 0, 0, 2);
            modbusTimer.Start();
        }

        void tickTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            tickTimer.Stop();
           
            
            updatePlan();
            dataAccess.DeleteAssociationTimeouts(AssociationTimeout);
            updateUnitData();
            tickTimer.Start();
        }

        private void updateUnitData()
        {
            DataTable dt = dataAccess.GetReportData(DateTime.Now, DateTime.Now);
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                             new Action(() =>
                             {
                                 AssociationGrid.DataContext = dt;
                             }));
        }

        private void updatePlan()
        {
            FramePlans = dataAccess.GetPlans(Model.Type.FRAME);
            BodyPlans = dataAccess.GetPlans(Model.Type.BODY);

            if (FramePlans.Count > 0)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                              new Action(() =>
                              {
                                  FrameModelPanel1.DataContext = FramePlans[0];

                              }));

            }
            if (FramePlans.Count > 1)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                             new Action(() =>
                             {
                                 FrameModelPanel2.DataContext = FramePlans[1];

                             }));
            }
            if (FramePlans.Count > 2)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                             new Action(() =>
                             {
                                 FrameModelPanel3.DataContext = FramePlans[2];

                             }));

            }
            if (FramePlans.Count > 3)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                             new Action(() =>
                             {
                                 FrameModelPanel4.DataContext = FramePlans[3];

                             }));

            }

            foreach (Plan p in FramePlans)
            {
                if (p.FStatus == true)
                {
                    CurrentFramePlan = p;
                    break;
                }
            }
                 

            if (BodyPlans.Count > 0)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                              new Action(() =>
                              {

                                  BodyModelPanel1.DataContext = BodyPlans[0];
                              }));

            }
            if (BodyPlans.Count > 1)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                             new Action(() =>
                             {

                                 BodyModelPanel2.DataContext = BodyPlans[1];
                             }));
            }
            if (BodyPlans.Count > 2)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                             new Action(() =>
                             {

                                 BodyModelPanel3.DataContext = BodyPlans[2];
                             }));

            }
            if (BodyPlans.Count > 3)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                             new Action(() =>
                             {

                                 BodyModelPanel4.DataContext = BodyPlans[3];
                             }));

            }

            foreach (Plan p in BodyPlans)
            {
                if (p.BStatus == true)
                {
                    CurrentBodyPlan = p;
                    break;
                }
            }

            int FrameTotal = 0;
            int FrameActual = 0;
            int FSerial = 0;
            int CSerial = 0;
            foreach (Plan p in FramePlans)
            {
                FrameTotal += p.Quantity;
                FrameActual += p.Actual;
                FSerial += p.FSerialNo;
                CSerial += p.CombinationSerialNo;
            }
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                             new Action(() =>
                             {
                                 FrametbTotalPlan.Text = FrameTotal.ToString();
                                 FrametbTotalAct.Text = FrameActual.ToString();
                                 FrametbTotalFserial.Text = FSerial.ToString();
                                 FrametbTotalCserial.Text = CSerial.ToString();
                             }));

            int BodyTotal = 0;

            int BSerial = 0;
            foreach (Plan p in BodyPlans)
            {
                BodyTotal += p.Quantity;

                BSerial += p.BSerialNo;
            }
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                             new Action(() =>
                             {
                                 BodyTotalPlan.Text = BodyTotal.ToString();
                                 BodyTotalAct.Text = BSerial.ToString();
                             }));

        }


        //Code added on 11 Nov
        private void andonManager_actQtyAlertEvent(object sender, actQtyScannerEventArgs e)
        {
            if (dataAccess.CheckWaitingStatus(latestCombinationCode))
            {
                dataAccess.UpdateAsscociationStatus(e.Barcode,latestCombinationCode);
                String modelCode = latestCombinationCode.Substring(0,4);


                foreach (Plan p in FramePlans)
                {
                    if (p.ModelCode == modelCode)
                    {
                        byte[] values = { 0, 1 };
                        p.Actual++;
                        modbusMaster.WriteMultipleRegister((ushort)3, (byte)0, (ushort)4, values);
                        dataAccess.UpdateActual(p.Actual, p.slNumber);
                        FGSuccess.Start();
                        break;
                    }
                   
                }
               

                tbMsg.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                              new Action(() =>
                                              {
                                                  tbMsg.Text +=  DateTime.Now.ToString() +  "Actual Updated" 
                                                      + Environment.NewLine;
                                              }));
                waitingforRO = false;
                latestCombinationCode = string.Empty;
            }

            
            else {
               
                latestCombinationCode = e.Barcode;
                dataAccess.UpdateWaitingStatus(latestCombinationCode);
                if (PrinterSimulation)
                    CCodeQ.Enqueue(latestCombinationCode);
            }
                
               
            
        }

        private void andonManager_combStickerAlertEvent(object sender, CSScannerEventArgs e)
        {
            String iCode = e.ModelNumber + e.Timestamp + e.SerialNo.ToString("D4");
           
            Plan plan= null;

            if (dataAccess.CheckIntegrationStatus(iCode) == false)
            {
                byte[] values = { 0, 1 };
                modbusMaster.WriteMultipleRegister((ushort)3, (byte)0, (ushort)3, values);
                tbMsg.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                           new Action(() =>
                                           {
                                               tbMsg.Text += "F3:Combination Sticker Already Printed" 
                                                   + Environment.NewLine;
                                           }));
                IError.Start();
                return;
            }

            foreach (Model m in Models)
            {
                if (m.Code == e.ModelNumber)
                {
                    foreach (Plan p in FramePlans)
                    {
                        if (p.ModelCode == m.Code)
                        {
                            plan = p;
                            break;
                        }
                    }
                    plan.CombinationSerialNo++;
                    String csCode = e.ModelNumber + DateTime.Now.ToString("yyMMdd") + plan.CombinationSerialNo.ToString("D4");

                    dataAccess.UpdateAssociation(csCode, Model.Type.COMBINED, "", iCode);

                    
                    if (!PrinterSimulation)
                    {
                        if( m.Name.Contains("Dummy"))
                        {
                            PrinterManager.PrintCombSticker(m, csCode, templatePath + m.Name + ".prn");
                        }
                        else
                            PrinterManager.PrintCombSticker(m, csCode);


                    }
                    tbMsg.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                           new Action(() =>
                                           {
                                               tbMsg.Text += DateTime.Now.ToString()  + ":F3-Combination Sticker Printed - " + csCode  
                                                   + Environment.NewLine;
                                           }));

                    if (PrinterSimulation)
                    {
                        CCodeQ.Enqueue(csCode);
                    }
                    break;

                }
            }





            dataAccess.UpdateCSerial(plan);



        }



        void andonManager_barcodeAlertEvent(object sender, BCScannerEventArgs e)
        {
            String barcode = e.ModelNumber + e.Timestamp + e.SerialNo.ToString("D4");

            Plan plan = null;

            if (dataAccess.UnitExists(barcode) == false)
            {
                return;
            }
           

            String assocationBarcode = String.Empty;
            String template = String.Empty;
            String modelName = String.Empty;
            if (e.ModelNumber.Contains("A")) // if body
            {
                String modelCode = e.ModelNumber.Substring(0, e.ModelNumber.Length - 1);

                foreach (Model m in Models)
                {
                    if (m.Code == modelCode)
                    {
                        if (m.Name.Contains("Dummy"))
                        {
                            template = DummyIntegratedBarcodeFile;

                        }
                        else
                            template = IntegratedBarcodeFile;
                        modelName = m.Name;
                        break;
                    }
                }

               


                if (dataAccess.UnitProcessed(barcode) == true)
                {
                    byte[] values = { 0, 1 };
                    modbusMaster.WriteMultipleRegister((ushort)3, (byte)0, (ushort)2, values);
                    F2Error1.Start();
                    tbMsg.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                             new Action(() =>
                                             {
                                                 tbMsg.Text += "F2:" + barcode + " Already Processed" 
                                                     + Environment.NewLine;
                                             }));
                    return;
                    //String iCode =
                    //    e.ModelNumber.Substring(0, e.ModelNumber.Length - 1) + e.Timestamp + CurrentFramePlan.IntegratedSerialNo.ToString("D4");
                    //if (!PrinterSimulation)
                    //{
                    //    bool result = false;
                    //    int count = 0;
                    //    do
                    //    {
                    //        result = PrinterManager.PrintBarcode("F2Printer", modelName, e.ModelNumber.Substring(0, e.ModelNumber.Length - 1),
                    //           DateTime.Now.ToString("yyMMdd"), CurrentFramePlan.IntegratedSerialNo.ToString("D4"), template);
                    //        count++;
                    //    } while ((result == false) && (count < 3));



                    //}
                    //tbMsg.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    //                          new Action(() =>
                    //                          {
                    //                              tbMsg.Text += DateTime.Now.ToString() + "Integrated Label Printed" + iCode
                    //                                  + Environment.NewLine;
                    //                          }));
                    //if (PrinterSimulation)
                    //{
                    //    ICodeQ.Enqueue(iCode);
                    //}
                   
                }


                assocationBarcode = dataAccess.UnitAssociated(Model.Type.BODY,
                    e.ModelNumber.Substring(0, e.ModelNumber.Length - 1), AssociationTimeout);

                
                if (assocationBarcode != String.Empty) // if association exists
                {
                    foreach (Plan p in FramePlans)
                    {
                        if (p.ModelCode == modelCode)
                        {
                            plan = p;
                            break;
                        }
                    }
                    plan.IntegratedSerialNo++;
                    String iCode =
                        e.ModelNumber.Substring(0, e.ModelNumber.Length - 1) + DateTime.Now.ToString("yyMMdd") + plan.IntegratedSerialNo.ToString("D4");
                    dataAccess.UpdateAssociation(barcode, Model.Type.BODY, assocationBarcode, iCode);
                    tbMsg.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                           new Action(() =>
                                           {
                                               tbMsg.Text +=  DateTime.Now.ToString() + "-F2:Main Body Unit Scanned - " + barcode
                                                   + Environment.NewLine;
                                           }));
                    

                    if (!PrinterSimulation)
                    {
                        bool result = false;
                        int count = 0;
                        do
                        {
                            result = PrinterManager.PrintBarcode("F2Printer", modelName, e.ModelNumber.Substring(0, e.ModelNumber.Length - 1),
                               DateTime.Now.ToString("yyMMdd"), plan.IntegratedSerialNo.ToString("D4"), template);
                            count++;
                        } while ((result == false) && (count < 3));



                    }
                    tbMsg.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                              new Action(() =>
                                              {
                                                  tbMsg.Text +=   DateTime.Now.ToString() + "-F2:Integrated Label Printed"  + iCode
                                                      + Environment.NewLine;
                                              }));
                    if (PrinterSimulation)
                    {
                        ICodeQ.Enqueue(iCode);
                    }
                    dataAccess.UpdateISerial(plan);
                }
                else
                {



                    dataAccess.InsertUnitAssociation(e.ModelNumber.Substring(0, e.ModelNumber.Length - 1), barcode, Model.Type.BODY);
                    tbMsg.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                           new Action(() =>
                                           {
                                               tbMsg.Text += DateTime.Now.ToString() + "-F2:Main Body Unit Scanned - barcode " 
                                                   + Environment.NewLine;
                                           }));


                }
            }
            else //if main frame
            {
                String modelCode = e.ModelNumber;

                foreach (Model m in Models)
                {
                    if (m.Code == modelCode)
                    {
                        if (m.Name.Contains("Dummy"))
                        {
                            template = DummyIntegratedBarcodeFile;
                        }
                        else
                            template = IntegratedBarcodeFile;
                        modelName = m.Name;
                        break;
                    }
                }

                if (dataAccess.UnitProcessed(barcode) == true)
                {
                    byte[] values = { 0,1 };
                    modbusMaster.WriteMultipleRegister((ushort)3, (byte)0, (ushort)2, values);
                    F2Error2.Start();
                    tbMsg.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                             new Action(() =>
                                             {
                                                 tbMsg.Text += "F2:" + barcode + " Already Processed" 
                                                     + Environment.NewLine;
                                             }));
                    //String iCode =
                    //    e.ModelNumber + e.Timestamp + CurrentFramePlan.IntegratedSerialNo.ToString("D4");
                    //if (!PrinterSimulation)
                    //{
                    //    bool result = false;
                    //    int count = 0;
                    //    do
                    //    {
                    //        result = PrinterManager.PrintBarcode("F2Printer", modelName, e.ModelNumber.Substring(0, e.ModelNumber.Length - 1),
                    //           DateTime.Now.ToString("yyMMdd"), CurrentFramePlan.IntegratedSerialNo.ToString("D4"), template);
                    //        count++;
                    //    } while ((result == false) && (count < 3));



                    //}
                    //tbMsg.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    //                          new Action(() =>
                    //                          {
                    //                              tbMsg.Text += DateTime.Now.ToString() + "Integrated Label Printed" + iCode
                    //                                  + Environment.NewLine;
                    //                          }));
                    //if (PrinterSimulation)
                    //{
                    //    ICodeQ.Enqueue(iCode);
                    //}
                    return;
                }



                assocationBarcode = dataAccess.UnitAssociated(Model.Type.FRAME, e.ModelNumber, AssociationTimeout);


                if (assocationBarcode != String.Empty) // if association exists
                {
                    foreach (Plan p in FramePlans)
                    {
                        if (p.ModelCode == modelCode)
                        {
                            plan = p;
                            break;
                        }
                    }
                    plan.IntegratedSerialNo++;
                    String iCode =
                       e.ModelNumber + DateTime.Now.ToString("yyMMdd") + plan.IntegratedSerialNo.ToString("D4");
                    dataAccess.UpdateAssociation(barcode, Model.Type.FRAME, assocationBarcode,iCode);
                    tbMsg.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                           new Action(() =>
                                           {
                                               tbMsg.Text += DateTime.Now.ToString() + "-F2:Main Frame Unit Scanned - " + barcode
                                                   + Environment.NewLine;
                                           }));
                   
                    if (!PrinterSimulation)
                    {
                       
                        bool result = false;
                        int count = 0;
                        do
                        {
                            result =
                           PrinterManager.PrintBarcode("F2Printer", modelName, e.ModelNumber, DateTime.Now.ToString("yyMMdd"),
                           plan.IntegratedSerialNo.ToString("D4"), template);
                            count++;
                        } while ((result == false) && (count < 3));


                    }
                    tbMsg.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                               new Action(() =>
                                               {
                                                   tbMsg.Text += DateTime.Now.ToString() + "-F2:Integrated Label Printed" + iCode
                                                       + Environment.NewLine;
                                               }));

                    if (PrinterSimulation)
                    {
                        ICodeQ.Enqueue(iCode);
                    }

                    dataAccess.UpdateISerial(plan);
                }
                else
                {

                    dataAccess.InsertUnitAssociation(e.ModelNumber, barcode, Model.Type.FRAME);
                    tbMsg.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                           new Action(() =>
                                           {
                                               tbMsg.Text +=  DateTime.Now.ToString() +  "-F2:Main Frame Unit Scanned - " + barcode
                                                   + Environment.NewLine;
                                           }));

                }

            }



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

                        if (CurrentBodyPlan == null)
                        {
                            MessageBox.Show(" Please Select Plan to continue",
                                "Application Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        if (CurrentBodyPlan.BSerialNo >= CurrentBodyPlan.Quantity)
                        {
                            MessageBox.Show("Current Plan Completed. Please Modify plan or Select another plan to continue",
                                "Application Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            CurrentBodyPlan.BSerialNo++;
                            String bcode = CurrentBodyPlan.ModelCode + "A" + DateTime.Now.ToString("yyMMdd")
                             + CurrentBodyPlan.BSerialNo.ToString("D4");

                            if (ScannerSimulation)
                                BCodeQ.Enqueue(bcode);

                            int printCount = m1PrintCount;
                            do
                            {

                                if (!PrinterSimulation)
                                {
                                    String template = CurrentBodyPlan.ModelName.Contains("Dummy") ? DummyM1BarcodeFile : M1BarcodeFile;
                                    bool result = false;
                                    int count = 0;
                                    do
                                    {
                                        result =
                                        PrinterManager.PrintBarcode("M1Printer", CurrentBodyPlan.ModelName,
                                        CurrentBodyPlan.ModelCode + "A", DateTime.Now.ToString("yyMMdd"),
                                         CurrentBodyPlan.BSerialNo.ToString("D4"), template);
                                        count++;
                                    } while ((result == false) && (count < 3));

                                }
                            } while (--printCount > 0);

                            dataAccess.InsertUnit(CurrentBodyPlan.ModelCode, Model.Type.BODY,
                                CurrentBodyPlan.BSerialNo);
                            dataAccess.UpdateBSerial(CurrentBodyPlan);


                        }
                    }
                    else if (e.StationId == 1)
                    {

                        if (CurrentFramePlan == null)
                        {
                            MessageBox.Show(" Please Select Plan to continue",
                                "Application Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        if (CurrentFramePlan.FSerialNo >= CurrentFramePlan.Quantity)
                        {
                            MessageBox.Show("Current Plan Completed. Please Modify plan or Select another plan to continue",
                                "Application Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {

                            CurrentFramePlan.FSerialNo++;
                            String fcode = CurrentFramePlan.ModelCode + DateTime.Now.ToString("yyMMdd")
                               + CurrentFramePlan.FSerialNo.ToString("D4");



                            if (ScannerSimulation)
                                FCodeQ.Enqueue(fcode);

                            int printCount = f1PrintCount;
                            do
                            {
                                String template = CurrentFramePlan.ModelName.Contains("Dummy") ? DummyF1BarcodeFile :  F1BarcodeFile;
                                if (!PrinterSimulation)
                                {
                                    bool result = false;
                                    int count = 0;
                                    do
                                    {

                                        result =
                                            PrinterManager.PrintBarcode("F1Printer", CurrentFramePlan.ModelName, CurrentFramePlan.ModelCode,
                                        DateTime.Now.ToString("yyMMdd"), CurrentFramePlan.FSerialNo.ToString("D4"),template);
                                        count++;
                                    } while ((result == false) && (count < 3));

                                }
                            } while (--printCount > 0 );
                            dataAccess.InsertUnit(CurrentFramePlan.ModelCode, Model.Type.FRAME,
                                CurrentFramePlan.FSerialNo);
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
                                                dbView = new DashBoardView(Users, CurrentUser.Name, PrinterManager);
                                                BaseGrid.Children.Add(dbView);
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
                    FrameLabelQuatitySelector1.IsEnabled = true;
                    continue;
                }
                else
                {
                    p1.FStatus = false;
                    dataAccess.UpdateFPlanStatus(p1);
                }
            }

            FrameLabelQuatitySelector2.IsEnabled = false;
            FrameLabelQuatitySelector3.IsEnabled = false;
            FrameLabelQuatitySelector4.IsEnabled = false;

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
                    FrameLabelQuatitySelector2.IsEnabled = true;
                    continue;
                }
                else
                {
                    p1.FStatus = false;
                    dataAccess.UpdateFPlanStatus(p1);
                }
            }
            FrameLabelQuatitySelector1.IsEnabled = false;
            FrameLabelQuatitySelector3.IsEnabled = false;
            FrameLabelQuatitySelector4.IsEnabled = false;

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
                    FrameLabelQuatitySelector3.IsEnabled = true;
                    continue;
                }
                else
                {
                    p1.FStatus = false;
                    dataAccess.UpdateFPlanStatus(p1);
                }
            }
            FrameLabelQuatitySelector1.IsEnabled = false;
            FrameLabelQuatitySelector2.IsEnabled = false;
            FrameLabelQuatitySelector4.IsEnabled = false;
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
                    FrameLabelQuatitySelector4.IsEnabled = true;
                    continue;
                }
                else
                {
                    p1.FStatus = false;
                    dataAccess.UpdateFPlanStatus(p1);
                }
            }
            FrameLabelQuatitySelector1.IsEnabled = false;
            FrameLabelQuatitySelector2.IsEnabled = false;
            FrameLabelQuatitySelector3.IsEnabled = false;
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
                    BodyLabelQuatitySelector1.IsEnabled = true;
                    continue;
                }
                else
                {
                    p1.BStatus = false;
                    dataAccess.UpdateBPlanStatus(p1);
                }
            }
            BodyLabelQuatitySelector2.IsEnabled = false;
            BodyLabelQuatitySelector3.IsEnabled = false;
            BodyLabelQuatitySelector4.IsEnabled = false;
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
                    BodyLabelQuatitySelector2.IsEnabled = true;
                    continue;
                }
                else
                {
                    p1.BStatus = false;
                    dataAccess.UpdateBPlanStatus(p1);
                }
            }
            BodyLabelQuatitySelector1.IsEnabled = false;
            BodyLabelQuatitySelector3.IsEnabled = false;
            BodyLabelQuatitySelector4.IsEnabled = false;
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
                    BodyLabelQuatitySelector3.IsEnabled = true;
                    continue;
                }
                else
                {
                    p1.BStatus = false;
                    dataAccess.UpdateBPlanStatus(p1);
                }
            }
            BodyLabelQuatitySelector1.IsEnabled = false;
            BodyLabelQuatitySelector2.IsEnabled = false;
            BodyLabelQuatitySelector4.IsEnabled = false;
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
                    BodyLabelQuatitySelector4.IsEnabled = true;
                    continue;
                }
                else
                {
                    p1.BStatus = false;
                    dataAccess.UpdateBPlanStatus(p1);
                }
            }
            BodyLabelQuatitySelector1.IsEnabled = false;
            BodyLabelQuatitySelector2.IsEnabled = false;
            BodyLabelQuatitySelector3.IsEnabled = false;
        }


        private void FrameLabelQuantitySelectionChanged(object sender, RoutedEventArgs e)
        {
            f1PrintCount = ((ComboBox)sender).SelectedIndex + 1;
        }

        private void BodyLabelQuantitySelectionChanged(object sender, RoutedEventArgs e)
        {
            m1PrintCount = ((ComboBox)sender).SelectedIndex + 1;
        }



        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (this.IsLoaded)
                {

                    if (BaseTabControl.SelectedIndex == 1)
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
                    else if (BaseTabControl.SelectedIndex == 3)
                    {

                    }
                }
            }


        }


        void WindowClosing(object sender, CancelEventArgs e)
        {
            if (andonManager != null)
                andonManager.stop();
        }


        private void MBmaster_OnException(ushort id, byte unit, byte function, byte exception)
        {
            string exc = "Modbus says error: ";
            switch (exception)
            {
                case Master.excIllegalFunction: exc += "Illegal function!"; break;
                case Master.excIllegalDataAdr: exc += "Illegal data adress!"; break;
                case Master.excIllegalDataVal: exc += "Illegal data value!"; break;
                case Master.excSlaveDeviceFailure: exc += "Slave device failure!"; break;
                case Master.excAck: exc += "Acknoledge!"; break;
                case Master.excGatePathUnavailable: exc += "Gateway path unavailbale!"; break;
                case Master.excExceptionTimeout: exc += "Slave timed out!"; break;
                case Master.excExceptionConnectionLost: exc += "Connection is lost!"; break;
                case Master.excExceptionNotConnected: exc += "Not connected!"; break;
            }

            MessageBox.Show(exc, "Modbus slave exception");
        }

        // ------------------------------------------------------------------------
        // Event for response data
        // ------------------------------------------------------------------------
        private void MBmaster_OnResponseData(ushort ID, byte unit, byte function, byte[] values)
        {
            // ------------------------------------------------------------------------
            // Identify requested data
            switch (ID)
            {
                case 1:
                    
                    break;
                case 2:

                    if( F1pressed == false )
                    {
                        if (values[1] == 1)
                        {
                            if (CurrentFramePlan == null)
                            {
                                MessageBox.Show(" Please Select Plan to continue",
                                    "Application Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }

                            if (CurrentFramePlan.FSerialNo >= CurrentFramePlan.Quantity)
                            {
                                MessageBox.Show("Current Plan Completed. Please Modify plan or Select another plan to continue",
                                    "Application Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else
                            {

                                CurrentFramePlan.FSerialNo++;
                                dataAccess.UpdateFSerial(CurrentFramePlan);
                                dataAccess.InsertUnit(CurrentFramePlan.ModelCode, Model.Type.FRAME,
                                    CurrentFramePlan.FSerialNo);

                                String fcode = CurrentFramePlan.ModelCode + DateTime.Now.ToString("yyMMdd")
                                   + CurrentFramePlan.FSerialNo.ToString("D4");



                                if (ScannerSimulation)
                                    FCodeQ.Enqueue(fcode);

                                int printCount = f1PrintCount;
                                do
                                {
                                    String template = CurrentFramePlan.ModelName.Contains("Dummy") ? DummyF1BarcodeFile : F1BarcodeFile;
                                    if (!PrinterSimulation)
                                    {
                                        bool result = false;
                                        int count = 0;
                                        do
                                        {

                                            result =
                                                PrinterManager.PrintBarcode("F1Printer", CurrentFramePlan.ModelName, CurrentFramePlan.ModelCode,
                                            DateTime.Now.ToString("yyMMdd"), CurrentFramePlan.FSerialNo.ToString("D4"), template);
                                            count++;
                                        } while ((result == false) && (count < 3));

                                    }
                                } while (--printCount > 0);
                                
                                

                                F1pressed = true;
                                F1Checkcount = 0;
                                DeviceAssociation deviceAssociation = dataAccess.getDeviceAssociation(2);

                                if (deviceAssociation == null) return;

                                String logMsg = String.Empty;

                                logMsg += deviceAssociation.Header ;
                                String lineName = deviceAssociation.LineName;
                                String stationName = deviceAssociation.StationName;

                                logMsg += stationName;

                                logMsg += "-PB pressed  " + fcode + "-Printed" + "----at: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                tbMsg.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                            new Action(() => { tbMsg.Text += logMsg + Environment.NewLine; }));

                            }
                        }
                    }
                    else
                    {
                        if (++F1Checkcount > 28)
                        {
                            F1pressed = false;

                        }
                    }

                    if (M1pressed == false)
                    {

                        if (values[3] == 1)
                        {
                            if (CurrentBodyPlan == null)
                            {
                                MessageBox.Show(" Please Select Plan to continue",
                                    "Application Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                            if (CurrentBodyPlan.BSerialNo >= CurrentBodyPlan.Quantity)
                            {
                                MessageBox.Show("Current Plan Completed. Please Modify plan or Select another plan to continue",
                                    "Application Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else
                            {
                                CurrentBodyPlan.BSerialNo++;
                                dataAccess.UpdateBSerial(CurrentBodyPlan);
                                dataAccess.InsertUnit(CurrentBodyPlan.ModelCode, Model.Type.BODY,
                                  CurrentBodyPlan.BSerialNo);
                                String bcode = CurrentBodyPlan.ModelCode + "A" + DateTime.Now.ToString("yyMMdd")
                                 + CurrentBodyPlan.BSerialNo.ToString("D4");

                                if (ScannerSimulation)
                                    BCodeQ.Enqueue(bcode);

                                int printCount = m1PrintCount;
                                do
                                {

                                    if (!PrinterSimulation)
                                    {
                                        String template = CurrentBodyPlan.ModelName.Contains("Dummy") ? DummyM1BarcodeFile : M1BarcodeFile;
                                        bool result = false;
                                        int count = 0;
                                        do
                                        {
                                            result =
                                            PrinterManager.PrintBarcode("M1Printer", CurrentBodyPlan.ModelName,
                                            CurrentBodyPlan.ModelCode + "A", DateTime.Now.ToString("yyMMdd"),
                                             CurrentBodyPlan.BSerialNo.ToString("D4"), template);
                                            count++;
                                        } while ((result == false) && (count < 3));

                                    }
                                } while (--printCount > 0);

                              
                                

                                M1pressed = true;
                                M1Checkcount = 0;

                                DeviceAssociation deviceAssociation = dataAccess.getDeviceAssociation(1);

                                if (deviceAssociation == null) return;

                                String logMsg = String.Empty;

                                logMsg += deviceAssociation.Header ;
                                String lineName = deviceAssociation.LineName;
                                String stationName = deviceAssociation.StationName;

                                logMsg +=  stationName;

                                logMsg += "-PB pressed  " + bcode + "-Printed" + "----at: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                tbMsg.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                            new Action(() => { tbMsg.Text += logMsg + Environment.NewLine; }));
                            }
                        }
                    }
                    else
                    {
                        if (++M1Checkcount > 8)
                        {
                            M1pressed = false;

                        }
                    }

                    break;
              
            }
        }


    }
}
