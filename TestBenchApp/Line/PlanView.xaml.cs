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
        List<Model> Models;
        List<Plan> plans;

        public PlanView()
        {
            InitializeComponent();
            da = new DataAccess();
            Models = da.GetModels();

            plans = da.GetPlans();

            ModelSelector1.DataContext = Models;
            ModelSelector2.DataContext = Models;
            ModelSelector3.DataContext = Models;
            ModelSelector4.DataContext = Models;



            
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
                        ModelNumber = Models[ModelSelector1.SelectedIndex].Number,
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
                        ModelNumber = Models[ModelSelector2.SelectedIndex].Number,
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
                        ModelNumber = Models[ModelSelector3.SelectedIndex].Number,
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
                        ModelNumber = Models[ModelSelector4.SelectedIndex].Number,
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
            MessageBoxResult result =  MessageBox.Show("Apply Changes", "Delete Plan", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
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
