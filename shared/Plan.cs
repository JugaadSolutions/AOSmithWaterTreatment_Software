using shared.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared
{
    public class Plan : INotifyPropertyChanged
    {

        public String ModelName { get; set; }
        public String ModelCode { get; set; }

        public int Quantity { get; set; }

        public int slNumber { get; set; }

        int actual;
        public int Actual {
            get
            {
                return actual;
            }

            set
            {
                actual = value;
                OnPropertyChanged("Actual");
            }
        }

        public DateTime Timestamp { get; set; }

        bool bstatus;
        public bool BStatus
        {
            get
            {
                return bstatus;
            }
            set
            {
                bstatus = value;
                OnPropertyChanged("BStatus");
            }
        }


        bool fstatus;
        public bool FStatus
        {
            get
            {
                return fstatus;
            }
            set
            {
                fstatus = value;
                OnPropertyChanged("FStatus");
            }
        }
        

        public int bSerialNo;
        public int BSerialNo
        {
            get
            {
                return bSerialNo;
            }
            set
            {
                bSerialNo = value;
                OnPropertyChanged("BSerialNo");
            }
        }

        int fSerialNo;
        public int FSerialNo
        {
            get
            {
                return fSerialNo;
            }
            set
            {
                fSerialNo = value;
                OnPropertyChanged("FSerialNo");
            }
        }


  
        int combinationSerialNo;
        public int CombinationSerialNo
        {
            get
            {
                return combinationSerialNo;
            }
            set
            {
                combinationSerialNo = value;
                OnPropertyChanged("CombinationSerialNo");
            }
        }

        Model.Type unitType;
        public Model.Type UnitType
        {
            get
            {
                return (Model.Type)unitType;
            }
            set
            {
                unitType = (Model.Type)value;
                OnPropertyChanged("UnitType");
            }
        }


        #region INotifyPropetyChangedHandler
        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }


    public class PlanViewModel : INotifyPropertyChanged
    {
        public Plan Plan {get;set;}

         ObservableCollection<Model> models;
        public ObservableCollection<Model> Models
        {
            get
        { 
            return models; 
        }
            set
            {
                models = value;
                OnPropertyChanged("Models");
            }
        }
        public Boolean IsSetEnabled { get; set; }
        public Boolean IsModifyEnabled { get; set; }
        public Boolean IsDeleteEnabled { get; set; }
        public int ModelSelectedIndex { get; set; }
        public Boolean IsSelectionEnabled { get; set; }

        public PlanViewModel(ObservableCollection<Model> availableModels, Model.Type unitType)
        {
            Plan = new Plan();

            Models = availableModels;

            IsSetEnabled = true;
            IsModifyEnabled = false;
            IsDeleteEnabled = false;

            ModelSelectedIndex = -1;
            IsSelectionEnabled = true;
            Plan.UnitType = unitType;
        }

        public PlanViewModel(Plan P,ObservableCollection<Model> usedModels )
        {
            Plan = P;
            Models = usedModels;
            for(int i = 0 ;i < Models.Count ;i++)
            {
                if( Models[i].Code == Plan.ModelCode )
                {
                    ModelSelectedIndex = i;
                    IsSelectionEnabled = false;

                    IsSetEnabled = false;
                    IsModifyEnabled = true;
                    IsDeleteEnabled = true;
                    break;
                }
            }
        }

        #region INotifyPropetyChangedHandler
        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }


}
