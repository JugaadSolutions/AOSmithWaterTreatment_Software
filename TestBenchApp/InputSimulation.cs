using ias.andonmanager;
using Printer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        bool PBSimulation = false;

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            List<LogEntry> log;
            LogEntry le;
            if (PBSimulation)
            {
                switch (e.Key)
                {
                    case Key.F1:
                         log= new List<LogEntry>();
                         le= new LogEntry(1, 0, "");
                         log.Add(le);
                        andonManager_andonAlertEvent(this, new AndonAlertEventArgs(DateTime.Now, 1, log));
                        break;
                    case Key.F2:
                         log = new List<LogEntry>();
                        le = new LogEntry(2, 0, "");
                        log.Add(le);
                        andonManager_andonAlertEvent(this, new AndonAlertEventArgs(DateTime.Now, 2, log));
                        break;

                    case Key.F3:
                        if (FCodeQ.Count > 0)
                        {

                            andonManager_actQtyAlertEvent(this, new actQtyScannerEventArgs(FCodeQ.Dequeue()));
                        }
                        break;

                    case Key.F4:
                        log = new List<LogEntry>();
                        le = new LogEntry(2, 0, "");
                        log.Add(le);
                        andonManager_andonAlertEvent(this, new AndonAlertEventArgs(DateTime.Now, 2, log));
                        break;

                    case Key.F5:
                        log = new List<LogEntry>();
                        le = new LogEntry(2, 0, "");
                        log.Add(le);
                        andonManager_andonAlertEvent(this, new AndonAlertEventArgs(DateTime.Now, 2, log));
                        break;

                    case Key.F6:
                        log = new List<LogEntry>();
                        le = new LogEntry(2, 0, "");
                        log.Add(le);
                        andonManager_andonAlertEvent(this, new AndonAlertEventArgs(DateTime.Now, 2, log));
                        break;

                   
                    default: break;
                }
            }
        }





        
    }
}
