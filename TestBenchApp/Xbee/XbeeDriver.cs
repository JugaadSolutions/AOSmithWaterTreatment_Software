using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics;



namespace ias.devicedriver
{
    public class XbeeDriver
    {
        COMMUNICATION_MODE communicationMode;

        TraceSource _xbeeTrace = null;
        TextWriterTraceListener _xbeeTraceListener = null;
        String nodeIdentifier = String.Empty;
        public String NodeIdentifier
        {
            get { return nodeIdentifier; }

        }

        int sourceAddress;
        public int SourceAddress
        {
            get { return sourceAddress; }
        }



        public enum COMMUNICATION_MODE
        {
            RAW = 0,           //raw mode (AP parameter = 0 )
            API = 1,             //api mode without escape char
            API_ESC = 2         //api mode with escape characters
        };

        public static String[] AT_COMMANDS = { "NI" };

        public enum AT_COMMAND_INDEX
        {
            NI = 0
        };


        private enum ESCAPE_CHARS
        {
            STX = 0x7E,
            ESCAPE = 0x7D,
            XON = 0x11,
            XOFF = 0x13
        };

        private enum API_ID
        {
            ATCMD = 0x08,
            Tx_64 = 0x00,
            Tx_16 = 0x01,
            Rx_64 = 0x80,
            Rx_16 = 0x81,
            MODEMSTATUS = 0x8A,
            ATRSP = 0x88,
            TxSTATUS = 0x89

        };

        private enum AT_RESPONSE_STATUS
        {
            OK = 0x00,
            ERROR = 0x01,
            INVALID_COMMAND = 0x02,
            INVALID_PARAMETER = 0x03
        };


        public XbeeDriver(COMMUNICATION_MODE mode)
        {
            communicationMode = mode;
            _xbeeTrace = new TraceSource("xbeeTrace");
            _xbeeTrace.Switch = new SourceSwitch("xbeeTraceSwitch");


            String xbeeTraceFile = ConfigurationSettings.AppSettings["XBeeTraceFile"];

            if (xbeeTraceFile != String.Empty)
            {
                _xbeeTraceListener = new TextWriterTraceListener(xbeeTraceFile);
                _xbeeTrace.Listeners.Add(_xbeeTraceListener);
                _xbeeTrace.Switch.Level = SourceLevels.Information;
            }
            else
            {
                _xbeeTrace.Switch.Level = SourceLevels.Off;
            }

        }


        public List<Byte> getNodeIdentifierPacket()
        {
            if (communicationMode == COMMUNICATION_MODE.RAW)
            {
                //to do
            }

            List<Byte> xbeePacket = null;   //the xbee packet

            List<Byte> packetData = new List<byte>();   //create list to contain packet data
            packetData.Add((Byte)API_ID.ATCMD);                   //add api identifier        
            packetData.Add(0x52);


            packetData.Add(Convert.ToByte('N'));       //add AT command identifier
            packetData.Add(Convert.ToByte('I'));

            xbeePacket = packetize(packetData);

            return xbeePacket;
        }

        public List<Byte> getTxPacket(int deviceId, List<Byte> data)
        {
            if (communicationMode == COMMUNICATION_MODE.RAW)        //if communication mode is raw
            {
                return data;    //do nothing 
            }

            List<Byte> xbeePacket = null;   //the xbee packet

            List<Byte> packetData = new List<byte>();   //create list to contain packet data
            packetData.Add((Byte)API_ID.Tx_16);         //add api identifier        
            packetData.Add(0);                          //add 0 to disable response frame

            Byte[] deviceIdArray = new Byte[2];   //a temporary array to store the device id bytes
            deviceIdArray =
                BitConverter.GetBytes((short)deviceId); //get the bytes ....bytes are got in little endian format

            packetData.Add(deviceIdArray[1]);       //add device id MSB
            packetData.Add(deviceIdArray[0]);       //add device id LSB


            packetData.Add(1);                          //disable ACK 
            packetData.AddRange(data);

            xbeePacket = packetize(packetData);

            return xbeePacket;
        }
        //Summary:
        //Parses the xbee rx packet
        //generates exceptions on error


