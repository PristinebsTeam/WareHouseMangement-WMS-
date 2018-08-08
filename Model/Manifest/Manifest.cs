using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PristinefulfilApiPackage.Model.Manifest
{
    public class Manifest
    {
        public string locatin { get; set; }
        public string createdby { get; set; }
        public string dsp_code { get; set; }
    }

    public class Manifest_Line
    {
        public string manifest_no { get; set; }
        public string line_id { get; set; }
        public string createdon { get; set; }
        public string createdby { get; set; }
        public string dsp_partner_code { get; set; }
        public string customer_id { get; set; }
        public string cust_name { get; set; }
        public string cust_add { get; set; }
        public string cust_city { get; set; }
        public string cust_state { get; set; }
        public string cust_pincode { get; set; }
        public string cust_phoneno { get; set; }
        public string awb_no  { get; set; }
        public decimal weight { get; set; }
        public string country { get; set; }
        public string order_no { get; set; }
        public string dsp_partner_name { get; set; }
        public string second_courier_dispatch { get; set; }
        public string posted_invoice_no { get; set; }
        public string pay_method { get; set; }
        public string cancelled_order { get; set; }
        public decimal totalamount { get; set; }
        public string locatin { get; set; }
    }
}
