using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PristinefulfilApiPackage.Model
{
    public class Login_Android
    {
        public string email { get; set; }
        public string password { get; set; }
    }
    public class AndroidNavBardata
    {
        public string flag { get; set; }
        public string email { get; set; }
        public int parent_id { get; set; }

    }
    public class Location
    {
        public int id { get; set; }
        public string email { get; set; }
        public string loc { get; set; }

    }
    public class InsertBarcode
    {
        public string scanbarcode12 { get; set; }
        public string documentno { get; set; }
        public string documentfromshelf { get; set; }
        public string toshefinputbyuser { get; set; }
        public string email { get; set; }
        public string Location { get; set; }

    }

    public class logout
    {
        public string emailaddress { get; set; }
    }
    public class GetdocumentNo
    {
        public string bincode { get; set; }

    }
    public class Editbarcode
    {
        public string DocNo { get; set; }
        public string barcode { get; set; }
        public string Qty { get; set; }
        public string toshelf { get; set; }
        public string email { get; set; }
        public string Location { get; set; }

    }

    public class Put_complete
    {
        public string docno { get; set; }
        public string email { get; set; }
        public string location { get; set; }
    }
    //Anoop
    public class Iteminfo
    {
        public string item_code { get; set; }
        public string descrip { get; set; }
        public string barcode { get; set; }
    }
    public class Bininfo
    {
        public string shelf_id { get; set; }
    }
    //for nav bar

    public class Binnavbar {
        public string flag { get; set; }
        public int parent_id { get; set; }
    }

    public class SignUP
    {
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string location { get; set; }
        public string role { get; set; }
    }


    //Anoop
}
