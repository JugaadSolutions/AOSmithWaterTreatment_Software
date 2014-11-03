
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


namespace TestBenchApp.DashBoard
{
    /// <summary>
    /// Interaction logic for DashBoardView.xaml
    /// </summary>

    public partial class DashBoardView : UserControl
    {
        

        BackgroundWorker worker;

        public Boolean Admin = false;

        DataAccess dataAccess;

        public  DashBoardView(Users users,String currentUser)
        {
            InitializeComponent();
            
            CurrentUser = currentUser;
            Users = users;

            dataAccess = new DataAccess();
            

           

            extendConstructor();


         


           

            
            
           
        }

        partial void  extendConstructor();

        



        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {
            

        }

       

        private void Reports_Click(object sender, RoutedEventArgs e)
        {

        }




       

     

     

        

      
       

        
  

       

       

       
    }
}
