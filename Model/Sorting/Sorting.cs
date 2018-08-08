using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PristinefulfilApiPackage.Model.Sorting
{
    public class Sorting
    {
        public string assign_person { get; set; }
        public string email { get; set; }
        //public string pick_no { get; set; }
    }

    public class Sort
    {
        public string clear_slot { get; set; }
    }

    public class scanitem
    {
        public string assign_person { get; set; }
        public string item_barcode { get; set; }
        public string order_number { get; set; }
        public string email { get; set; }
        //public string pickno { get; set; }
    }

    public class ScanCom
    {
        public string order_no { get; set; }
        //public string pickno { get; set; }
        public string assignto { get; set; }
        public string SlotbarCode { get; set; }
    }
}
