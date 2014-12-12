using shared;
using shared.Entity;
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

namespace TestBenchApp.UIControls
{
    /// <summary>
    /// Interaction logic for PlanView.xaml
    /// </summary>
    public partial class PlanView : UserControl
    {
        PlanViewModel PVM;
        Model.Type UnitType;
      
        public event EventHandler<SetPlanArgs> SetPlanEvent;
        public event EventHandler<SetPlanArgs> ModifyPlanEvent;
       


        public PlanView(PlanViewModel pvm, Model.Type unitType)
        {
            InitializeComponent();

            PVM = pvm;

            this.DataContext = PVM;

            SetPlan();
           

            UnitType = unitType;
        }

        private void SetButton_Click(object sender, RoutedEventArgs e)
        {

            if (updatePlan() == false) return;
           


            if (SetPlanEvent != null)
                SetPlanEvent(this, new SetPlanArgs(PVM));
        }

        public void SetPlan()
        {
            if (PVM.Plan.ModelCode!= null && PVM.Plan.ModelCode != String.Empty)
            {
                for (int i = 0; i < PVM.Models.Count; i++)
                {
                    if (PVM.Plan.ModelCode == PVM.Models[i].Code)
                    {
                        //ModelTag.Children.Clear();
                        //TextBox TagTextBox = new TextBox();
                        //TagTextBox.Text = PVM.Models[i].Name;
                        //TagTextBox.FontSize = 20;
                        //TagTextBox.Foreground = Brushes.Black;
                        //TagTextBox.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                        //TagTextBox.IsReadOnly = true;
                        //TagTextBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        //TagTextBox.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        //TagTextBox.MinWidth = 250;

                        //ModelTag.Children.Add(TagTextBox);

                        ModelSelector.Visibility = System.Windows.Visibility.Collapsed;
                        TagTextBox.Text = PVM.Models[i].Name;
                        TagTextBox.Visibility = System.Windows.Visibility.Visible;

                        //ModelSelector.SelectedIndex = i;
                        //ModelSelector.IsEnabled = false;
                    }
                }
                SetButton.IsEnabled = false;
                ModifyButton.IsEnabled = true;
            }

            else
            {
                SetButton.IsEnabled = true;
                ModifyButton.IsEnabled = false;
            }
        }

        private bool updatePlan()
        {
            if (ModelSelector.SelectedIndex == -1)
                return false;
            int Quantity;
            bool parseResult = int.TryParse(QuantityTextBox.Text, out Quantity);
            if ( parseResult == false || Quantity == 0)
            {
                MessageBox.Show(" Invalid Quantity ", "Application Info", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            PVM.Plan.ModelCode = PVM.Models[ModelSelector.SelectedIndex].Code;
            PVM.Plan.ModelName = PVM.Models[ModelSelector.SelectedIndex].Name;
            PVM.Plan.Quantity = Quantity;
            PVM.Plan.UnitType = UnitType;

            return true;
        }

        private void ModifyButton_Click(object sender, RoutedEventArgs e)
        {
            int Quantity;
            if (int.TryParse(QuantityTextBox.Text, out Quantity) == false)
            {
                MessageBox.Show(" Invalid Quantity ", "Application Info", MessageBoxButton.OK, MessageBoxImage.Error);
                return ;
            }
            PVM.Plan.Quantity = Quantity;
            if (ModifyPlanEvent != null)
                ModifyPlanEvent(this, new SetPlanArgs(PVM));
        }
    }


    public class SetPlanArgs : EventArgs
    {
        public PlanViewModel PVM { get; set; }

        public SetPlanArgs(PlanViewModel pvm)
        {
            PVM = pvm;
        }
    }
}