        public List<Byte> parseRxPacket(List<Byte> packet)
        {
            #region TRACE_CODE
            String traceString = DateTime.Now.ToString();
            foreach (Byte b in packet)
            {
                traceString += "0x" + b.ToString("x2") + " ";
            }
            traceString += Environment.NewLine;

            _xbeeTrace.TraceInformation(traceString);
            foreach (TraceListener l in _xbeeTrace.Listeners)
            {
                l.Flush();
            }

            #endregion

            List<Byte> parsedPacket = null;


            if (communicationMode == COMMUNICATION_MODE.RAW)
            {
                return packet;
            }

            if (packet.Contains((Byte)ESCAPE_CHARS.STX))
            {
                int lastIndex = 0;
                lastIndex = packet.FindLastIndex(findSTX);
                if (packet.Count >= lastIndex + 3)
                {
                    Byte[] packetLengthArray = { packet[lastIndex + 2], packet[lastIndex + 1] };                //msb and lsb of length
                    short packetLength = BitConverter.ToInt16(packetLengthArray, 0);    //convert to short
                    if (packet.Count < packetLength + lastIndex + 3)
                        return null;
                    else parsedPacket = new List<byte>();
                }

            }
            List<List<Byte>> packetList = new List<List<byte>>();
            while (packet.Contains((Byte)ESCAPE_CHARS.STX))
            {
                int lastIndex = 0;
                lastIndex = packet.FindLastIndex(findSTX);

                packetList.Insert(0, packet.GetRange(lastIndex, packet.Count-lastIndex ));
                packet.RemoveRange(lastIndex, packet.Count - lastIndex);
            }



            foreach (List<Byte> l in packetList)
            {
                List<Byte> parsedL = null;
                if (communicationMode == COMMUNICATION_MODE.API_ESC)
                {
                    parsedL = removeEscapeChars(l);
                }
                Byte[] packetLengthArray = { parsedL[2], parsedL[1] };                //msb and lsb of length
                short packetLength = BitConverter.ToInt16(packetLengthArray, 0);    //convert to short


                parsedL.RemoveAt(0); //remove the start delimiter   

                parsedL.RemoveRange(0, 2); //remove the length fields

                Byte checksum = parsedL[parsedL.Count - 1];       //get the received checksum

                parsedL.RemoveAt(parsedL.Count - 1);              //remove the checksum from the packet

                if (computeCheckSum(parsedL) != checksum)        //validate checksum
                {
                    throw new Xbee_Exception("Checksum Error");
                }

                if (packetLength != parsedL.Count)               //verify received byte count
                {
                    throw new Xbee_Exception("Recevied Length mismatch ");
                }
                if (parsedL[0] != (Byte)API_ID.Rx_16)            //verify api id
                {
                    throw new Xbee_Exception(" API identifier mismatch");
                }
                else
                {
                    parsedL.RemoveAt(0);                         //remove the api id
                }

                parsedL.RemoveRange(0, 4);       //remove the address , rssi , option bytes
                parsedPacket.AddRange(parsedL.GetRange(0, parsedL.Count));

            }

            return parsedPacket;
        }


        //List<Byte> parseRxPacket(List<Byte> packet)
        //{
        //    #region TRACE_CODE
        //    String traceString = DateTime.Now.ToString();
        //    foreach (Byte b in packet)
        //    {
        //        traceString += "0x" + b.ToString("x2") + " ";
        //    }
        //    traceString += Environment.NewLine;

        //    _xbeeTrace.TraceInformation(traceString);
        //    foreach (TraceListener l in _xbeeTrace.Listeners)
        //    {
        //        l.Flush();
        //    }

        //    #endregion

        //    List<Byte> parsedPacket = null;
            
            
        //    if (communicationMode == COMMUNICATION_MODE.RAW)
        //    {
        //        return packet;
        //    }

