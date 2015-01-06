using shared.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace TestBenchApp.DashBoard
{
    /// <summary>
    /// Interaction logic for combStickerManager.xaml
    /// </summary>
    public partial class ModelsManager : UserControl
    {
        DataAccess dataAccess;
        ObservableCollection<Model> ActualModels;
        ObservableCollection<Model> DummyElecModels;
        ObservableCollection<Model> DummyMechModels;
        public event EventHandler<EventArgs> CancelClicked;
        public event EventHandler<TestEventArgs> TestPrintBtnClicked;

        public ModelsManager()
        {
            InitializeComponent();
            ActualModelAddDeleteControl.selectionChanged += ModelAddDeleteControl_selectionChanged;

            ActualModelAddDeleteControl.deleteClicked += ModelAddDeleteControl_deleteClicked;
            ((Label)ActualModelAddDeleteControl.aMDGroupBox.Header).Content = "Actual Models";

            DummyElecModelAddDeleteControl.selectionChanged += DummyElecModelAddDeleteControl_selectionChanged;
            DummyElecModelAddDeleteControl.addClicked += DummyElecModelAddDeleteControl_addClicked;
            DummyElecModelAddDeleteControl.deleteClicked += DummyElecModelAddDeleteControl_deleteClicked;
            ((Label)DummyElecModelAddDeleteControl.aMDGroupBox.Header).Content = "Dummy Elec Models";


            DummyMechModelAddDeleteControl.selectionChanged += DummyMechModelAddDeleteControl_selectionChanged;
            DummyMechModelAddDeleteControl.addClicked += DummyMechModelAddDeleteControl_addClicked;
            DummyMechModelAddDeleteControl.deleteClicked += DummyMechModelAddDeleteControl_deleteClicked;
            ((Label)DummyMechModelAddDeleteControl.aMDGroupBox.Header).Content = "Dummy Mech Models";

            dataAccess = new DataAccess();
            loadModels();

        }

        void DummyElecModelAddDeleteControl_deleteClicked(object sender, UIControls.AddModifyDeleteSelectionChangedEventArgs e)
        {
            if (e.SelectedIndex == -1) return;
            MessageBox.Show("Model will be Deleted. Press OK to Confirm", "Application Info",
              MessageBoxButton.OK, MessageBoxImage.Exclamation);
            dataAccess.DeleteModel(DummyMechModels[e.SelectedIndex]);

            loadModels();
        }

        void DummyMechModelAddDeleteControl_deleteClicked(object sender, UIControls.AddModifyDeleteSelectionChangedEventArgs e)
        {
            if (e.SelectedIndex == -1) return;
            MessageBox.Show("Model will be Deleted. Press OK to Confirm", "Application Info",
              MessageBoxButton.OK, MessageBoxImage.Exclamation);
            dataAccess.DeleteModel(DummyElecModels[e.SelectedIndex]);

            loadModels();
        }

        void DummyMechModelAddDeleteControl_addClicked(object sender, EventArgs e)
        {
            gbModelDetails.Visibility = System.Windows.Visibility.Hidden;


            Model m = new Model();

            m.Code = DummyMechModels[0].Code;
            m.CustomerCare = DummyMechModels[0].CustomerCare;
            m.Depth = DummyMechModels[0].Depth;
            m.Email = DummyMechModels[0].Email;
            m.Height = DummyMechModels[0].Height;
            m.ModelType = DummyMechModels[0].ModelType;
            m.MRP = DummyMechModels[0].MRP;
            m.Name = DummyMechModels[0].Name;
            m.NetQuantity = DummyMechModels[0].NetQuantity;
            m.Product = DummyMechModels[0].Product;
            m.ProductNumber = DummyMechModels[0].ProductNumber;
            m.StorageCapacity = DummyMechModels[0].StorageCapacity;
            m.Width = DummyMechModels[0].Width;

            ModelDetailsControl.CurrentModel = m;

            gbModelDetails.Visibility = System.Windows.Visibility.Visible;
        }

        void DummyMechModelAddDeleteControl_selectionChanged(object sender, UIControls.AddModifyDeleteSelectionChangedEventArgs e)
        {
            gbModelDetails.Visibility = System.Windows.Visibility.Hidden;

            if (e.SelectedIndex == -1) return;

            ModelDetailsControl.CurrentModel = DummyMechModels[e.SelectedIndex];

            gbModelDetails.Visibility = System.Windows.Visibility.Visible;
        }

        void DummyElecModelAddDeleteControl_addClicked(object sender, EventArgs e)
        {
            gbModelDetails.Visibility = System.Windows.Visibility.Hidden;


            Model m = new Model();

            m.Code = DummyElecModels[0].Code;
            m.CustomerCare = DummyElecModels[0].CustomerCare;
            m.Depth = DummyElecModels[0].Depth;
            m.Email = DummyElecModels[0].Email;
            m.Height = DummyElecModels[0].Height;
            m.ModelType = DummyElecModels[0].ModelType;
            m.MRP = DummyElecModels[0].MRP;
            m.Name = DummyElecModels[0].Name;
            m.NetQuantity = DummyElecModels[0].NetQuantity;
            m.Product = DummyElecModels[0].Product;
            m.ProductNumber = DummyElecModels[0].ProductNumber;
            m.StorageCapacity = DummyElecModels[0].StorageCapacity;
            m.Width = DummyElecModels[0].Width;

            ModelDetailsControl.CurrentModel = m;

            gbModelDetails.Visibility = System.Windows.Visibility.Visible;
        }

        void DummyElecModelAddDeleteControl_selectionChanged(object sender, UIControls.AddModifyDeleteSelectionChangedEventArgs e)
        {
            gbModelDetails.Visibility = System.Windows.Visibility.Hidden;

            if (e.SelectedIndex == -1) return;

            ModelDetailsControl.CurrentModel = DummyElecModels[e.SelectedIndex];

            gbModelDetails.Visibility = System.Windows.Visibility.Visible;
        }

        void ModelAddDeleteControl_deleteClicked(object sender, 
            UIControls.AddModifyDeleteSelectionChangedEventArgs e)
        {
            if(e.SelectedIndex == -1 ) return;
            MessageBox.Show("Model will be Deleted. Press OK to Confirm", "Application Info",
              MessageBoxButton.OK, MessageBoxImage.Exclamation);
            dataAccess.DeleteModel(ActualModels[e.SelectedIndex]);

            loadModels();
        }

        void loadModels()
        {
            ModelDetailsControl.Visibility = System.Windows.Visibility.Hidden;

            ActualModels = dataAccess.GetModels(MODEL_TYPE.ACTUAL);
            ActualModelAddDeleteControl.aMDGroupBox.DataContext = ActualModels;

            DummyElecModels = dataAccess.GetModels(MODEL_TYPE.DUMMY_ELEC);
            DummyElecModelAddDeleteControl.aMDGroupBox.DataContext = DummyElecModels;

            DummyMechModels = dataAccess.GetModels(MODEL_TYPE.DUMMY_MECH);
            DummyMechModelAddDeleteControl.aMDGroupBox.DataContext = DummyMechModels;

        }

      

       

        void ModelAddDeleteControl_selectionChanged(object sender, 
            UIControls.AddModifyDeleteSelectionChangedEventArgs e)
        {
            gbModelDetails.Visibility = System.Windows.Visibility.Hidden;

            if (e.SelectedIndex == -1) return;

            ModelDetailsControl.CurrentModel = ActualModels[e.SelectedIndex];

            gbModelDetails.Visibility = System.Windows.Visibility.Visible;
        }

        

        
        private void btnSave_Click_1(object sender, RoutedEventArgs e)
        {
            Model m = ModelDetailsControl.GetModel();

            if( m.SlNo == -1 )
                dataAccess.InsertModel(m);

            else
                dataAccess.UpdateModel(m);
            MessageBox.Show("Model Saved Successfully", "Application Info", 
                MessageBoxButton.OK, MessageBoxImage.Information);

            loadModels();

        }

        private void btnSaveAs_Click_1(object sender, RoutedEventArgs e)
        {
             Model m = ModelDetailsControl.GetModel();
             
            dataAccess.InsertModel(m);

            MessageBox.Show("Model Saved Successfully", "Application Info", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            loadModels();


        }

        private void btnTestPrint_Click_1(object sender, RoutedEventArgs e)
        {
            Model m = ModelDetailsControl.GetModel();

            TestEventArgs t = new TestEventArgs(m);

            if (TestPrintBtnClicked != null)
                TestPrintBtnClicked(this, t);

        }

        private void btnCancel_Click_1(object sender, RoutedEventArgs e)
        {
            if (CancelClicked != null)
                CancelClicked(this, new EventArgs());
        }

        private void ModelAddDeleteControl_addClicked(object sender, EventArgs e)
        {
            gbModelDetails.Visibility = System.Windows.Visibility.Hidden;


            Model m = new Model();
            
            m.Code = ActualModels[0].Code;
            m.CustomerCare = ActualModels[0].CustomerCare;
            m.Depth = ActualModels[0].Depth;
            m.Email = ActualModels[0].Email;
            m.Height = ActualModels[0].Height;
            m.ModelType = ActualModels[0].ModelType;
            m.MRP = ActualModels[0].MRP;
            m.Name = ActualModels[0].Name;
            m.NetQuantity = ActualModels[0].NetQuantity;
            m.Product = ActualModels[0].Product;
            m.ProductNumber = ActualModels[0].ProductNumber;
            m.StorageCapacity = ActualModels[0].StorageCapacity;
            m.Width = ActualModels[0].Width;

            ModelDetailsControl.CurrentModel = m;

            gbModelDetails.Visibility = System.Windows.Visibility.Visible;
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (this.IsLoaded)
                {

                    gbModelDetails.Visibility = System.Windows.Visibility.Hidden;
                }
            }


        }


       
    }


    #region EVENT ARGS
    public class TestEventArgs : EventArgs
    {
        public Model m { get; set; }


        public TestEventArgs(Model model)
        {
            m = model;
        }
    }




    #endregion
}
