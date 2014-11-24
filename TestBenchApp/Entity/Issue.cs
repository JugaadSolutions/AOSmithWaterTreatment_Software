using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TestBenchApp.Entity
{
    

     public enum ISSUE_STATE { NONE = 0, RAISED = 1, CRITICAL = 2, RESOLVED = 3 };



        public class Issue : INotifyPropertyChanged
        {

            private List<double> timeout;
            private int criticalLevel = 0;
            private ISSUE_STATE state = ISSUE_STATE.NONE;

            public event EventHandler<issueEscalateEventArgs> issueEscalationEvent;

            public ISSUE_STATE State
            {
                get { return state; }
                set
                {
                    state = value;
                    OnPropertyChanged("State");
                }
            }

            int line = 0;
            public int Line
            {
                get { return line; }
                set { line = value; }
            }


            int device = 0;
            public int Device
            {
                get { return device; }
                set { device = value; }
            }


            int station = 0;
            public int Station
            {
                get { return station; }
                set { station = value; }
            }

            private int department = 0;
            public int Department
            {
                get { return department; }
                set
                {
                    department = value;
                    OnPropertyChanged("Department");
                }
            }

            string message = String.Empty;
            public String Message
            {
                get { return message; }
                set { message = value; }
            }



            Timer timer = null;

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


            public Issue(int line, int station, int department, string message, List<double> timeout)
            {

                Line = line;
                Station = station;
                Department = department;
                Message = message;



                this.State = ISSUE_STATE.RAISED;
                this.timeout = timeout;

                timer = new Timer(timeout[criticalLevel] * 60 * 1000);
                timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                timer.Start();

                if (issueEscalationEvent != null)
                    issueEscalationEvent(this, new issueEscalateEventArgs(criticalLevel));
            }


            public Issue(int device, int line, int station, int department, string message, List<double> timeout)
            {
                Device = device;
                Line = line;
                Station = station;
                Department = department;
                Message = message;



                this.State = ISSUE_STATE.RAISED;
                this.timeout = timeout;

                timer = new Timer(timeout[criticalLevel] * 60 * 1000);
                timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                timer.Start();

                if (issueEscalationEvent != null)
                    issueEscalationEvent(this, new issueEscalateEventArgs(criticalLevel));
            }

            public void raise()
            {
                if (issueEscalationEvent != null)
                    issueEscalationEvent(this, new issueEscalateEventArgs(criticalLevel));
            }


            public void resolve()
            {
                timer.Stop();
            }

            void timer_Elapsed(object sender, ElapsedEventArgs e)
            {

                timer.Stop();

                State = ISSUE_STATE.CRITICAL;
                this.criticalLevel++;

                if (issueEscalationEvent != null)
                {
                    issueEscalateEventArgs args = new issueEscalateEventArgs(this.criticalLevel);
                    issueEscalationEvent(this, args);
                }

                if (this.criticalLevel < 3)
                {
                    timer.Interval = timeout[criticalLevel] * 60 * 1000; ;
                    timer.Start();
                }





            }





        }



        public class EscalationEventArgs : EventArgs
        {
            public int unitID;
            public int department;
            public string message;
            public int escalationLevel;
            public EscalationEventArgs(int d, string m, int eL)
            {

                department = d;
                message = m;
                escalationLevel = eL;
            }

            public void setUnitID(int unitID)
            {
                this.unitID = unitID;
            }
        }


        public class issueEscalateEventArgs : EventArgs
        {
            public int escalationLevel;


            public issueEscalateEventArgs(int escalationLevel)
            {
                this.escalationLevel = escalationLevel;

            }
        }


        public class IssueCollection : ObservableCollection<Issue>
        {
        }
    

}