        //    if (packet.Contains((Byte)ESCAPE_CHARS.STX))
        //    {
        //        int lastIndex = 0;
        //        lastIndex = packet.FindLastIndex(findSTX);
        //        if (packet.Count >= lastIndex + 3)
        //        {
        //            Byte[] packetLengthArray = { packet[lastIndex + 2], packet[lastIndex + 1] };                //msb and lsb of length
        //            short packetLength = BitConverter.ToInt16(packetLengthArray, 0);    //convert to short
        //            if (packet.Count < packetLength + lastIndex + 3)
        //                    return null;
        //            else parsedPacket = new List<byte>();
        //        }

        //    }

        //    while (packet.Contains((Byte)ESCAPE_CHARS.STX))
        //    {
        //        if (communicationMode == COMMUNICATION_MODE.API_ESC)
        //        {
        //            packet = removeEscapeChars(packet);
        //        }
        //        int index = packet.FindIndex(findSTX);
        //        Byte[] packetLengthArray = { packet[index + 2], packet[index + 1] };                //msb and lsb of length
        //        short packetLength = BitConverter.ToInt16(packetLengthArray, 0);    //convert to short

        //        List<Byte> tempPacket = new List<byte>();
        //        tempPacket.AddRange(packet.GetRange(index, packetLength + 4));

        //        packet.RemoveRange(index, packetLength + 4);

        //        tempPacket.RemoveAt(0); //remove the start delimiter   



        //        tempPacket.RemoveRange(0, 2); //remove the length fields

        //        Byte checksum = tempPacket[tempPacket.Count - 1];       //get the received checksum

        //        tempPacket.RemoveAt(tempPacket.Count - 1);              //remove the checksum from the packet

        //        if (computeCheckSum(tempPacket) != checksum)        //validate checksum
        //        {
        //            throw new Xbee_Exception("Checksum Error");
        //        }

        //        if (packetLength != tempPacket.Count)               //verify received byte count
        //        {
        //            throw new Xbee_Exception("Recevied Length mismatch ");
        //        }
        //        if (tempPacket[0] != (Byte)API_ID.Rx_16)            //verify api id
        //        {
        //            throw new Xbee_Exception(" API identifier mismatch");
        //        }
        //        else
        //        {
        //            tempPacket.RemoveAt(0);                         //remove the api id
        //        }

        //        Byte[] deviceIdArray = new Byte[2];
        //        deviceIdArray[0] = tempPacket[1];                       //get LSB
        //        deviceIdArray[1] = tempPacket[0];                       //get MSB

        //        //deviceId = BitConverter.ToInt16(deviceIdArray, 0);  //convert bytes to short

        //        tempPacket.RemoveRange(0, 4);       //remove the address , rssi , option bytes
        //        parsedPacket.AddRange(tempPacket.GetRange(0 , tempPacket.Count));

        //    }

        //    return parsedPacket;
        //}



