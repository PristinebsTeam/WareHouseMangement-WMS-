using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PristinefulfilApiPackage.Model.Grn
{
    public class GRN
    {
        public GRN_Header header { get; set; }
        public List<GRN_Lines> lines { get; set; }
    }
    public class GRN_Header
    {
        public string GRN_BoxNo { get; set; }
        public string Gate_Entry_No { get; set; }
        public string Create_DateTime { get; set; }
        public string Vendor_Code { get; set; }
        public string Location_code { get; set; }
        public string Doc_No { get; set; }
        public string TotalBox { get; set; }
        public string status { get; set; }
        public string Vendor_BoxId { get; set; }
        public string EntryBy { get; set; }
    }
    public class GRN_Lines
    {
        public string GRN_BoxId { get; set; }
        public string Line_No { get; set; }
        public string Item_No { get; set; }
        public string Item_Description { get; set; }
        public string Quantity { get; set; }
        public string Qty_in_Box { get; set; }
        public string status { get; set; }
        public string Error { get; set; }
        public string DocNo { get; set; }
        public string Document_Type { get; set; }
    }
    public class ScanBoxId{
        public string box_id { get; set; }
        public string createdby { get; set; }
        public string  location { get; set; }
    }

    public class GRNdetails
    {
        public string GrnBoxid { get; set; }
    }
}
