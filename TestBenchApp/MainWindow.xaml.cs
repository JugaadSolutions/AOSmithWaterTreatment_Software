﻿using ias.andonmanager;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Threading;
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
        String _dbConnectionString = String.Empty;
        DataAccess dataAccess = null;
        Queue<int> deviceQ = null;
        String[] comLayers;
        AndonManager.MODE Mode = AndonManager.MODE.NONE;
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
            andonManager.start();
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
            tabPlan.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                            new Action(() =>
                                            {
                                                BaseGrid.Children.Clear();
                                                LoginPage lp = new LoginPage();
                                                lp.addClicked += lp_addClicked;
                                                BaseGrid.Children.Add(lp);
                                            }));
        }

        void lp_addClicked(object sender, EventArgs e)
        {
            BaseGrid.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                            new Action(() =>
                                            {
                                                BaseGrid.Children.Clear();
                                                Plan p = new Plan();
                                                p.addClicked += p_addClicked;
                                                BaseGrid.Children.Add(p);
                                            }));
        }

        void p_addClicked(object sender, EventArgs e)
        {
            tabPlan.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                          new Action(() =>
                                          {
                                              BaseGrid.Children.Clear();
                                              LoginPage lp = new LoginPage();
                                              lp.addClicked += lp_addClicked;
                                              BaseGrid.Children.Add(lp);
                                          }));
        }

        private void tabProductionData_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void cb1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cb4_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cb3_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cb2_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
