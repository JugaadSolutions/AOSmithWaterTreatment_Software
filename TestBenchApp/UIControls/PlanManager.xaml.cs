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

namespace TestBenchApp.UIControls
{
    /// <summary>
    /// Interaction logic for PlanManager.xaml
    /// </summary>
    public partial class PlanManager : UserControl
    {
       
        List<Plan> FramePlans;
        List<Plan> BodyPlans;

        

        ObservableCollection<Model> UsedFrameModels;
        ObservableCollection<Model> AvailableFrameModels;

        ObservableCollection<Model> UsedBodyModels;
        ObservableCollection<Model> AvailableBodyModels;

        DataAccess da;

        ObservableCollection<PlanViewModel> FrameViewModels;
        ObservableCollection<PlanViewModel> BodyViewModels;

        public PlanManager()
        {
            InitializeComponent();

            da = new DataAccess();

            AvailableFrameModels = da.GetModels();
            AvailableBodyModels = da.GetModels();

            FramePlans = da.GetPlans(Model.Type.FRAME);
            BodyPlans = da.GetPlans(Model.Type.BODY);

            FrameViewModels = new ObservableCollection<PlanViewModel>();
            BodyViewModels = new ObservableCollection<PlanViewModel>();

            UsedFrameModels = new ObservableCollection<Model>();
            UsedBodyModels = new ObservableCollection<Model>();

            updateAvailableModels();

            foreach (Plan p in FramePlans)
            {
                PlanViewModel pvm = new PlanViewModel(p, UsedFrameModels);

                addPlanViews(pvm);
            }
            
            foreach (Plan p in BodyPlans)
            {
                PlanViewModel pvm = new PlanViewModel(p, UsedBodyModels);
                addPlanViews(pvm);
            }

           


        }

      

        void pv_SetPlanEvent(object sender, SetPlanArgs e)
        {
            da.InsertPlan(e.PVM.Plan);

            PlanView pv = sender as PlanView;
            pv.SetPlan();
            pv.ModifyPlanEvent += pv_ModifyPlanEvent;

            if (e.PVM.Plan.UnitType == Model.Type.FRAME)
            {
                FramePlans = da.GetPlans(Model.Type.FRAME);
                FramePlanAddButton.IsEnabled = true;
            }
            else
            {
                BodyPlans = da.GetPlans(Model.Type.BODY);
                BodyPlanAddButton.IsEnabled = true;
            }

            MessageBox.Show(" Plan Set ", "Application Info", MessageBoxButton.OK, MessageBoxImage.Information);

            updateAvailableModels();
            return;
        }

        void pv_ModifyPlanEvent(object sender, SetPlanArgs e)
        {
            da.ModifyPlan(e.PVM.Plan);

            MessageBox.Show(" Plan Modified ", "Application Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;

        }


        void updateAvailableModels()
        {
            foreach (Plan p in FramePlans)
            {
                foreach (Model m in AvailableFrameModels)
                    if (p.ModelCode == m.Code)
                    {

                        UsedFrameModels.Add(m);
                    }


            }

            foreach (Model m in UsedFrameModels)
                AvailableFrameModels.Remove(m);




            foreach (Plan p in BodyPlans)
            {
                foreach (Model m in AvailableBodyModels)
                    if (p.ModelCode == m.Code)
                    {

                        UsedBodyModels.Add(m);
                    }

            }
            foreach (Model m in UsedBodyModels)
                AvailableBodyModels.Remove(m);
        }




        private void FramePlanAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (AvailableFrameModels.Count <= 0)
            {
                MessageBox.Show("Application Info", "No Models Available", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            PlanViewModel pvm = new PlanViewModel(AvailableFrameModels,Model.Type.FRAME);


            addPlanViews(pvm);


            FramePlanAddButton.IsEnabled = false;
        }


        void addPlanViews(PlanViewModel pvm)
        {
            if (pvm.Plan.UnitType == Model.Type.FRAME)
            {
                FramePlanPanel.RowDefinitions.Add(new RowDefinition());
                PlanView pv = new PlanView(pvm, Model.Type.FRAME);
                pv.SetValue(Grid.RowProperty, FramePlanPanel.RowDefinitions.Count - 1);
                pv.SetPlanEvent += pv_SetPlanEvent;
                pv.ModifyPlanEvent += pv_ModifyPlanEvent;
                FramePlanPanel.Children.Add(pv);
            }

            if (pvm.Plan.UnitType == Model.Type.BODY)
            {
                BodyPlanPanel.RowDefinitions.Add(new RowDefinition());
                PlanView pv = new PlanView(pvm, Model.Type.BODY);
                pv.SetValue(Grid.RowProperty, BodyPlanPanel.RowDefinitions.Count - 1);
                pv.SetPlanEvent += pv_SetPlanEvent;
                pv.ModifyPlanEvent += pv_ModifyPlanEvent;
                BodyPlanPanel.Children.Add(pv);
            }
        }

        private void BodyPlanAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (AvailableBodyModels.Count <= 0)
            {
                MessageBox.Show("No Models Available", "Application Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            PlanViewModel pvm = new PlanViewModel(AvailableBodyModels,Model.Type.BODY);


            addPlanViews(pvm);


            BodyPlanAddButton.IsEnabled = false;
        }
      
    }

       
}
