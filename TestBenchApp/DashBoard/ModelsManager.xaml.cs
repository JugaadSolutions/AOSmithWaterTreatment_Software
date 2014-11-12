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

        public ModelsManager()
        {
            InitializeComponent();
            ModelAddDeleteControl.selectionChanged += ModelAddDeleteControl_selectionChanged;
            ModelAddDeleteControl.addClicked += ModelAddDeleteControl_addClicked;
            ModelAddDeleteControl.deleteClicked += ModelAddDeleteControl_deleteClicked;

            ((Label)ModelAddDeleteControl.aMDGroupBox.Header).Content = "Models";

            dataAccess = new DataAccess();
            loadModels();

        }

        void loadModels()
        {

            Models = dataAccess.GetModels();
            ModelAddDeleteControl.aMDGroupBox.DataContext = Models;
        }
        void ModelAddDeleteControl_deleteClicked(object sender, EventArgs e)
        {
            
        }

        void ModelAddDeleteControl_addClicked(object sender, EventArgs e)
        {
            
        }

        void ModelAddDeleteControl_selectionChanged(object sender, 
            UIControls.AddModifyDeleteSelectionChangedEventArgs e)
        {
            gbModelDetails.Visibility = System.Windows.Visibility.Hidden;

            ModelDetailsControl.CurrentModel = Models[e.SelectedIndex];

            gbModelDetails.Visibility = System.Windows.Visibility.Visible;
        }

        

        
        private void btnSave_Click_1(object sender, RoutedEventArgs e)
        {
            Model m = ModelDetailsControl.GetModel();

            dataAccess.UpdateModel(m);
            MessageBox.Show("Model Saved Successfully", "Application Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnSaveAs_Click_1(object sender, RoutedEventArgs e)
        {
             Model m = ModelDetailsControl.GetModel();
             
            dataAccess.InsertModel(m);

            MessageBox.Show("Model Saved Successfully", "Application Info", MessageBoxButton.OK, MessageBoxImage.Information);


        }

        private void btnTestPrint_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancel_Click_1(object sender, RoutedEventArgs e)
        {
            if (CancelClicked != null)
                CancelClicked(this, new EventArgs());
        }

       
    }
}
