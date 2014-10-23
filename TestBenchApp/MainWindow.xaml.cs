using System;
using System.Windows;
using System.Windows.Threading;
using TestBenchApp.Line;

namespace TestBenchApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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

    }
}
