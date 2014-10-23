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
using System.Windows.Threading;

namespace TestBenchApp
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : UserControl
    {
        public event EventHandler<EventArgs> addClicked;

        public LoginPage()
        {
            InitializeComponent();
        }

        private void btnLogin_Click_1(object sender, RoutedEventArgs e)
        {
            if (addClicked != null)
                addClicked(this, new EventArgs());
        }
    }
}
