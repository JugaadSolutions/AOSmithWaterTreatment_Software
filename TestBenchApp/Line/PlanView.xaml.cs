using shared;
using shared.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace TestBenchApp.Line
{
    /// <summary>
    /// Interaction logic for Line.xaml
    /// </summary>
    public partial class PlanView : UserControl
    {
        public event EventHandler<EventArgs> SaveEvent;
        public event EventHandler<EventArgs> btnDoneClicked;

        DataAccess da;
        ObservableCollection<Model> Models1,Models2,Models3,Models4;
        List<Plan> plans;

        public PlanView()
        {
            InitializeComponent();
            da = new DataAccess();
            Models1 = da.GetModels();
            Models2 = da.GetModels();
            Models3 = da.GetModels();
            Models4 = da.GetModels();


            plans = da.GetPlans();

            if (plans.Count > 0)
            {
                tbSetPq1.Text = plans[0].Quantity.ToString();
                ModelSelector1.DataContext = Models1;
                for (int i = 0; i < Models1.Count; i++)
                {
                    if (plans[0].ModelCode == Models1[i].Code)
                    {
                        ModelSelector1.SelectedIndex = i;
                        ModelSelector1.IsEnabled = false;
                    }
                }

            }
            else
            {
                ModelSelector1.DataContext = Models1;
                ModelSelector2.DataContext = Models2;
                ModelSelector3.DataContext = Models3;
                ModelSelector4.DataContext = Models4;
            }

            if (plans.Count > 1)
            {
                tbSetPq2.Text = plans[1].Quantity.ToString();
                ModelSelector2.DataContext = Models2;
                for (int i = 0; i < Models2.Count; i++)
                {
                    if (plans[1].ModelCode == Models2[i].Code)
                    {
                        ModelSelector2.SelectedIndex = i;
                        ModelSelector2.IsEnabled = false;
                    }
                }
                
            }
            else
            {
                
                ModelSelector2.DataContext = Models2;
                ModelSelector3.DataContext = Models3;
                ModelSelector4.DataContext = Models4;
            }


            if (plans.Count > 2)
            {
                tbSetPq3.Text = plans[2].Quantity.ToString();
                ModelSelector3.DataContext = Models3;
                for (int i = 0; i < Models3.Count; i++)
                {
                    if (plans[2].ModelCode == Models3[i].Code)
                    {
                        ModelSelector3.SelectedIndex = i;
                        ModelSelector3.IsEnabled = false;
                    }
                }

            }
            else
            {

                
                ModelSelector3.DataContext = Models3;
                ModelSelector4.DataContext = Models4;
            }


            if (plans.Count > 3)
            {
                tbSetPq4.Text = plans[3].Quantity.ToString();
                ModelSelector4.DataContext = Models4;
                for (int i = 0; i < Models2.Count; i++)
                {
                    if (plans[3].ModelCode == Models4[i].Code)
                    {
                        ModelSelector4.SelectedIndex = i;
                        ModelSelector4.IsEnabled = false;
                    }
                }

            }
            else
            {

                
                ModelSelector3.DataContext = Models3;
                ModelSelector4.DataContext = Models4;
            }
            



            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
            int quantity1 = 0,quantity2 = 0,quantity3 = 0,quantity4 = 0;
            List<Plan> newplans = new List<Plan>();
            bool planFlag = false;

            if (tbSetPq1.Text != String.Empty)
            {
                if (!int.TryParse(tbSetPq1.Text, out quantity1))
                {
                    MessageBox.Show("Incorrect Plan. Please Verify", "Plan Incorrect", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

            }
            if (tbSetPq2.Text != String.Empty)
            {
                if (!int.TryParse(tbSetPq2.Text, out quantity2))
                {
                    MessageBox.Show("Incorrect Plan. Please Verify", "Plan Incorrect", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
            if (tbSetPq3.Text != String.Empty)
            {
                if (!int.TryParse(tbSetPq3.Text, out quantity3))
                {
                    MessageBox.Show("Incorrect Plan. Please Verify", "Plan Incorrect", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
            if (tbSetPq4.Text != String.Empty)
            {
                if (!int.TryParse(tbSetPq4.Text, out quantity4))
                {
                    MessageBox.Show("Incorrect Plan. Please Verify", "Plan Incorrect", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }

            if (tbSetPq1.Text != String.Empty)
            {
                try
                {
                    newplans.Add(new Plan
                    {
                        ModelCode = Models1[ModelSelector1.SelectedIndex].Code,
                        Quantity = quantity1,
                        BSerialNo = 0,
                        FSerialNo = 0,
                        CombinationSerialNo = 0,
                        Actual = 0,
                        BStatus = false,
                        FStatus = false,
                        Timestamp = DateTime.Now
                    });

                    planFlag = true;
                }
                catch(Exception s)
                {
                    MessageBox.Show("Incorrect Plan. Please Verify", "Plan Incorrect", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

            }

            if (tbSetPq2.Text != String.Empty)
            {
                try
                {
                    newplans.Add(new Plan
                    {
                        ModelCode = Models2[ModelSelector2.SelectedIndex].Code,
                        Quantity = quantity2,
                        BSerialNo = 0,
                        FSerialNo = 0,
                        CombinationSerialNo = 0,
                        Actual = 0,
                        BStatus = false,
                        FStatus = false,
                        Timestamp = DateTime.Now
                    });

                    planFlag = true;
                }
                catch (Exception s)
                {
                    MessageBox.Show("Incorrect Plan. Please Verify", "Plan Incorrect", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }

            if (tbSetPq3.Text != String.Empty)
            {
                try
                {
                    newplans.Add(new Plan
                    {
                        ModelCode = Models3[ModelSelector3.SelectedIndex].Code,
                        Quantity = quantity3,
                        BSerialNo = 0,
                        FSerialNo = 0,
                        CombinationSerialNo = 0,
                        Actual = 0,
                        FStatus = false,
                        BStatus = false,
                        Timestamp = DateTime.Now
                    });

                    planFlag = true;
                }
                catch (Exception s)
                {
                    MessageBox.Show("Incorrect Plan. Please Verify", "Plan Incorrect", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }

            if (tbSetPq4.Text != String.Empty)
            {
                try
                {
                    newplans.Add(new Plan
                    {
                        ModelCode = Models4[ModelSelector4.SelectedIndex].Code,
                        Quantity = quantity4,
                        BSerialNo = 0,
                        FSerialNo = 0,
                        CombinationSerialNo = 0,
                        Actual = 0,
                        BStatus = false,
                        FStatus = false,
                        Timestamp = DateTime.Now
                    });

                    planFlag = true;
                }
                catch (Exception s)
                {
                    MessageBox.Show("Incorrect Plan. Please Verify", "Plan Incorrect", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }


            foreach (Plan p in newplans)
            {

                da.InsertPlan(p);
            }


            if (planFlag == true)
            {
                MessageBox.Show("Plan has been set successfully", " ", MessageBoxButton.OK);
                planFlag = false;
            }


          

           
            
                
            
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            if (btnDoneClicked != null)
                btnDoneClicked(this, new EventArgs());
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnDelete4_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Delete Plan?", "Delete Plan", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if ((plans[3].FStatus == true) || (plans[3].BStatus == true))
                {
                    MessageBox.Show("Plan is currently Active", "Application Info",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (plans[3].Actual >= plans[3].Quantity)
                {
                    MessageBox.Show("Plan has been completed", "Application Info",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                da.DeletePlan(plans[0]);
            }
        }

        private void btnModify4_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Apply Changes", "Plan Modifcation", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if (result == MessageBoxResult.Yes)
            {
                plans[3].Quantity = Convert.ToInt32(tbSetPq4.Text);
                da.UpdatePlanQuantity(plans[3]);
            }
            else
                tbSetPq4.Text = Convert.ToString(plans[3].Quantity);
        }

        private void btnDelete3_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Delete Plan?", "Delete Plan", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if ((plans[2].FStatus == true) || (plans[2].BStatus == true))
                {
                    MessageBox.Show("Plan is currently Active", "Application Info",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (plans[2].Actual >= plans[2].Quantity)
                {
                    MessageBox.Show("Plan has been completed", "Application Info",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                da.DeletePlan(plans[2]);
            }
        }

        private void btnModify3_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Apply Changes", "Plan Modifcation", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if (result == MessageBoxResult.Yes)
            {
                plans[2].Quantity = Convert.ToInt32(tbSetPq3.Text);
                da.UpdatePlanQuantity(plans[2]);
            }
            else
                tbSetPq3.Text = Convert.ToString(plans[2].Quantity);
        }

        private void btnDelete2_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Delete Plan?", "Delete Plan", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if ((plans[1].FStatus == true) || (plans[1].BStatus == true))
                {
                    MessageBox.Show("Plan is currently Active", "Application Info",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (plans[1].Actual >= plans[1].Quantity)
                {
                    MessageBox.Show("Plan has been completed", "Application Info",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                da.DeletePlan(plans[0]);
            }
        }

        private void btnModify2_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Apply Changes", "Plan Modifcation", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if (result == MessageBoxResult.Yes)
            {
                plans[1].Quantity = Convert.ToInt32(tbSetPq2.Text);
                da.UpdatePlanQuantity(plans[1]);
            }
            else
                tbSetPq2.Text = Convert.ToString(plans[1].Quantity);
        }

        private void btnDelete1_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result =  MessageBox.Show("Delete Plan?", "Delete Plan", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if ((plans[0].FStatus == true) || (plans[0].BStatus == true))
                {
                    MessageBox.Show("Plan is currently Active", "Application Info", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (plans[0].Actual >= plans[0].Quantity)
                {
                    MessageBox.Show("Plan has been completed", "Application Info",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                da.DeletePlan(plans[0]);
            }
        }

        private void btnModify1_Click_1(object sender, RoutedEventArgs e)
        {
    
            MessageBoxResult result =  MessageBox.Show("Apply Changes", "Plan Modifcation", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if (result == MessageBoxResult.Yes)
            {
                plans[0].Quantity = Convert.ToInt32(tbSetPq1.Text);
                da.UpdatePlanQuantity(plans[0]);
            }
            else
                tbSetPq1.Text = Convert.ToString(plans[0].Quantity);

        }

        
     

    }
}
