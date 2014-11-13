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

namespace TestBenchApp
{
    /// <summary>
    /// Interaction logic for ReprintManager.xaml
    /// </summary>
    public partial class ReprintManager : UserControl
    {

        public event EventHandler<EventArgs> btnDoneClicked;
        public event EventHandler<EventArgs> cbCSClicked;
        public event EventHandler<EventArgs> cbM1Clicked;
        public event EventHandler<EventArgs> cbF1Clicked;

        public event EventHandler<EventArgs> cbCSunChecked;
        public event EventHandler<EventArgs> cbM1unChecked;
        public event EventHandler<EventArgs> cbF1unChecked;

        public ReprintManager()
        {
            InitializeComponent();
            F1Reprintcb.Checked +=F1Reprintcb_Checked;
            M1Reprintcb.Checked += M1Reprintcb_Checked;
            CSReprintcb.Checked += CSReprintcb_Checked;

            F1Reprintcb.Unchecked += F1Reprintcb_Unchecked;
            M1Reprintcb.Unchecked += M1Reprintcb_Unchecked;
            CSReprintcb.Unchecked += CSReprintcb_Unchecked;
            
        }

        void CSReprintcb_Unchecked(object sender, RoutedEventArgs e)
        {
            if (cbCSunChecked != null)
                cbCSunChecked(this, new EventArgs());
        }

        void M1Reprintcb_Unchecked(object sender, RoutedEventArgs e)
        {
            if (cbM1unChecked != null)
                cbM1unChecked(this, new EventArgs());
        }

        void F1Reprintcb_Unchecked(object sender, RoutedEventArgs e)
        {
            if (cbF1unChecked != null)
                cbF1unChecked(this, new EventArgs());
        }

        void CSReprintcb_Checked(object sender, RoutedEventArgs e)
        {
            if (cbCSClicked != null)
                cbCSClicked(this, new EventArgs());
        }

        void M1Reprintcb_Checked(object sender, RoutedEventArgs e)
        {
            if (cbM1Clicked != null)
                cbM1Clicked(this, new EventArgs());
        }

        private void F1Reprintcb_Checked(object sender, RoutedEventArgs e)
        {
            if (cbF1Clicked != null)
                cbF1Clicked(this, new EventArgs());
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            if (btnDoneClicked != null)
                btnDoneClicked(this, new EventArgs());
        }

        


    }
}
