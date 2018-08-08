using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PristinefulfilApiPackage.Model.Physical
{
    public class Physical
    {
    }
    public class FirstData
    {
        public string email { get; set; }
        public string location { get; set; }
    }
    public class Binscan
    {
        public string bincode { get; set;}
    }
    public class ItemCodeScan
    {
        public string physcialHeaderNum { get; set; }
        public string bincode { get; set; }
        public string itemcode { get; set; }
        public string loc { get; set; }
        public string createdby { get; set; }
    }
    public class Complete
    {
        public string physcalHeaderNum { get; set; }
        public string assign_person { get; set; }
        public string location { get;set; }
    }
}
