using shared;
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

namespace TestBenchApp.DashBoard
{
    /// <summary>
    /// Interaction logic for PasswordControl.xaml
    /// </summary>
    public partial class PasswordControl : UserControl
    {
        public event EventHandler<PasswordChangeEventArgs> PasswordChangeEvent;
        public event EventHandler<PasswordCancelEventArgs> PasswordCancelEvent;

        Users Users;
        public PasswordControl(Users users)
        {
            InitializeComponent();

            Users = users;
            UserSelector.DataContext = Users;
        }

        private void PasswordChange_Click_1(object sender, RoutedEventArgs e)
        {

            if (OldPassword.Password != ((User)UserSelector.SelectedItem ).Password)
            {
                MessageBox.Show("Incorrect Old Password. Please try again", 
                    "Password Info", MessageBoxButton.OK, MessageBoxImage.Information);
                OldPassword.Clear();
                NewPassword.Clear();
                OldPassword.Focus();
                return;
            }
            if (PasswordChangeEvent != null)
            {
                User u = ((User)UserSelector.SelectedItem);
                PasswordChangeEvent(this, new PasswordChangeEventArgs(new User(u.Name, NewPassword.Password)));
            }
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

        private void UserSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

    public class PasswordChangeEventArgs : EventArgs
    {
        public User User{ get; set; }

        public PasswordChangeEventArgs(User u)
        {
            User = u;
        }
    }

    public class PasswordCancelEventArgs : EventArgs
    {
       
    }
}
