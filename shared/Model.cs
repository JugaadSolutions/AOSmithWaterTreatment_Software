using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.Entity
{
    public enum MODEL_TYPE { NONE = 0, ACTUAL = 1, DUMMY_ELEC, DUMMY_MECH };
    public class Model : INotifyPropertyChanged
    {
        public enum Type { NONE = 0, BODY = 1, FRAME = 2, COMBINED };

        public int SlNo {get;set;}
        public String Product {get;set;}
        public String ProductNumber {get;set;}
        public double StorageCapacity {get;set;}
        public double NetQuantity {get;set;}
        public String Name { get; set; }
        public String Code { get; set; }
        public int MRP { get; set; }
        public String CustomerCare { get; set; }
        public String Email { get; set; }
        public String EAN { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }
        public MODEL_TYPE ModelType { get; set; }

        public Model()
        {
            SlNo = -1;
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
