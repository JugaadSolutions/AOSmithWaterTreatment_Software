using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ias.andonmanager
{

    public enum AndonCommand
    {
        CMD_GET_STATUS = 0x80,
        CMD_CLEAR_ISSUES = 0x81,
        CMD_SET_STATION_COUNT = 0x82,
        CMD_SET_PASSWORD = 0x83,
        CMD_GET_PASSWORD = 0x84,
        CMD_GET_STATION_COUNT = 0x85,
        CMD_PING = 0x86,
        CMD_RAISE_ISSUE = 0x88,
        CMD_RESOLVE_ISSUE = 0x88,
        CMD_HOOTER_ON = 0xA0,
        CMD_HOOTER_OFF = 0xA1

    };
   

    partial class AndonManager
    {


        private enum AndonResponse
        {
            RES_COMM_OK = 0x00
        };

        private enum ANDON_CONFIG{ LOG_ENTRY_SIZE = 40 , TIMESTAMP_SIZE = 4,};


        private class TransactionInfo
        {
            public int deviceId;
            public AndonCommand command;
            public List<Byte> data;


            public TransactionInfo(int deviceId, AndonCommand command, List<Byte> data)
            {
                this.deviceId = deviceId;
                this.command = command;
                this.data = new List<byte>();
                if (data != null)
                    this.data.AddRange(data);
            }
        };
    }
    public class LogEntry
    {
        int station;
        public int Station
        {
            get { return station; }
            set { station = value; }
        }

               
        int  department;
        public int Department
        {
            get { return department; }
            set { department = value; }

        }
        String data;
        public String Data
        {
            get { return data;}
            set { data = value; }
        }



        public LogEntry( int station , int department , String data)
        {
            this.station = station;
            this.department = department;
            this.data = data;
        }

        public LogEntry()
        {
            station = -1;
            department = -1;
            data = String.Empty;
        }


    

    }

}
