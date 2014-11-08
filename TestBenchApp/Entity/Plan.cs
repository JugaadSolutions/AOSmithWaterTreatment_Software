using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBenchApp.Entity
{
    public class Plan : INotifyPropertyChanged
    {

        public String ModelName { get; set; }
        public String ModelNumber { get; set; }

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
        
        public int BSerialNo { get; set; }
        public int  FSerialNo { get; set; }
        public int CombinationSerialNo { get; set; }


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
