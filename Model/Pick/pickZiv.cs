using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PristinefulfilApiPackage.Model.Pick
{
    public class PickData
    {
        public List<pickZivline> line { get; set; }
    }
    public class  pickZivline
    {
        public string Warehouse_docno { get; set; }
        public int warehouse_line { get; set; } 
        public string SubpickNo { get; set; }
        public string pick_lineNo { get; set; }
        public string activity_type { get; set; }
        public string sourceno { get; set; }
        public string source_lineno { get; set; }
        public string source_document { get; set; }
        public string locationcode { get; set; }
        public string itemno { get; set; }
        public string description { get; set; }
        public string qty { get; set; }
        public string bincode { get; set; }
        public string order_no { get; set; }
        public string barcode { get; set; }
        public string assgineto { get; set; }
        public string pickno { get; set; }   
        public string sales_Line_parent { get; set; }
    }

    public class Scanbarcode
    {
        public string barcodeno { get; set; }
        public string bicode { get; set; }
        public string pickno { get; set; }
        public string Pick_Line_No { get; set; }
        public string location { get; set; }
        public string email { get; set; }
    }
    public class Scanbin
    {
        public string scanbincode { get; set; }
        public string location { get; set; }
        public string email { get; set; }
    }
    public class NotFoundCaseData
    {
        public string scanbin { get; set; }
        public string barcode { get; set; }
        public string pickno { get; set; }
        public string Pick_Line_No { get; set; }
        public int count { get; set; }
        public string location { get; set; }
        public string email { get; set; }

    }
    public class complete_pickno
    {
        public string pickno { get; set; }
        public string location { get; set; }
        public string email { get; set; }
    }

    public class Bindpickk
    {
        public string email { get; set; }
        public string selectLoc { get; set; }
    }

}