        public List<Byte> parseATresponse(List<Byte> packet, String atCommand)
        {
            #region TRACE_CODE
            String traceString = DateTime.Now.ToString();
            foreach (Byte b in packet)
            {
                traceString += "0x" + b.ToString("x2") + " ";
            }
            traceString += Environment.NewLine;

            _xbeeTrace.TraceInformation(traceString);
            foreach (TraceListener l in _xbeeTrace.Listeners)
            {
                l.Flush();
            }

            #endregion


            if (communicationMode == COMMUNICATION_MODE.RAW)
            {
                return packet;
            }

            if (packet[0] != (Byte)ESCAPE_CHARS.STX)        //check for start delimiter
            {
                throw new Xbee_Exception("Start Delimiter Error");
            }

            packet.RemoveAt(0); //remove the start delimiter           
            Byte[] packetLengthArray = { packet[1], packet[0] };    //msb and lsb of length
            short packetLength = BitConverter.ToInt16(packetLengthArray, 0);    //convert to short

            packet.RemoveRange(0, 2); //remove the length fields

            Byte checksum = packet[packet.Count - 1];     //get the received checksum


            if (communicationMode == COMMUNICATION_MODE.API_ESC)
            {
                packet = removeEscapeChars(packet);
            }

            packet.RemoveAt(packet.Count - 1);            //remove the checksum from the packet



            if (computeCheckSum(packet) != checksum)       //validate checksum
            {
                throw new Xbee_Exception("Checksum Error");
            }

            if (packetLength != packet.Count)       //verify received byte count
            {
                throw new Xbee_Exception("Recevied Length mismatch ");
            }
            if (packet[0] != (Byte)API_ID.ATRSP)    //verify api id
            {
                throw new Xbee_Exception(" API identifier mismatch");
            }
            else
            {
                packet.RemoveAt(0);     //remove the api id
            }

            packet.RemoveAt(0);         //remove frame id
            char[] atIds = new char[2];
            atIds[0] = (char)packet[0];       //get LSB
            atIds[1] = (char)packet[1];       //get MSB

            String atcmd = new string(atIds);



            if (atcmd != atCommand)
            {
                throw new Xbee_Exception(" AT command mismatch");
            }

            packet.RemoveRange(0, 2);

            AT_RESPONSE_STATUS status = (AT_RESPONSE_STATUS)packet[0];

            if (status != AT_RESPONSE_STATUS.OK)
            {
                String msg = " AT Response : {0}";
                msg = String.Format(msg, status);
                throw new Xbee_Exception(msg);
            }

            packet.RemoveAt(0);  //remove the status byte

            return packet;
        }

        public List<Byte> parsePacket(List<Byte> packet, String atCommand)
        {
            #region TRACE_CODE
            String traceString = DateTime.Now.ToString();
            foreach (Byte b in packet)
            {
                traceString += "0x" + b.ToString("x2") + " ";
            }
            traceString += Environment.NewLine;

            _xbeeTrace.TraceInformation(traceString);
            foreach (TraceListener l in _xbeeTrace.Listeners)
            {
                l.Flush();
            }

            #endregion

            List<Byte> returnPacket = null;

            if (communicationMode == COMMUNICATION_MODE.RAW)
            {
                return packet;
            }

            if (packet[0] != (Byte)ESCAPE_CHARS.STX)        //check for start delimiter
            {
                throw new Xbee_Exception("Start Delimiter Error");
            }

            packet.RemoveAt(0); //remove the start delimiter           
            Byte[] packetLengthArray = { packet[1], packet[0] };    //msb and lsb of length
            short packetLength = BitConverter.ToInt16(packetLengthArray, 0);    //convert to short

            packet.RemoveRange(0, 2); //remove the length fields

            Byte checksum = packet[packet.Count - 1];     //get the received checksum


            if (communicationMode == COMMUNICATION_MODE.API_ESC)
            {
                packet = removeEscapeChars(packet);
            }

            packet.RemoveAt(packet.Count - 1);            //remove the checksum from the packet
            if (computeCheckSum(packet) != checksum)       //validate checksum
            {
                throw new Xbee_Exception("Checksum Error");
            }

            if (packetLength != packet.Count)       //verify received byte count
            {
                throw new Xbee_Exception("Recevied Length mismatch ");
            }

            switch ((API_ID)packet[0])
            {
                case API_ID.Rx_16:
                    packet.RemoveAt(0);             //remove frame id
                    returnPacket = parseRx_16Packet(packet);
                    break;

                case API_ID.ATRSP:
                    packet.RemoveAt(0);
                    returnPacket = parseATresponse(packet); //remove frame id
                    break;

            }

            return returnPacket;
        }


        private bool findSTX(Byte b)
        {
            if (b == (Byte)ESCAPE_CHARS.STX)
                return true;
            else return false;
        }

        private List<Byte> parseRx_16Packet(List<Byte> packet)
        {
            Byte[] deviceIdArray = new Byte[2];
            deviceIdArray[0] = packet[1];                       //get LSB
            deviceIdArray[1] = packet[0];                       //get MSB

            sourceAddress = BitConverter.ToInt16(deviceIdArray, 0);  //convert bytes to short

            packet.RemoveRange(0, 4);       //remove the address , rssi , option bytes

            return packet;
        }


