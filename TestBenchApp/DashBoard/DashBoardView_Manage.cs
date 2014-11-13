
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
        public bool cbM1Checked = false;
        public bool cbF1Checked = false;
        public bool cbCSChecked = false;

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
            PlanView p = new PlanView();
            p.btnDoneClicked += p_btnDoneClicked; 
            Transient.Children.Clear();
            Transient.Children.Add(p);
            Transient.Visibility = System.Windows.Visibility.Visible;
        }

        private void p_btnDoneClicked(object sender, EventArgs e)
        {
            Transient.Children.Clear();
        }
        #endregion

        #region MANAGE_MODELS
        private void Models_Click_1(object sender, RoutedEventArgs e)
        {
            ModelsManager c = new ModelsManager();
            c.CancelClicked += c_btnCancelClicked;
            c.TestPrintBtnClicked += c_TestPrintBtnClicked;
            Transient.Children.Clear();
            Transient.Children.Add(c);
            Transient.Visibility = System.Windows.Visibility.Visible;
        }

        void c_TestPrintBtnClicked(object sender, TestEventArgs e)
        {
            String slNo = null;

            if (e.m.Name.Contains("Puritee"))
                slNo = "B163" + DateTime.Now.ToString("yyMMdd") + "0001";

            combPrinterManager.combStickerTestPrint(e.m.Product, e.m.ProductNumber, 
                e.m.MRP.ToString(), e.m.Name, Convert.ToString(e.m.StorageCapacity), Convert.ToString(e.m.NetQuantity),
                slNo);
           
        }

      
        private void c_btnCancelClicked(object sender, EventArgs e)
        {
            Transient.Children.Clear();
        }

        #endregion

        #region MANAGE_REPRINT
        private void Reprint_Click_1(object sender, RoutedEventArgs e)
        {
            ReprintManager r = new ReprintManager();
            r.btnDoneClicked += r_btnDoneClicked;
            r.cbM1Clicked += r_cbM1Clicked;
            r.cbF1Clicked += r_cbF1Clicked;
            r.cbCSClicked += r_cbCSClicked;

            r.cbF1unChecked += r_cbF1unChecked;
            r.cbCSunChecked += r_cbCSunChecked;
            r.cbM1unChecked += r_cbM1unChecked;

            Transient.Children.Clear();
            Transient.Children.Add(r);
            Transient.Visibility = System.Windows.Visibility.Visible;

        }

        void r_cbM1unChecked(object sender, EventArgs e)
        {
            cbM1Checked = false;
        }

        void r_cbCSunChecked(object sender, EventArgs e)
        {
            cbCSChecked = false;
        }

        void r_cbF1unChecked(object sender, EventArgs e)
        {
            cbF1Checked = false; 
        }

        void r_cbCSClicked(object sender, EventArgs e)
        {
            cbCSChecked = true;

            cbF1Checked = false;
            cbM1Checked = false;
        }

        void r_cbF1Clicked(object sender, EventArgs e)
        {
            cbF1Checked = true;

            cbCSChecked = false;
            cbM1Checked = false;
        }

        void r_cbM1Clicked(object sender, EventArgs e)
        {
            cbM1Checked = true;

            cbCSChecked = false;
            cbF1Checked = false;

        }

        private void r_btnDoneClicked(object sender, EventArgs e)
        {
            Transient.Children.Clear();
            cbCSChecked = false;
            cbF1Checked = false;
            cbM1Checked = false;
        }
        #endregion

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            
        }









      
    }
}
