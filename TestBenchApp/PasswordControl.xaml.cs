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

namespace GEMSPL.DashBoard.Manage
{
    /// <summary>
    /// Interaction logic for PasswordControl.xaml
    /// </summary>
    public partial class PasswordControl : UserControl
    {
        public event EventHandler<PasswordChangeEventArgs> PasswordChangeEvent;
        public event EventHandler<PasswordCancelEventArgs> PasswordCancelEvent;

        String Oldpassword = String.Empty;
        public PasswordControl(String oldpassword)
        {
            InitializeComponent();

            Oldpassword = oldpassword;
        }

        private void PasswordChange_Click_1(object sender, RoutedEventArgs e)
        {
            if (OldPassword.Password != Oldpassword)
            {
                MessageBox.Show("Incorrect Old Password. Please try again", 
                    "Password Info", MessageBoxButton.OK, MessageBoxImage.Information);
                OldPassword.Clear();
                NewPassword.Clear();
                OldPassword.Focus();
                return;
            }
            if( PasswordChangeEvent != null )
                PasswordChangeEvent(this, new PasswordChangeEventArgs(NewPassword.Password));
        }

        private void PasswordCancel_Click_1(object sender, RoutedEventArgs e)
        {
            if (PasswordCancelEvent != null)
                PasswordCancelEvent(this, new PasswordCancelEventArgs());
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            OldPassword.Focus();
        }
    }

    public class PasswordChangeEventArgs : EventArgs
    {
        public String Password { get; set; }

        public PasswordChangeEventArgs(String _password)
        {
            Password = _password;
        }
    }

    public class PasswordCancelEventArgs : EventArgs
    {
       
    }
}
