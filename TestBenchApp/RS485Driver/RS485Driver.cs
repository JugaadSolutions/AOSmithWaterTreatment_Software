using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ias.devicedriver
{
    public class RS485Driver
    {

 
        public Byte CMD_SOF = 0xAA;
        public Byte CMD_EOF = 0xBB;
        public Byte RESP_SOF = 0xCC;
        public Byte RESP_EOF = 0xDD;

        public RS485Driver()
        {
        }
            

        public List<Byte> Packetize(Byte deviceId, Byte cmd, List<Byte> data)
        {
            List<Byte> rs485Packet = null;
            List<Byte> packetData = null;

            
            Byte packetLength = 0x01;

            rs485Packet = new List<byte>();

            packetData = new List<byte>();

            packetData.Add(deviceId);     //add device id 
            if (data != null)
            {
                packetLength += Convert.ToByte(data.Count);
            }
            packetData.Add(packetLength);
            packetData.Add(Convert.ToByte(cmd));

            if (data != null)
            {
                packetData.AddRange(data);
            }

            Byte checksum = computeChecksum(packetData);
            
            rs485Packet.Add(CMD_SOF);                   //add start header
            rs485Packet.AddRange(packetData);       //add the data
            rs485Packet.Add(checksum);              //add checksum
            rs485Packet.Add(CMD_EOF);                   //add end trailer

            return rs485Packet;
        }

        public bool Parse(List<Byte> packet, out int status, out int deviceId, out List<Byte> packetData)
        {
            bool result = true;

            Byte dataLength = 0x00;
            packetData = null;
            status = 0xFF;
            deviceId = 0xFF;

            if (packet == null || (packet.Count < 5))
                return false;

            if ((packet[0] != RESP_SOF) || (packet[packet.Count - 1] != RESP_EOF))   //check header and trailer
            {
                return false;
            }

            packet.RemoveAt(0);                     //remove header
            packet.RemoveAt(packet.Count - 1);      //remove trailer


            Byte receivedChecksum = packet[packet.Count - 1];   //get the received checksum
            packet.RemoveAt(packet.Count - 1);                  //remove the checksum

            if (computeChecksum(packet) != receivedChecksum)   //checksum validation
            {
                return false;
                
                
            }
            deviceId = packet[0];
            //dataLength = packet[1];
            status = packet[1];
            packet.RemoveRange(0, 2);

            if (packet.Count == 0)
            {
                deviceId = 0xFA;
                return true;
            }

            byte[] deviceIDBytes = new byte[] { (Byte)(packet[1]-0x30),(Byte)( packet[0]-0x30)};

            deviceId = deviceIDBytes[1] * 10 + deviceIDBytes[0];

            packet.RemoveRange(0, 2);

            if (packet.Count > 0)
            {
                packetData = packet;
            }
                       
            return result;
        }

        private static Byte computeChecksum(List<Byte> data)
        {
            Byte checkSum = 0x00;
            foreach (Byte b in data)
            {
                checkSum ^= b;
            }
            return checkSum;
        }


    }

     
}
