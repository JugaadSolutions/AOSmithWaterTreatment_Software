using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows;
/*========================================================================================
Procedure/Module :  PRINTING USING SOCKET
Purpose          :  PRINTING USING SOCKET 
Created By       :  Paneendra
Created on       :  21-Apr-2012
Modified By      :  Paneendra
Modified on      :  ------------------
Copyright (c) Bar Code India Ltd. All rights reserved.
========================================================================================*/
namespace HDC_COMMSERVER
{
    public class BcilNetwork : IDisposable
    {           
            #region "Construcgtor"        
          
                   
            #endregion

            #region "Distructor"               
            ~BcilNetwork()
                {
                    Dispose (true);
                }
            #endregion                     

            #region PublicVariables

                public IPAddress PrinterIP { get; set; }
                public int PrinterPort { get; set; }
                
                           
                private Socket _Sock = null;                                
                private IPEndPoint serverEndPoint;
                
                private bool _IsDisposed = false;

                

            #endregion

            #region "NetworkPrinting"
                /// <summary>
                /// INITIALISE SOCKET
                /// </summary>
                /// <returns></returns>
                


                bool _InitializeSockClient()
                {
                    try
                    {                        
                        _Sock = null;
                      
                        serverEndPoint = new IPEndPoint(PrinterIP, PrinterPort);
                        _Sock = new Socket(serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        _Sock.Connect(serverEndPoint);
                        if (_Sock.Connected)                        
                            return true;                        
                        else
                        {
                           // _Logger.LogMessage(EventNotice.EventTypes.evtInfo,"_InitializeSockClient", "Initialze Socket Connection Failed........."  );                            
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        //_Logger.LogMessage(EventNotice.EventTypes.evtInfo, "_InitializeSockClient", ex.Message);                                                    
                        throw ex;
                    }
                }

        
                /// <summary>
                /// CHECK FOR SOCKET CONNECTED OR NOT
                /// </summary>
                /// <returns></returns>
                bool _IsSockConnected()
                {                    
                    try
                    {
                        if (_Sock == null)
                            return false;
                        if (_Sock.Connected == false)
                            return false;
                        return true ;
                    }
                    catch (System.Exception ex)
                    {
                        _SocklientTerminate();
                        //_Logger.LogMessage(EventNotice.EventTypes.evtError, "IsSockConnected", ex.Message);                                                                            
                        throw ex;
                    }                    
                }

                /// <summary>
                /// SOCKET TERMINATE
                /// </summary>
                void _SocklientTerminate()
                {
                    try
                    {
                        if (_Sock != null && _Sock.Connected)                        
                            _Sock.Close();                        
                        _Sock = null;
                    }
                    catch (Exception ex)
                    {
                        //_Logger.LogMessage(EventNotice.EventTypes.evtError, "SocklientTerminate", ex.StackTrace);                                                                                                    
                    }
                }
                /// <summary>
                /// DATA RECEIVE FROM PRINTER VIA SOCKET
                /// </summary>
                /// <param name="_client"></param>
                /// <returns></returns>
                string _Response(Socket _client)
                {
                    try
                    {
                        Byte[] _data = new Byte[8025];
                        SocketAsyncEventArgs _ar = new SocketAsyncEventArgs();
                        _client.ReceiveTimeout = 5000;
                        Int32 byteCount = _client.Receive(_data);
                        return System.Text.Encoding.ASCII.GetString(_data, 0, byteCount);                         
                    }
                    catch (Exception ex)
                    {
                        _SocklientTerminate();
                        //_Logger.LogMessage(EventNotice.EventTypes.evtError, "_Response", ex.StackTrace);
                        throw ex;                        
                    }
                }
                /// <summary>
                /// SEND DATA FROM SOCKET
                /// </summary>
                /// <param name="_sChkPrinterStatusFlag"></param>
                /// <returns></returns>
                public bool NetworkPrint(String data)
                {                                        
                    byte[] _dBuffer = System.Text.Encoding.ASCII.GetBytes(data);
                    try
                    {
                        if (_IsSockConnected() == false)
                        {
                            if (_InitializeSockClient() != false)                            
                                return false;                            
                        }                    
                        
                        _Sock.Send(_dBuffer);
                        return true;                        
                    }
                    catch (Exception ex)
                    {
                        
                        _SocklientTerminate();
                        //_Logger.LogMessage(EventNotice.EventTypes.evtError, "NetworkPrint", ex.StackTrace);                        
                        return false;                                    
                    }
                }
                /// <summary>
                /// Get Network Printer Status
                /// </summary>
                /// <returns></returns>
                public string NetworkPrinterStatus()
                {
                    string[] _Arr = null;
                    string _sReturn = "";
                    byte[] _dBuffer = System.Text.Encoding.ASCII.GetBytes("");                    
                    try
                    {
                        if (_IsSockConnected() == false)
                        {
                            if (_InitializeSockClient() != true)
                            {
                                _sReturn = "PRINTER NOT INITIALIZE";
                                return _sReturn;                            
                            }                                
                        }
                        //_dBuffer = System.Text.Encoding.ASCII.GetBytes("~HS");
                        //_Sock.Send(_dBuffer);
                        //_sReturn = _Response(_Sock);
                        ////_sReturn = "014,0,0,0411,000,0,0,0,000,0,0,0001,0,0,0,1,0,6,0,00000000,1,0001234,0";
                        //_dBuffer = System.Text.Encoding.ASCII.GetBytes("");
                        //_Arr = _sReturn.Split(',');
                        //if (_Arr.Length > 14)
                        //{
                        //    if (_Arr[1].Trim() == "1")
                        //        _sReturn = "PRINTER PAPER OUT";                                             
                        //    else if (_Arr[2].Trim() == "1")
                        //        _sReturn = "PRINTER IN PAUSE STATUS";                                                            
                        //    else if (_Arr[5].Trim() == "1")
                        //        _sReturn = "PRINTER BUFFER FULL";                                                           
                        //    else if (Convert.ToInt64(_Arr[4].Trim()) > 50)
                        //        _sReturn = "UNUSED BIT GREATER THAN 50";                                                           
                        //    else if (_Arr[14].Trim() == "1")
                        //        _sReturn = "PRINTER RIBBON OUT";                                                            
                        //    else
                        //        _sReturn = "PRINTER READY";                            
                        //}
                        //else
                        //    _sReturn = "UNKNOWN ERROR";
                        _sReturn = "PRINTER READY";      
                        return _sReturn;
                    }                    
                    catch (Exception ex)
                    {
                        _SocklientTerminate();
                       // _Logger.LogMessage(EventNotice.EventTypes.evtError, "NetworkPrinterStatus", ex.StackTrace);                        
                        _sReturn = "PRINTER NOT IN NETWORK";                       
                    }
                    return _sReturn;                    
                }
                #endregion

            #region "Dispose"
                /// <summary>
                /// Dispose
                /// </summary>
                /// <param name="IsDisposing"></param>
                protected virtual void Dispose(bool IsDisposing)
                {
                    if (_IsDisposed)
                        return;
                    if (IsDisposing)
                    {
                        // Free any managed resources in this section
                    }                    
                    _IsDisposed = true;
                }
                /// <summary>
                /// Dispose
                /// </summary>
                public void Dispose()
                {
                    _SocklientTerminate();                  
                    Dispose(true);
                    // Tell the garbage collector not to call the finalizer
                    // since all the cleanup will already be done.
                    GC.SuppressFinalize(true);
                }
                #endregion               
    }
}
