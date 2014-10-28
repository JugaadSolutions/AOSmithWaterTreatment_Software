using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TestBenchApp.Entity
{
    #region DATACLASSES
    public class line : INotifyPropertyChanged
    {
        int id;
        public int ID
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("ID");
            }
        }

        string name = String.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
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



        StationCollection stations;

        public StationCollection Stations
        {
            get { return stations; }
            set
            {
                stations = value;
                OnPropertyChanged("Stations");
            }
        }


        DataAccess dataAccess;

        Dictionary<int, Issue> issue_record_map = null;

        public event EventHandler<EscalationEventArgs> escalationEvent;

        public line(int id, String name)
        {

            this.ID = id;
            this.Name = name;

            dataAccess = new DataAccess();




            issue_record_map = new Dictionary<int, Issue>();


            stations = dataAccess.getStations(id);

        }


    }

    public class lineInfo
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public int ItemIndex { get; set; }

        public lineInfo()
        {
        }
    }

    public class lineCollection : ObservableCollection<line>
    {
        string header = String.Empty;
        public String Header
        {
            get { return header; }
            set
            {
                header = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Header"));
            }
        }

        public Dictionary<int, string> _dictionary = null;

        public lineCollection()
        {
            _dictionary = new Dictionary<int, string>();
        }

        public bool find(lineInfo lineInfo)
        {
            if (_dictionary.ContainsValue(lineInfo.Name)) return true;


            if (_dictionary.ContainsKey(lineInfo.ID)) return true;

            return false;
        }

        public void add(line line)
        {
            try
            {
                _dictionary.Add(line.ID, line.Name);
                Add(line);
            }
            catch (Exception s)
            {
                MessageBox.Show("Unable to Add Line", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public String getLineName(int id)
        {
            return _dictionary[id];
        }

        public String getStationName(int line, int station)
        {
            if (station == 0) return String.Empty;
            IEnumerator<line> enumerator = this.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.ID == line)
                    return enumerator.Current.Stations.getStationName(station);
            }
            return String.Empty;
        }

        public String getClassName(int line, int station, int department, int Class)
        {
            String classDescription = String.Empty;
            IEnumerator<line> enumerator = this.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.ID != line)
                    continue;
                IEnumerator<Station> senumerator = enumerator.Current.Stations.GetEnumerator();
                while (senumerator.MoveNext())
                {
                    if (senumerator.Current.ID == station)
                    {
                        if (department == 1)
                        {
                            classDescription = senumerator.Current.BreakdownClass.getClassName(Class);
                        }
                        else
                        {
                            classDescription = senumerator.Current.QualityClass.getClassName(Class);
                        }
                    }
                }
            }
            return classDescription;
        }









    }

    public partial class Station : INotifyPropertyChanged
    {
        int line;

        int id = 0;
        public int ID
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("ID");
            }
        }

        string name = string.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        ClassCollection breakdownClass;
        public ClassCollection BreakdownClass
        {
            get { return breakdownClass; }
            set
            {
                breakdownClass = value;
                OnPropertyChanged("BreakdownClass");
            }
        }

        ClassCollection qualityClass;

        public ClassCollection QualityClass
        {
            get { return qualityClass; }
            set
            {
                qualityClass = value;
                OnPropertyChanged("QualityClass");
            }
        }


        DataAccess dataAccess;


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


        public Station(int line, int ID, String name)
        {
            this.line = line;
            this.ID = ID;

            this.Name = name;

            dataAccess = new DataAccess();

            breakdownClass = new ClassCollection();
            qualityClass = new ClassCollection();

            breakdownClass = dataAccess.getClass(line, ID, 1);
            qualityClass = dataAccess.getClass(line, ID, 2);


        }

    }

    public class StationCollection : ObservableCollection<Station>
    {
        public Dictionary<int, string> _dictionary = null;

        public StationCollection()
        {
            _dictionary = new Dictionary<int, string>();
        }
        public bool find(lineInfo lineInfo)
        {
            if (_dictionary.ContainsValue(lineInfo.Name)) return true;


            if (_dictionary.ContainsKey(lineInfo.ID)) return true;

            return false;
        }

        public string getStationName(int id)
        {
            IEnumerator<Station> se = this.GetEnumerator();
            while (se.MoveNext())
            {
                if (se.Current.ID == id)
                    return se.Current.Name;
            }
            return string.Empty;

        }


    }

    public class stationInfo
    {
        public int LineIndex { get; set; }
        public int ID { get; set; }
        public String Name { get; set; }
        public int ItemIndex { get; set; }

        public stationInfo()
        {
        }
    }
    public class Class : INotifyPropertyChanged
    {
        int department;

        int id = 0;
        public int ID
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("ID");
            }
        }

        string name = String.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
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

        public Class(int department, int ID, string Name)
        {
            this.department = department;
            this.ID = ID;
            this.Name = Name;

        }

    }

    public class ClassCollection : ObservableCollection<Class>
    {

        public string getClassName(int id)
        {
            IEnumerator<Class> se = this.GetEnumerator();
            while (se.MoveNext())
            {
                if (se.Current.ID == id)
                    return se.Current.Name;
            }
            return string.Empty;
        }


    }

    public class classInfo
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public int ItemIndex { get; set; }
        public int LineIndex { get; set; }
        public int StationIndex { get; set; }

        public classInfo()
        {
        }
    }


    #endregion
}