        private List<Byte> parseATresponse(List<Byte> packet)
        {
            char[] atIds = new char[2];
            atIds[0] = (char)packet[0];       //get LSB
            atIds[1] = (char)packet[1];       //get MSB

            String atcmd = new string(atIds);   //make the at string
            packet.RemoveRange(0, 2);           //remove the at identifier

            AT_RESPONSE_STATUS status = (AT_RESPONSE_STATUS)packet[0];

            if (status != AT_RESPONSE_STATUS.OK)        //check response status..throw exception
            {                                           //on anything other than ok.....
                String msg = " AT Response : {0}";
                msg = String.Format(msg, status);
                throw new Xbee_Exception(msg);
            }

            packet.RemoveAt(0);                         //remove the status byte

            switch (atcmd)
            {
                case "NI":
                    break;

                //parsing of other at commands to be done
            }
            return packet;
        }

        private List<Byte> packetize(List<Byte> packetData)
        {
            List<Byte> xbeePacket = new List<byte>();

            Byte[] packetLength = new Byte[2];
            packetLength = BitConverter.GetBytes((short)packetData.Count);  //get the length bytes

            Byte checksum = computeCheckSum(packetData);
            packetData.Add(checksum);


            if (communicationMode == COMMUNICATION_MODE.API_ESC)            //if escaped api mode
            {
                packetData = addEscape(packetData);
            }

            xbeePacket.Add((Byte)ESCAPE_CHARS.STX);                         //add start byte
            xbeePacket.Add(packetLength[1]);                                //add MSB
            xbeePacket.Add(packetLength[0]);                                //add LSB
            xbeePacket.AddRange(packetData);                                //add packetData


            return xbeePacket;
        }





        private static Byte computeCheckSum(List<Byte> data)
        {
            Byte checkSum = 0;
            foreach (Byte b in data)
                checkSum += b;
            return Convert.ToByte(0xFF - checkSum);
        }

        private static List<Byte> addEscape(List<Byte> data)
        {
            List<Byte> escapedData = new List<byte>();
            foreach (Byte b in data)
            {
                if ((b == (Byte)ESCAPE_CHARS.STX)
                         || (b == (Byte)ESCAPE_CHARS.ESCAPE)
                         || (b == (Byte)ESCAPE_CHARS.XOFF)
                         || (b == (Byte)ESCAPE_CHARS.XON)
                         )
                {
                    escapedData.Add((Byte)ESCAPE_CHARS.ESCAPE);
                    escapedData.Add((Byte)(b ^ 0x20));
                }
                else
                {
                    escapedData.Add(b);
                }
            }
            return escapedData;
        }

        private List<Byte> removeEscapeChars(List<Byte> data)
        {
            bool escapeFlag = false;       //index of the escape char
            try
            {
                for (int i = 0; i < data.Count - 1; i++)
                {
                    if (data[i] == (Byte)ESCAPE_CHARS.ESCAPE)
                    {
                        data[i + 1] ^= 0x20;
                        data.RemoveAt(i);
                        escapeFlag = true;
                    }

                }

                #region TRACE_CODE
                if (escapeFlag == true)
                {
                    String traceString = DateTime.Now.ToString() + "Escape ";
                    foreach (Byte b in data)
                    {
                        traceString += "0x" + b.ToString("x2") + " ";
                    }
                    traceString += Environment.NewLine;

                    _xbeeTrace.TraceInformation(traceString);
                    foreach (TraceListener l in _xbeeTrace.Listeners)
                    {
                        l.Flush();
                    }
                }

                #endregion
            }
            catch (Exception e)
            {
                throw e;
            }
            return data;
        }

        private static bool matchEscape(Byte b)
        {
            if (b == (Byte)ESCAPE_CHARS.ESCAPE)
                return true;
            else
                return false;
        }


    }

    public class Xbee_Exception : Exception
    {
        public String message = String.Empty;

        public Xbee_Exception(String msg)
        {
            message = msg;
        }
    }




}
