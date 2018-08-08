using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PristinefulfilApiPackage.Model.GateEntry
{
    public class GateEntry
    {
    }
    public class gateEntryfirst
    {
        public string location { get; set; }
        public string maker { get; set; }
    }
    public class gateVendorAPI
    {
        public string location { get; set; }
    }
    public class gateNavSend
    {
        public string gateEntry { get; set; }
        public string Doctype { get; set; }
        public string vendor_Num { get; set; } = "";
        public string location { get; set; } = "";
        public string createdby { get; set; }
    }
    public class GetDocnumber
    {
        public checkright header { get; set; }
        public List<DocTO> line { get; set; }
    }
    public class checkright
    {
        public string Type { get; set; }
        public string Location { get; set; }
        public string Vendor { get; set; }
        public string assignto { get; set; }
    }
    public class DocTO
    {
        public string information { get; set; }
    }
    public class gateEntryNUMBer
    {
        public string gateEntrynumber { get; set; }
    }
    public class docNum_tranferOrd
    {
        public string type { get; set; }
        public string assginto { get; set; }
        public string dropdownvalue { get; set; }
    }
    public class GateEntryValue
    {
        public string gateentry_number { get; set; }
        public string VehicleNo { get; set; }
        public string Freight { get; set; }
        public string Freight_Amount { get; set; } = "";
        public string DocType { get; set; }
        public string VendorNo { get; set; } = "";
        public string Transfer_location { get; set; } = "";
        public string Purchase_number { get; set; } = "";
        public string Transfer_number { get; set; } = "";
        public string No_of_Boxes { get; set; }
        public string assignPerson { get; set; }
    }
}
