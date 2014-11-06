using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBenchApp.Entity
{
    public class Model
    {
        public enum Type { BODY=1 , FRAME=2 };
        public String Name { get; set; }
        public String Number { get; set; }
    }
}
