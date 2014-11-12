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
        ObservableCollection<Model> Models;
        public event EventHandler<EventArgs> CancelClicked;
        public event EventHandler<TestEventArgs> TestPrintBtnClicked;

        public ModelsManager()
        {
            InitializeComponent();
            ModelAddDeleteControl.selectionChanged += ModelAddDeleteControl_selectionChanged;

            ModelAddDeleteControl.deleteClicked += ModelAddDeleteControl_deleteClicked;
            ((Label)ModelAddDeleteControl.aMDGroupBox.Header).Content = "Models";

            dataAccess = new DataAccess();
            loadModels();

        }

        void ModelAddDeleteControl_deleteClicked(object sender, 
            UIControls.AddModifyDeleteSelectionChangedEventArgs e)
        {
            if(e.SelectedIndex == -1 ) return;
            MessageBox.Show("Model will be Deleted. Press OK to Confirm", "Application Info",
              MessageBoxButton.OK, MessageBoxImage.Exclamation);
            dataAccess.DeleteModel(Models[e.SelectedIndex]);

            loadModels();
        }

        void loadModels()
        {
            ModelDetailsControl.Visibility = System.Windows.Visibility.Hidden;
            Models = dataAccess.GetModels();
            ModelAddDeleteControl.aMDGroupBox.DataContext = Models;
        }

      

       

        void ModelAddDeleteControl_selectionChanged(object sender, 
            UIControls.AddModifyDeleteSelectionChangedEventArgs e)
        {
            gbModelDetails.Visibility = System.Windows.Visibility.Hidden;

            if (e.SelectedIndex == -1) return;

            ModelDetailsControl.CurrentModel = Models[e.SelectedIndex];

            gbModelDetails.Visibility = System.Windows.Visibility.Visible;
        }

        

        
        private void btnSave_Click_1(object sender, RoutedEventArgs e)
        {
            Model m = ModelDetailsControl.GetModel();

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
