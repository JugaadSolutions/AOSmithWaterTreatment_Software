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

namespace TestBenchApp.DashBoard
{
    /// <summary>
    /// Interaction logic for combStickerManager.xaml
    /// </summary>
    public partial class ModelsManager : UserControl
    {
        public ModelsManager()
        {
            InitializeComponent();
        }

        public event EventHandler<EventArgs> btnCancelClicked;

        private void btnSaveCombSticker_Click(object sender, RoutedEventArgs e)
        {


        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (btnCancelClicked != null)
                btnCancelClicked(this, new EventArgs());
        }

        private void ProductNameSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnSaveAsCombSticker_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnTestPrintCombSticker_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ProductNoSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ModelSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
