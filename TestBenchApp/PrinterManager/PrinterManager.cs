using HDC_COMMSERVER;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Printer
{
    public class PrinterManager
    {
        #region PublicVariables

        public String BarcodeFileName { get; set; }
        public String CombinationCodeFileName { get; set; }
        public IPAddress IPAddress { get; set; }
        public int Port { get; set; }
    
        #endregion


        BcilNetwork Driver;
        #region Constructor
        public PrinterManager()
        {


            
        }
        #endregion

        #region Public Methods

        public void SetupDriver()
        {
            Driver = new BcilNetwork { PrinterIP = IPAddress, PrinterPort = Port };
        }
        public bool PrintBarcode(String Model,String SerialNo)
        {
            try
            {
                String BarcodeData = File.ReadAllText(BarcodeFileName);
                BarcodeData = BarcodeData.Replace("{MODEL}", Model);
                BarcodeData = BarcodeData.Replace("{SERIAL}", SerialNo);
                return Driver.NetworkPrint(BarcodeData);


                
            }
            catch (Exception e)
            {
                return false;
            }
        }


        #endregion
    }
}
