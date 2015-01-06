using shared.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public event EventHandler<ReprintArgs> TOKReprint;
        public event EventHandler<BatchPrintArgs> BatchPrint;
        ObservableCollection<Model> Models;

        DataAccess dataAccess;
        DataTable f1Serial, M1Serial, CSSerial,Tok;

        public ReprintManager()
        {
            InitializeComponent();

            dataAccess = new DataAccess();
            f1Serial = dataAccess.GetReprintSerialNos("F1");
            M1Serial = dataAccess.GetReprintSerialNos("M1");
            CSSerial = dataAccess.GetReprintSerialNos("CS");
            Tok = dataAccess.GetReprintSerialNos("TOK");

            F1ReprintGrid.DataContext = f1Serial;
            M1ReprintGrid.DataContext = M1Serial;
            CSReprintGrid.DataContext = CSSerial;
            TOKReprintGrid.DataContext = Tok;

            loadModels();

        }


        void loadModels()
        {
            
            Models = dataAccess.GetModels();
            ModelSelectorComboBox.DataContext = Models;
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


                F1Reprint(this, new ReprintArgs(model,barcode.Substring(0,4), barcode.Substring(4, 6), barcode.Substring(10, 4)));
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
                M1Reprint(this, new ReprintArgs(model,barcode.Substring(0,5), barcode.Substring(5,6),barcode.Substring(11,4)));
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

                String barcode = (String)((DataRowView)CSReprintGrid.SelectedItem).Row["Barcode"];
                CSReprint(this, new ReprintArgs("", barcode.Substring(0, 4), barcode.Substring(4, 6), barcode.Substring(10, 4)));
            }
        }

        private void TOKReprintButton_Click(object sender, RoutedEventArgs e)
        {
            if (TOKReprintGrid.SelectedIndex == -1)
            {
                MessageBox.Show(" Please select Serial No to reprint", "Applicaiton Info", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }
            if (TOKReprint != null)
            {
                String barcode = (String)((DataRowView)TOKReprintGrid.SelectedItem).Row["Barcode"];
                String modelCode =  barcode.Substring(0, 4);
                String model = String.Empty;
                foreach ( Model m in Models )
                {
                    if( modelCode == m.Code )
                    {
                        model = m.Name;
                        break;
                    }
                }
                


                TOKReprint(this, new ReprintArgs(model,modelCode, barcode.Substring(4, 6), barcode.Substring(10, 4)));
            }
        }


        private void BatchPrintButton_Click(object sender, RoutedEventArgs e)
        {
            if( PrintDatePicker.SelectedDate== null )
                
            {
                MessageBox.Show(" Please Select Date of BatchPrint", "Applicaiton Info", MessageBoxButton.OK,
                MessageBoxImage.Information);
                return;
            }
            if( ModelSelectorComboBox.SelectedIndex == -1 )
                
            {
                MessageBox.Show(" Please Select Model", "Applicaiton Info", MessageBoxButton.OK,
                MessageBoxImage.Information);
                return;
            }
            String model = Models[ModelSelectorComboBox.SelectedIndex].Name;
            String code = Models[ModelSelectorComboBox.SelectedIndex].Code;

             

            String date = PrintDatePicker.SelectedDate.Value.ToString("yyMMdd");

            int serialNo ;
            if (int.TryParse(InitialSerialNoTextBox.Text, out serialNo) == false)
            {
                MessageBox.Show(" Invalid Initial Serial No", "Applicaiton Info", MessageBoxButton.OK,
                MessageBoxImage.Information);
                return;
            }

            int quantity;

            if( int.TryParse(QuantityTextBox.Text,out quantity) == false )
            {
                MessageBox.Show(" Invalid Quantity of Reprints", "Applicaiton Info", MessageBoxButton.OK,
                MessageBoxImage.Information);
                return;
            }
           
            BatchPrintArgs bpa;
            if (F1RadioButton.IsChecked == true)
            {
                bpa = new BatchPrintArgs(model, code, date, serialNo, quantity, REPRINT_STAGE.F1);
            }
            else if (M1RadioButton.IsChecked == true)
            {
                bpa = new BatchPrintArgs(model, code, date, serialNo, quantity, REPRINT_STAGE.M1);
            }
            else if (IntegratedRadioButton.IsChecked == true)
            {
                bpa = new BatchPrintArgs(model, code, date, serialNo, quantity, REPRINT_STAGE.INTEGRATED);
            }
            else if (CombinationRadioButton.IsChecked == true)
            {
                bpa = new BatchPrintArgs(model, code, date, serialNo, quantity, REPRINT_STAGE.COMBINATION);
            }
            else
            {
                MessageBox.Show(" Please Select stage of Reprint", "Applicaiton Info", MessageBoxButton.OK,
               MessageBoxImage.Information);
                return;
            }

            if (BatchPrint != null)
                BatchPrint(this, bpa);

            PrintDatePicker.SelectedDate = null;
            ModelSelectorComboBox.SelectedIndex = -1;
            InitialSerialNoTextBox.Clear();
            QuantityTextBox.Clear();

            F1RadioButton.IsChecked = M1RadioButton.IsChecked = IntegratedRadioButton.IsChecked
                = CombinationRadioButton.IsChecked = false;

        }


    }

    public class ReprintArgs : EventArgs
    {
        public String Model;
        public String Code;
        public String Date;
        public String SerialNo;

        public ReprintArgs(String model,String code,String date,String serialNo)
        {
            Model = model;
            Code = code;
            Date = date;
            SerialNo = serialNo;
        }
    }

    public enum REPRINT_STAGE { F1, M1, INTEGRATED, COMBINATION };


    public class BatchPrintArgs : EventArgs
    {
        public String Model;
        public String Code;
        public String Date;
        public int SerialNo;
        public int Quantity;
        public REPRINT_STAGE ReprintStage;
        public BatchPrintArgs(String model, String code, String date, int serialNo,int qty, REPRINT_STAGE rps)
        {
            Model = model;
            Code = code;
            Date = date;
            SerialNo = serialNo;
            Quantity = qty;
            ReprintStage = rps;
        }
    }
}
