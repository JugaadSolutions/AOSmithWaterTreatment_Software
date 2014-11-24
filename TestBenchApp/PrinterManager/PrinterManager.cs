using HDC_COMMSERVER;
using prjParagon_WMS;
using shared.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
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

        public String CombinationTemplate { get; set; }
        public IPAddress IPAddress { get; set; }
        public int Port { get; set; }
        public String CombinationPrinterName { get; set;}
        

        #endregion


        Dictionary<String,BcilNetwork> Drivers;
        #region Constructor
        public PrinterManager()
        {

            Drivers = new Dictionary<string, BcilNetwork>();
            
        }
        #endregion

        #region Public Methods

        public void SetupDriver(String name, IPAddress ipaddr, int port, String template)
        {
            BcilNetwork Driver = new BcilNetwork {
                            PrinterIP = ipaddr, 
                            PrinterPort = port,
                            Template = template};
            if (Driver.initialize())
                Drivers.Add(name, Driver);
        }



        public bool PrintBarcode(String name, String Model,String ModelCode, String date,String serialno)
        {
            try
            {
                if (Drivers.ContainsKey(name))
                {

                    String BarcodeData = File.ReadAllText(Drivers[name].Template);
                    BarcodeData = BarcodeData.Replace("{MODEL}", Model);
                    BarcodeData = BarcodeData.Replace("B163A>51401010001", ModelCode + ">5" + date + serialno);
                    return Drivers[name].NetworkPrint(BarcodeData);
                }
                return false;


                
            }
            catch (Exception e)
            {
                return false;
            }
        }

     

       

        public bool PrintCombSticker(Model m, String barCode)
        {
            try
            {
                String CombStickerData = File.ReadAllText(CombinationTemplate);
                CombStickerData = CombStickerData.Replace("PRODUCT", m.Product);
                CombStickerData = CombStickerData.Replace("PRODNO", m.ProductNumber);
                CombStickerData = CombStickerData.Replace("{MRP}", m.MRP.ToString());
                CombStickerData = CombStickerData.Replace("MODELNAME", m.Name);
                CombStickerData = CombStickerData.Replace("STORAGECAPACITY", Convert.ToInt32(m.StorageCapacity).ToString());
                CombStickerData = CombStickerData.Replace("NETQUANTITY", Convert.ToInt32(m.NetQuantity).ToString());
                CombStickerData = CombStickerData.Replace("B163A>51401010001", m.Code + ">5" + barCode.Substring(4,6) + barCode.Substring(10,4));
                CombStickerData = CombStickerData.Replace("{W}X{D}X{H}", Convert.ToInt32(m.Width).ToString()
                    +" X "+Convert.ToInt32(m.Depth).ToString()+" X " +Convert.ToInt32(m.Height).ToString());
                CombStickerData = CombStickerData.Replace("MM/YYYY", barCode.Substring(6, 2) + "/" + "20" + barCode.Substring(4, 2));

                CombStickerData = CombStickerData.Replace("12345678901234", barCode);


                return clsPrint.SendStringToPrinter(CombinationPrinterName, CombStickerData);
            }
            catch
            {
                return false;
            }
        }



        #endregion
    }
}
