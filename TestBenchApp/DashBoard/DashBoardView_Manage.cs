﻿
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using System.Runtime.Serialization;
using System.Xml;

using System.Xml.Serialization;
using TestBenchApp.Entity;
using TestBenchApp.Line;

namespace TestBenchApp.DashBoard
{
    public partial class DashBoardView : UserControl
    {
       
        public String CurrentUser { get; set; }
        Users Users;

        public UIElement TansientElement { get; set; }

        

        #region MANAGE_PASSWORD
        private void Password_Click_1(object sender, RoutedEventArgs e)
        {
            PasswordControl pwdControl = new PasswordControl(Users);
            pwdControl.PasswordChangeEvent += pwdControl_PasswordChangeEvent;
            pwdControl.PasswordCancelEvent += pwdControl_PasswordCancelEvent;

            Transient.Children.Clear();
            Transient.Children.Add(pwdControl);
            Keyboard.Focus(pwdControl);
        }

        void pwdControl_PasswordCancelEvent(object sender, PasswordCancelEventArgs e)
        {
            Transient.Children.Clear();
        }

        void pwdControl_PasswordChangeEvent(object sender, PasswordChangeEventArgs e)
        {
            dataAccess.ChangePassword(e.User);
            Users.ChangePassword(e.User);

          
           
            MessageBox.Show("Password Changed", "Application Info", MessageBoxButton.OK, MessageBoxImage.Information);
            Transient.Children.Clear();
        }

#endregion

        #region MANAGE_PLAN
        private void SetPlan_Click(object sender, RoutedEventArgs e)
        {
            PlanView pv = new PlanView();
            

        }
        #endregion





        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {

        }



      





      
    }
}