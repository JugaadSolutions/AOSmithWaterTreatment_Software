using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBenchApp.Entity
{
    public class Plan
    {
        public enum STATUS { ACTIVE,INACTIVE};

        public String Model { get; set; }

        public int Quantity { get; set; }
        public int Actual { get; set; }

        public DateTime Timestamp { get; set; }
        public STATUS Status { get; set; }   
        
        public String BSerialNo { get; set; }
        public String FSerialNo { get; set; }
        public String CombinationCode { get; set; }
    }
}
