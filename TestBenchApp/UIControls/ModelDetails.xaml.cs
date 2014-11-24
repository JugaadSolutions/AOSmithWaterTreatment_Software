using shared.Entity;
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
using TestBenchApp.Entity;

namespace TestBenchApp.UIControls
{
    /// <summary>
    /// Interaction logic for ModelDetails.xaml
    /// </summary>
    public partial class ModelDetails : UserControl
    {
        Model currentModel;
        public Model CurrentModel
        {
            get { return currentModel; }

            set
            {
                currentModel = value;
                this.DataContext = currentModel;
                this.Visibility = System.Windows.Visibility.Visible;
            }
        }
        public ModelDetails()
        {
            InitializeComponent();

        }

       

        internal Model GetModel()
        {
            double netQty = 0;
            double capacity = 0;
            int mrp = 0,width = 0, height = 0, depth = 0;
          
            if( (Double.TryParse(NetQuantityTextBox.Text, out netQty) ==false)
                || (Double.TryParse(StorageCapacityTextBox.Text,out capacity ) == false)
                || (int.TryParse(MRPTextBox.Text,out mrp) == false)
                || (int.TryParse(WidthTextBox.Text , out width) == false)
                || (int.TryParse(HeightTextBox.Text , out height)== false)
                || (int.TryParse(DepthTextBox.Text , out depth)== false))
            {
                MessageBox.Show("Invalid Data!! Please Verify ", "Application Info", MessageBoxButton.OK,
                        MessageBoxImage.Warning );
                return null;
            }

            CurrentModel.Product = ProductTextBox.Text;
            CurrentModel.ProductNumber = ProductNoTextBox.Text;
            CurrentModel.Name = ModelNameTextBox.Text;
            CurrentModel.NetQuantity = netQty;
            CurrentModel.StorageCapacity = capacity;
            CurrentModel.Code = CodeTextBox.Text;
            CurrentModel.MRP = mrp;
            CurrentModel.CustomerCare = CustomerCareNoTextBox.Text;
            CurrentModel.Email = EmailTextBox.Text;
            CurrentModel.EAN = EANCodeTextBox.Text;
            CurrentModel.Width = width;
            CurrentModel.Height = height;
            CurrentModel.Depth = depth;

            return CurrentModel;
                 

        }
    }
}
