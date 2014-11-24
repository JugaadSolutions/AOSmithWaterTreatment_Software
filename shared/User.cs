using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace shared
{
    public class User
    {
        public String Name { get; set; }
        public String Password { get; set; }

        public User(String name, String password)
        {
            Name = name;
            Password = password;
        }
    }

    
    public class Users : ObservableCollection<User>
    {
        // need a parameterless constructor for serialization
        
        public  void ChangePassword(User user)
        {
            IEnumerator<User> enumerator = this.GetEnumerator();
            while (enumerator.MoveNext())
            {

                if (enumerator.Current.Name == user.Name)
                {
                    enumerator.Current.Password = user.Password;
                    break;
                }
            }
                
        }
    }

}
