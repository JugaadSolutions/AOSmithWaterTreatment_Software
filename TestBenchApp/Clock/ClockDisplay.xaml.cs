using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestBenchApp.Clock
{
    /// <summary>
    /// Interaction logic for ClockDisplay.xaml
    /// </summary>
    public partial class ClockDisplay : UserControl
    {
        ClockVM clock;
        public ClockDisplay()
        {
            InitializeComponent();



            clock = new ClockVM();
            this.DataContext = clock;

        }

        public ClockDisplay(String ts)
        {
            InitializeComponent();



            clock = new ClockVM(ts);
            this.DataContext = clock;

        }

        public ClockDisplay(ClockVM clock)
        {
            this.clock = new ClockVM(clock.getDateTime());
            this.DataContext = clock;
        }

       

    }

    public class ClockVM : INotifyPropertyChanged
    {

        public Timer clockUpdateTimer;
        DateTime dateTime;
        String time;
        public String Time
        {
            get { return dateTime.ToString("HH:mm:ss"); }
            set
            {
                time = value;
                OnPropertyChanged("Time");
            }
        }

        String date;
        public String Date
        {
            get { return dateTime.ToString("dd-MM-yyyy"); }
            set
            {
                date = value;
                OnPropertyChanged("Date");
            }
        }


        public DateTime getDateTime()
        {
            return dateTime;
        }


        public ClockVM()
        {
            dateTime = DateTime.Now;
            clockUpdateTimer = new Timer(1000);
            clockUpdateTimer.AutoReset = false;
            clockUpdateTimer.Start();
            clockUpdateTimer.Elapsed += clockUpdateTimer_Elapsed;

            DateTime.Now.TimeOfDay.ToString();
        }
        public ClockVM(String ts)
        {
            String day = ts.Substring(0, 2);
            String month = ts.Substring(2, 2);
            String year = ts.Substring(4, 2);
            String hour = ts.Substring(6, 2);
            String minute = ts.Substring(8, 2);
            dateTime = new DateTime(DateTime.Now.Year, Convert.ToInt32(month), Convert.ToInt32(day), Convert.ToInt32(hour),
                Convert.ToInt32(minute), 0);
            clockUpdateTimer = new Timer(1000);
            clockUpdateTimer.AutoReset = false;
            clockUpdateTimer.Start();
            clockUpdateTimer.Elapsed += clockUpdateTimer_Elapsed;
        }
        public ClockVM(String day, String month, String hour, String minute, String second)
        {
            dateTime = new DateTime(DateTime.Now.Year, Convert.ToInt32(month), Convert.ToInt32(day), Convert.ToInt32(hour),
                Convert.ToInt32(minute), Convert.ToInt32(second));
            clockUpdateTimer = new Timer(1000);
            clockUpdateTimer.AutoReset = false;
            clockUpdateTimer.Start();
            clockUpdateTimer.Elapsed += clockUpdateTimer_Elapsed;
        }

        public ClockVM(DateTime dateTime)
        {
            this.dateTime = new DateTime(DateTime.Now.Year, Convert.ToInt32(dateTime.Month),
                Convert.ToInt32(dateTime.Day), Convert.ToInt32(dateTime.Hour),
               Convert.ToInt32(dateTime.Minute), Convert.ToInt32(dateTime.Second));
            clockUpdateTimer = new Timer(1000);
            clockUpdateTimer.AutoReset = false;
            clockUpdateTimer.Start();
            clockUpdateTimer.Elapsed += clockUpdateTimer_Elapsed;
        }


        void clockUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

            dateTime = DateTime.Now;
            Date = dateTime.ToString("dd-MM-yyyy");
            Time = dateTime.ToString("HH:mm:ss");
            clockUpdateTimer.Start();
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
