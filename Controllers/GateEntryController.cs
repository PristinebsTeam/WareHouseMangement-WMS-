using System;
using Microsoft.AspNetCore.Mvc;
using PristinefulfilApiPackage.Model.GateEntry;
using PristinefulfilApiPackage.App_Data;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Xml;
using System.Data.SqlClient;

namespace PristinefulfilApiPackage.Controllers
{
    [Produces("application/json")]
    [Route("api/[Controller]/[Action]")]
    public class GateEntryController : Controller
    {
        [HttpPost]
        public JsonResult NewGateEntry([FromBody] gateEntryfirst entryfirst)
        {
            Main objmain = new Main();
            DataSet ds = new DataSet();
            ds = objmain.newGateEntryHeader(entryfirst);
            return objmain.DataTableToJsonWithJsonNet(ds.Tables[0]);
        }
        [HttpPost]
        public JsonResult Get_Vendor_Detail([FromBody] gateVendorAPI vendorAPI)
        {
            Main objmain = new Main();
            JObject details = new JObject();
            DataTable dt = new DataTable();
            dt = objmain.vendor_nav(vendorAPI);
            if (dt.Rows.Count > 0)
            {
                string url = dt.Rows[0]["gateEntry_Vendor_Api_url"].ToString();
                string[] user1 = dt.Rows[0]["gateEntry_username"].ToString().Split("\\");
                string password = dt.Rows[0]["gateEntry_password"].ToString();
                string user = user1[1];
                string domain = user1[0];
                string URL_status = url;
                CredentialCache myCache = new CredentialCache();
                //create ntlm authentication it require 3 parameter user,password,domain
                myCache.Add(new Uri(URL_status), "NTLM", new NetworkCredential(user, password, domain));
                MyWebClient WebClient = new MyWebClient();
                WebClient.Credentials = myCache;
                string Result = WebClient.DownloadString(new Uri(URL_status));
                details = JObject.Parse(Result);
            }
            return new JsonResult(details);
        }
        [HttpPost]
        public JsonResult Get_Location_Detail([FromBody] gateVendorAPI aPI)
        {
            Main objmain = new Main();
            JObject details = new JObject();
            DataTable dt = new DataTable();
            dt = objmain.location_nav(aPI);
            if (dt.Rows.Count > 0)
            {
                string url = dt.Rows[0]["getEntry_Location_API_URL"].ToString();
                string[] user1 = dt.Rows[0]["gateEntry_username"].ToString().Split("\\");
                string password = dt.Rows[0]["gateEntry_password"].ToString();
                string user = user1[1];
                string domain = user1[0];
                string URL_status = url;
                CredentialCache myCache = new CredentialCache();
                //create ntlm authentication it require 3 parameter user,password,domain
                myCache.Add(new Uri(URL_status), "NTLM", new NetworkCredential(user, password, domain));
                MyWebClient WebClient = new MyWebClient();
                WebClient.Credentials = myCache;
                string Result = WebClient.DownloadString(new Uri(URL_status));
                details = JObject.Parse(Result);
            }
            return new JsonResult(details);
        }
        [HttpPost]
        public JsonResult Nav_Send_Response([FromBody] gateNavSend navSend)
        {
            Main objmain = new Main();
            DataSet ds = new DataSet();
            ds = objmain.NavSendData(navSend.Doctype, navSend.gateEntry, navSend.vendor_Num, navSend.location, navSend.createdby);
            try
            {
                if (ds.Tables[1].Rows.Count > 0)
                {
                    string apiresult = ds.Tables[1].Rows[0][0].ToString();
                    ds.Tables[1].Columns.Add("condition", typeof(System.String));
                    ds.Tables[1].Columns.Add("Message", typeof(System.String));
                    XmlDocument xmltest = new XmlDocument();
                    xmltest.LoadXml(apiresult);
                    XmlNodeList error = xmltest.GetElementsByTagName("faultstring");
                    if (error.Count > 0)
                    {
                        string erroroutput = error[0].InnerXml;
                        ds.Tables[1].Rows[0]["condition"] = "False";
                        ds.Tables[1].Rows[0]["Message"] = erroroutput;
                        return objmain.DataTableToJsonWithJsonNet(ds.Tables[1]);         // UI return if any NAV error occur                     
                    }
                    XmlNodeList success = xmltest.GetElementsByTagName("return_value");
                    if (success.Count > 0)
                    {
                        string successoutput = success[0].InnerXml;
                        ds.Tables[1].Rows[0]["condition"] = "True";
                        ds.Tables[1].Rows[0]["Message"] = successoutput;

                        return objmain.DataTableToJsonWithJsonNet(ds.Tables[1]);
                    }
                    return new JsonResult("BAD");
                }
                else
                {
                    return new JsonResult("BAD");
                }
            }
            catch (Exception ee)
            {
                return new JsonResult(ee.Message);
            }
        }
        [HttpPost]
        public JsonResult Nav_Send_DocNumber_transferOrder([FromBody] GetDocnumber DocnuberTranferOrder)
        {
            Main objmain = new Main();
            DataTable dt = new DataTable();
            string table = "";
            dt = objmain.ToDataTable(DocnuberTranferOrder.line);
            if (DocnuberTranferOrder.header.Type == "Transfer Order")
            {
                table = "gateEntry_tranfer_order";
                DataColumn Location = new DataColumn();
                Location.DataType = System.Type.GetType("System.String");
                Location.Caption = "Location";
                Location.ColumnName = "Location";
                Location.DefaultValue = DocnuberTranferOrder.header.Location;
                dt.Columns.Add(Location);
                Location.SetOrdinal(1);// to put the column in position 1;
            }
            if (DocnuberTranferOrder.header.Type == "Purchase Order")
            {
                table = "gateEntry_DocNum";
                DataColumn Vendor = new DataColumn();
                Vendor.DataType = System.Type.GetType("System.String");
                Vendor.Caption = "Vendor";
                Vendor.ColumnName = "Vendor";
                Vendor.DefaultValue = DocnuberTranferOrder.header.Vendor;
                dt.Columns.Add(Vendor);
                Vendor.SetOrdinal(1);// to put the column in position 1;
            }
            DataColumn AssignTo = new DataColumn();
            AssignTo.DataType = System.Type.GetType("System.String");
            AssignTo.Caption = "AssignTo";
            AssignTo.ColumnName = "AssignTo";
            AssignTo.DefaultValue = DocnuberTranferOrder.header.assignto;
            dt.Columns.Add(AssignTo);
            AssignTo.SetOrdinal(2);// to put the column in position 3;

            objmain.delete_previousdata(DocnuberTranferOrder.header.assignto);
            if (dt.Rows.Count > 0)
            {
                try
                {
                    using (SqlConnection con = objmain.GetConnection())   // insert line in bulk in pick line table
                    {
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                        {
                            //Set the database table name
                            sqlBulkCopy.DestinationTableName = table;
                            sqlBulkCopy.WriteToServer(dt);
                            con.Close();
                            return new JsonResult("Done");
                        }
                    }
                }
                catch (Exception)
                {
                    return new JsonResult("Unable to store data please contact to devloper or try again");
                }
            }
            else
            {
                return new JsonResult("There is No data to Insert");
            }

        } // NAV send doc number and transfer order in our system
        [HttpGet]
        public JsonResult All_gateEntry()
        {
            Main objmain = new Main();
            return objmain.allgateEntery();
        }
        [HttpPost]
        public JsonResult Specific_gateEntry_info([FromBody] gateEntryNUMBer nUMBer)
        {
            Main objmain = new Main();
            return objmain.specific_gateentry(nUMBer.gateEntrynumber);
        }
        [HttpPost]
        public JsonResult get_docnum_or_transfer_order([FromBody] docNum_tranferOrd docNum_trns)
        {
            Main objmain = new Main();
            return objmain.get_TO_DOCnum_UI(docNum_trns.assginto, docNum_trns.dropdownvalue, docNum_trns.type);

        }
        [HttpPost]
        public JsonResult Complete_try([FromBody] GateEntryValue gateEntryValue)
        {
            Main objmain = new Main();
            DataSet ds = new DataSet();
            ds = objmain.GateEntryUpdateinfo(gateEntryValue);
            try
            {
                if (ds.Tables[1].Rows.Count > 0)
                {
                    string apiresult = ds.Tables[1].Rows[0][0].ToString();
                    ds.Tables[1].Columns.Add("condition", typeof(System.String));
                    ds.Tables[1].Columns.Add("Message", typeof(System.String));
                    XmlDocument xmltest = new XmlDocument();
                    xmltest.LoadXml(apiresult);
                    XmlNodeList error = xmltest.GetElementsByTagName("faultstring");
                    if (error.Count > 0)
                    {
                        string erroroutput = error[0].InnerXml;
                        ds.Tables[1].Rows[0]["condition"] = "False";
                        ds.Tables[1].Rows[0]["Message"] = erroroutput;
                        return objmain.DataTableToJsonWithJsonNet(ds.Tables[1]);         // UI return if any NAV error occur                     
                    }
                    XmlNodeList success = xmltest.GetElementsByTagName("return_value");
                    if (success.Count > 0)
                    {
                        string successoutput = success[0].InnerXml;
                        ds.Tables[1].Rows[0]["condition"] = "True";
                        ds.Tables[1].Rows[0]["Message"] = successoutput;
                        return objmain.DataTableToJsonWithJsonNet(ds.Tables[1]);
                    }
                    return new JsonResult("BAD");
                }
                else
                {
                    return new JsonResult("BAD");
                }
            }
            catch (Exception)
            {
                return objmain.DataTableToJsonWithJsonNet(ds.Tables[0]);
            }
        }

        //CHECK
    }
}