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
using TestBenchApp.Entity;

namespace TestBenchApp
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : UserControl
    {
        public event EventHandler<LoginEventArgs> LoginEvent;
        public event EventHandler<EventArgs> LoginFailEvent;
        Users Users;
        public LoginPage(Users users)
        {
            InitializeComponent();
            Users = users;
            UserSelector.DataContext = users;
        }

        private void btnLogin_Click_1(object sender, RoutedEventArgs e)
        {
            if (tbPassword.Password != ((User)UserSelector.SelectedItem).Password)
            {
                MessageBox.Show("Incorrect Old Password. Please try again",
                    "Password Info", MessageBoxButton.OK, MessageBoxImage.Information);
                tbPassword.Clear();
            }

            if (LoginEvent != null)
                LoginEvent(this, new LoginEventArgs((User)UserSelector.SelectedItem));
        }


    }

    public class LoginEventArgs : EventArgs
    {
        public User User { get; set; }

        public LoginEventArgs(User u)
        {
            User = u;

        }
    }
}
