
using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Timers;
using TestBenchApp.Entity;
using TestBenchApp.Line;
using Printer;
using shared;
using System.Collections.ObjectModel;
using shared.Entity;


namespace TestBenchApp.DashBoard
{
    /// <summary>
    /// Interaction logic for DashBoardView.xaml
    /// </summary>

    public partial class DashBoardView : UserControl
    {

        PrinterManager PrinterManager;

        BackgroundWorker worker;
        
        public Boolean Admin = false;

        DataAccess dataAccess;
        ObservableCollection<Model> Models;

        public DashBoardView(Users users, String currentUser, PrinterManager printerManager)
        {
            InitializeComponent();
            
            CurrentUser = currentUser;
            Users = users;

            dataAccess = new DataAccess();

            Models = dataAccess.GetModels();
           

            extendConstructor();

            PrinterManager = printerManager;

         


           

                       
           
        }

        partial void  extendConstructor();

        



        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {
            

        }

       

        



        






       

     

     

        

      
       

        
  

       

       

       
    }
}
