using GEMSPL.Entity;
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
    /// Interaction logic for UsersControl.xaml
    /// </summary>
    public partial class UsersControl : UserControl
    {
        public event EventHandler<AddUserEventArgs> AddUserEvent;
        public event EventHandler<DeleteUserEventArgs> DeleteUserEvent;
        public event EventHandler<CancelEventArgs> CancelEvent;
        Users Users;

        public UsersControl(Users users)
        {
            InitializeComponent();
            Users = users;
            UserSelector.DataContext = Users.UserDictionary.Keys;
            UserSelector.SelectedIndex = 0;

        }

        private void AddUser_Click_1(object sender, RoutedEventArgs e)
        {
            if (UserName.Text == string.Empty || Password.Password== String.Empty)
            {
                MessageBox.Show("Invalid Data. Please try again",
                    "Password Info", MessageBoxButton.OK, MessageBoxImage.Information);
                UserName.Clear();
                Password.Clear();
                UserName.Focus();
                return;
            }
            if (AddUserEvent != null)
                AddUserEvent(this, new AddUserEventArgs(UserName.Text, Password.Password));
        }

        private void DeleteUser_Click_1(object sender, RoutedEventArgs e)
        {
            String name = (String)UserSelector.SelectedItem;
            if (name == "admin")
            {
                MessageBox.Show("Admin User cannot be deleted",
                   "Manage Users Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (DeleteUserEvent != null)
                DeleteUserEvent(this, new DeleteUserEventArgs(name));
        }


        private void Cancel_Click_1(object sender, RoutedEventArgs e)
        {
            if (CancelEvent != null)
                CancelEvent(this, new CancelEventArgs());
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            UserName.Focus();
        }
    }

    public class AddUserEventArgs : EventArgs
    {
        public String Name { get; set; }
        public String Password { get; set; }

        public AddUserEventArgs(String name,String _password)
        {
            Name = name;
            Password = _password;
        }
    }

    public class DeleteUserEventArgs : EventArgs
    {
        public String Name { get; set; }
        public DeleteUserEventArgs(String name)
        {
            Name = name;
            
        }
    }

    public class CancelEventArgs : EventArgs
    {

    }
}
