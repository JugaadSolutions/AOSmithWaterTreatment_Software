using System;
using System.Collections.Generic;
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

namespace TestBenchApp.UIControls
{
    /// <summary>
    /// Interaction logic for ModelTypeSelection.xaml
    /// </summary>
    public partial class ModelTypeSelection : UserControl
    {
        public event EventHandler<ModelTypeEventArgs> SelectionEvent;
        public ModelTypeSelection()
        {
            InitializeComponent();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            ModelTypeEventArgs ma;
            if( ActualRadioButton.IsChecked == true )
            {
                ma = new ModelTypeEventArgs(MODEL_TYPE.ACTUAL);
            }
            else if( DummyElecRadioButton.IsChecked == true )
            {
                ma = new ModelTypeEventArgs(MODEL_TYPE.DUMMY_ELEC);
            }
            else if( DummyMechRadioButton.IsChecked == true )
            {
                ma = new ModelTypeEventArgs(MODEL_TYPE.DUMMY_MECH);
            }
            else
            {
                MessageBox.Show("Please Select Model Type", "Application Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public enum MODEL_TYPE {ACTUAL,DUMMY_ELEC, DUMMY_MECH};

    public class ModelTypeEventArgs : EventArgs
    {
        MODEL_TYPE ModelType;
        public ModelTypeEventArgs(MODEL_TYPE type)
        {
            ModelType = type;
        }
    }

}
