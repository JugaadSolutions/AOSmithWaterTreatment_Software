using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBenchApp.Entity
{
    public class DeviceAssociation
    {
        public int ID
        { get; set; }

        public string Header
        { get; set; }

        public int Line
        { get; set; }


        public String LineName
        { get; set; }

        public int Station
        {
            get;
            set;
        }


        public String StationName
        { get; set; }

        public DeviceAssociation()
        {
            ID = -1;

            Line = 0;
            LineName = String.Empty;

            Station = 0;
            StationName = String.Empty;

        }

        public DeviceAssociation(int id)
        {

            ID = id;

            Header = String.Empty;
            Line = 0;
            LineName = String.Empty;

            Station = 0;
            StationName = String.Empty;
        }
        public DeviceAssociation(int id, String header, int line, String lineName)
        {
            ID = id;
            Header = header;
            Line = line;
            LineName = lineName;
            Station = 0;
            StationName = String.Empty;
        }
        public DeviceAssociation(int id, String header, int line, String lineName, int station, String stationName)
        {
            ID = 1;
            Header = header;
            Line = line;
            LineName = lineName;
            Station = station;
            StationName = stationName;
        }
    }
}
