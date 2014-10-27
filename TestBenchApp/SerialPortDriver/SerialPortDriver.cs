using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Configuration;
using System.Threading;

namespace ias.devicedriver
{
    public class SerialPortDriver : SerialPort
    {
        public enum TRANSACTION_MODE { NONE = 0, DELAY_BASED = 1, EVENT_BASED = 2 };
        int sleepOnWrite = 0;
        public bool abort = false;

        public byte startOfPacket = 0xFF;
        public int startOfPacketIndex = -1;
        public byte endOfPacket = 0xFF;
        public int endOfPacketIndex = -1;

       

        List<Byte> rxData = null;

        public bool packetCollection = false;


        public SerialPortDriver()
        {
            BaudRate = 9600;
            DataBits = 8;
            StopBits = StopBits.One;
            Parity = Parity.None;
            Handshake = Handshake.None;

            sleepOnWrite = int.Parse(ConfigurationSettings.AppSettings["SleepOnWrite"]);
            ReadTimeout = 10000;
            WriteTimeout = 10000;
        }

        public SerialPortDriver(int baudRate , int dataBits ,StopBits stopBits, Parity parity , Handshake handShake)
        {
            BaudRate = baudRate;
            DataBits = dataBits;
            StopBits = stopBits;
            Parity = parity;
            Handshake = handShake;

            sleepOnWrite = int.Parse(ConfigurationSettings.AppSettings["SleepOnWrite"]);
            ReadTimeout = 10000;
            WriteTimeout = 10000;
        }



        //public SerialPortDriver(String portName)
        //{
        //    BaudRate = 9600;
        //    DataBits = 8;
        //    StopBits = StopBits.One;
        //    Parity = Parity.None;
        //    Handshake = Handshake.None;
        //    PortName = portName;
        //    sleepOnWrite = int.Parse(ConfigurationSettings.AppSettings["SleepOnWrite"]);
        //    ReadTimeout = 100;
        //    WriteTimeout = 100;
        //}

        public  bool open()
        {
            try
            {
                PortName = ConfigurationSettings.AppSettings["PORT"];

                Open();
                return true;
            }
            catch (System.InvalidOperationException)
            {
                return false;
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return false;
            }
            catch (System.ArgumentException)
            {
                return false;
            }

            catch (System.IO.IOException)
            {
                return false;
            }

            catch (System.UnauthorizedAccessException)
            {
                return false;
            }
        }

        public bool open(String port)
        {

            try
            {
                PortName = port;
                Open();
                return true;
            }
            catch (System.InvalidOperationException)
            {
                return false;
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return false;
            }
            catch (System.ArgumentException)
            {
                return false;
            }

            catch (System.IO.IOException)
            {
                return false;
            }

            catch (System.UnauthorizedAccessException)
            {
                return false;
            }
        }


        public void WriteToPort(String data)
        {
            char[] dataArr = data.ToCharArray();
            for (int i = 0; i < data.Length; i++)
            {
                if (abort == false)
                {
                    Write(dataArr, i, 1);
                    //Thread.Sleep(sleepOnWrite);
                }
            }
        }

        public void WriteToPort(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (abort == false)
                {
                    Write(data, i, 1);
                    Thread.Sleep(sleepOnWrite);
                }
            }
        }
    }


}
