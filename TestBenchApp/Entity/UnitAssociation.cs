using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TestBenchApp.Entity
{
    public class UnitAssociation
    {
        public event EventHandler<EventArgs> CompleteEvent;
        public event EventHandler<EventArgs> TimeoutEvent;

        public String Model { get; set; }

        String bCode;
        public String BCode {
            get { return bCode; }

            set
            {
                bCode = value;
                if (FCode == String.Empty)
                {
                    Association.Start();
                }
                else
                {
                    if (CompleteEvent != null)
                        CompleteEvent(this, new EventArgs());
                }
            }

        }
        String fCode;
        public String FCode
        {
            get { return fCode; }

            set
            {
                fCode = value;
                if (BCode == String.Empty)
                {
                    Association.Start();
                }
                else
                {
                    if (CompleteEvent != null)
                        CompleteEvent(this, new EventArgs());
                }
            }

        }

        Timer Association { get; set; }

        public UnitAssociation(int timeout)
        {
            Association = new Timer(timeout);
            Association.AutoReset = false;
            Association.Elapsed +=Association_Elapsed;
            BCode = String.Empty;
            FCode = String.Empty;
        }

        void Association_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (TimeoutEvent != null)
                TimeoutEvent(this, new EventArgs());
        }
    }


}
