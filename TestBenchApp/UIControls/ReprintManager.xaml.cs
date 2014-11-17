using System;
using System.Collections.Generic;
using System.Data;
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

namespace TestBenchApp
{
    /// <summary>
    /// Interaction logic for ReprintManager.xaml
    /// </summary>
    public partial class ReprintManager : UserControl
    {

        public event EventHandler<EventArgs> btnDoneClicked;
        public event EventHandler<ReprintArgs> CSReprint;
        public event EventHandler<ReprintArgs> M1Reprint;
        public event EventHandler<ReprintArgs> F1Reprint;

        DataAccess dataAccess;
        DataTable f1Serial, M1Serial, CSSerial;

        public ReprintManager()
        {
            InitializeComponent();

            dataAccess = new DataAccess();
            f1Serial = dataAccess.GetReprintSerialNos("F1");
            M1Serial = dataAccess.GetReprintSerialNos("M1");
            CSSerial = dataAccess.GetReprintSerialNos("CS");

            F1ReprintGrid.DataContext = f1Serial;
            M1ReprintGrid.DataContext = M1Serial;
            CSReprintGrid.DataContext = CSSerial;

        }

       
       

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            if (btnDoneClicked != null)
                btnDoneClicked(this, new EventArgs());
        }

        private void F1ReprintButton_Click(object sender, RoutedEventArgs e)
        {
            if( F1ReprintGrid.SelectedIndex == -1 )
            {
                MessageBox.Show( " Please select Serial No to reprint" , "Applicaiton Info",MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }
            if (F1Reprint != null)
            {
                String model = (String)((DataRowView)F1ReprintGrid.SelectedItem).Row["Model"];
                String barcode =(String) ((DataRowView)F1ReprintGrid.SelectedItem).Row["Barcode"];


                F1Reprint(this, new ReprintArgs(model,barcode));
            }
        }

        private void M1ReprintButton_Click(object sender, RoutedEventArgs e)
        {
            if (M1ReprintGrid.SelectedIndex == -1)
            {
                MessageBox.Show(" Please select Serial No to reprint", "Applicaiton Info", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }
            if (M1Reprint != null)
            {
                String model = (String)((DataRowView)M1ReprintGrid.SelectedItem).Row["Model"];
                String barcode = (String)((DataRowView)M1ReprintGrid.SelectedItem).Row["Barcode"];
                M1Reprint(this, new ReprintArgs(model,barcode));
            }
        }

        private void CombinationReprintButton_Click(object sender, RoutedEventArgs e)
        {
            if (CSReprintGrid.SelectedIndex == -1)
            {
                MessageBox.Show(" Please select Serial No to reprint", "Applicaiton Info", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }
            if (CSReprint != null)
            {
                
                String barcode = (String)((DataRow)CSReprintGrid.SelectedItem)["Barcode"];
                CSReprint(this, new ReprintArgs("",barcode));
            }
        }

        


    }

    public class ReprintArgs : EventArgs
    {
        public String Model;
        public String Barcode;

        public ReprintArgs(String model,String bcode)
        {
            Model = model;
            Barcode = bcode;
        }
    }
}
