using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PristinefulfilApiPackage.Model.Pick;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using System.Xml;
using PristinefulfilApiPackage.App_Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PristinefulfilApiPackage.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class PickZivController : Controller
    {
        [HttpPost]
        public string Post([FromBody] PickData NavData)         //for navision to insert new pick in database
        {
            Main objmain = new Main();
            string navresult = "Unsucces Pick No." + NavData.line[0].pickno;
            DataTable dt = new DataTable();
            dt = ToDataTable(NavData.line);
            if (dt.Rows.Count > 0)
            {
                using (SqlConnection con = objmain.GetConnection()) 
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(objmain.GetDataString(), SqlBulkCopyOptions.FireTriggers))
                    {
                        sqlBulkCopy.DestinationTableName = "Pick_Line";
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();
                    }
                }
                navresult = "Succes Pick No." + NavData.line[0].pickno;
            }
            return navresult;
        }
        // notfound case insert a line into database by navion
        public string Insert_Line_By_Navision([FromBody]pickZivline line)         //for navision to insert new pick in database
        {
            Main objmain = new Main();
            string navresult = "Unsucces New Line Of This Pick No." + line.pickno;
            try
            {
                int result = objmain.Insert_Pick_Line(line);
                if (result > 0)
                {
                    navresult = "succes New Line Of This Pick No." + line.pickno;
                }
            }
            catch (Exception)
            {
                navresult = "Exception New Line Of This Pick No." + line.pickno;
            }
            return navresult;//"Pickno:---" + pichead.header.pickno + "Line data_" + pichead.line[0].pickno + "     ---" + pichead.line[1].pickno + "result---" + result +"count==="+ dt.Rows.Count;
        }
        //for list add into datatable
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }          //list data objects convert into datatable
        public string Cancel_Pick_Order([FromBody]pickZivline line)
        {
            Main objmain = new Main();
            string order_no = line.order_no;
            string status = null;
            int result = objmain.Cancel_Pick_Order(order_no);
            if (result > 0)
            {
                status = "Success Cancel Order";
            }
            else
            {
                status = "Unsuccess Cancel Order";
            }
            return status;
        }
        //for send data to pick component
        public JsonResult Get_Pick_Detail([FromBody]Bindpickk loc)           //bind pick data by this method
        {
            Main objmain = new Main();
            DataSet ds = new DataSet();
            string useremail = loc.email.ToString();
            ds = objmain.Get_Pick_Detail(Convert.ToInt32(loc.selectLoc.ToString()), useremail);
            try
            {
                DataColumn pending_pick = new DataColumn();
                pending_pick.ColumnName = "pending_pick";
                pending_pick.DefaultValue = ds.Tables[1].Rows[0][0].ToString();
                ds.Tables[0].Columns.Add(pending_pick);
            }
            catch (Exception ee)
            {

            }
            return objmain.DataTableToJsonWithJsonNet(ds.Tables[0]);

        }
        //for check bin its exist or not in  our database zivame
        public JsonResult Check_Scan_Bin([FromBody]Scanbin Scan)           // check scaned bin is exist in our db
        {
            Main objmain = new Main();
            DataTable dt = new DataTable();
            dt = objmain.check_Scan_Bin_Detail(Scan);
            if (dt.Rows.Count > 0)
            {
                JValue value1 = new JValue("True");
                JObject Jobj1 = new JObject();
                Jobj1["condition"] = value1;
                return new JsonResult(Jobj1);
            }
            JValue value = new JValue("False");
            JObject Jobj = new JObject();
            Jobj["condition"] = value;
            return new JsonResult(Jobj);
        }
        //check barcode into our database
        public JsonResult Check_Scan_Barcodes([FromBody]Scanbarcode barcodeDetail)     // all  operation douring scan barcode
        {
            Main objmain = new Main();
            DataTable dt = new DataTable();
            try
            {
                dt = objmain.check_Scan_Barcode_Detail(barcodeDetail);        //check barcode is exist or not
                if (dt.Rows.Count > 0)
                {
                    dt = objmain.pick_Transfer_API_Procedure(barcodeDetail);     // if barcoe  is exist then send request to navision  and get response
                    if (dt.Rows.Count > 0)
                    {
                        string apiresult = dt.Rows[0][0].ToString();
                        XmlDocument xmltest = new XmlDocument();
                        xmltest.LoadXml(apiresult);
                        XmlNodeList error = xmltest.GetElementsByTagName("faultstring");
                        if (error.Count > 0)
                        {
                            int increaseqty = objmain.Pick_Increase_Qty_Procedure(barcodeDetail, "ERROR");
                            if (increaseqty > 0)
                            {
                                DataSet ds = new DataSet();
                                ds = objmain.Get_Pick_Detail(Convert.ToInt32(barcodeDetail.location), barcodeDetail.email);
                                DataColumn pending_pick = new DataColumn();
                                pending_pick.ColumnName = "pending_pick";
                                pending_pick.DefaultValue = ds.Tables[1].Rows[0][0].ToString();
                                ds.Tables[0].Columns.Add(pending_pick);
                                return objmain.Add_Condition(ds.Tables[0], "Error", error[0].InnerXml);
                            }
                            else
                            {
                                DataSet ds = new DataSet();
                                ds = objmain.Get_Pick_Detail(Convert.ToInt32(barcodeDetail.location), barcodeDetail.email);
                                DataColumn pending_pick = new DataColumn();
                                pending_pick.ColumnName = "pending_pick";
                                pending_pick.DefaultValue = ds.Tables[1].Rows[0][0].ToString();
                                ds.Tables[0].Columns.Add(pending_pick);
                                return objmain.Add_Condition(ds.Tables[0], "Error", string.Concat(error[0].InnerXml, "also error in Wrapper"));
                            }
                        }
                        XmlNodeList success = xmltest.GetElementsByTagName("return_value");
                        if (success.Count > 0)
                        {
                            int increaseqty = objmain.Pick_Increase_Qty_Procedure(barcodeDetail, "UpQTYPICK");    //increase his pick qty
                            if (increaseqty > 0)
                            {
                                DataSet ds = new DataSet();
                                ds = objmain.Get_Pick_Detail(Convert.ToInt32(barcodeDetail.location), barcodeDetail.email);
                                DataColumn pending_pick = new DataColumn();
                                pending_pick.ColumnName = "pending_pick";
                                pending_pick.DefaultValue = ds.Tables[1].Rows[0][0].ToString();
                                ds.Tables[0].Columns.Add(pending_pick);
                                return objmain.Add_Condition(ds.Tables[0], "True", "Cool Scan, Now Scan your Bin");
                            }
                            else
                            {
                                DataSet ds = new DataSet();
                                ds = objmain.Get_Pick_Detail(Convert.ToInt32(barcodeDetail.location), barcodeDetail.email);
                                DataColumn pending_pick = new DataColumn();
                                pending_pick.ColumnName = "pending_pick";
                                pending_pick.DefaultValue = ds.Tables[1].Rows[0][0].ToString();
                                ds.Tables[0].Columns.Add(pending_pick);
                                return objmain.Add_Condition(ds.Tables[0], "True", "Cool Scan but there is some problem on Our Side");
                            }
                        }
                        return new JsonResult("API Mistake");
                    }                                   //if api hit read it output
                    else
                        return new JsonResult("Error In database Repeat this condition");
                }
                else
                {
                    JValue value = new JValue("False");
                    JObject Jobj = new JObject();
                    Jobj["condition"] = value;
                    return new JsonResult(Jobj);
                }
            }
            catch (Exception ee)
            {
                return objmain.Add_Condition(dt, "Error", ee.ToString());
            }
        }
        //for not fond case send request to navision to get new line
        public JsonResult NotFoundClick([FromBody]NotFoundCaseData Notfound)
        {
            Main objmain = new Main();
            string result = objmain.Pick_Not_Found_Api_Hit(Notfound);
            XmlDocument xmltest = new XmlDocument();
            xmltest.LoadXml(result);
            XmlNodeList error = xmltest.GetElementsByTagName("faultstring"); //check for error
            if (error.Count > 0)
            {
                if (error[0].InnerXml == "NOTAVAILABLE")
                {
                    int NotfoundPass = objmain.Not_Found_Pash_ToManage_Qty(Notfound, "NOTAVAILABLE");
                    if (NotfoundPass > 0)
                    {
                        DataSet ds = new DataSet();
                        ds = objmain.Get_Pick_Detail(Convert.ToInt32(Notfound.location), Notfound.email);
                        DataColumn pending_pick = new DataColumn();
                        pending_pick.ColumnName = "pending_pick";
                        pending_pick.DefaultValue = ds.Tables[1].Rows[0][0].ToString();
                        ds.Tables[0].Columns.Add(pending_pick);
                        return objmain.Add_Condition(ds.Tables[0], "Error", "Sorry, This item is currently not available in warehouse");
                    }
                    else
                    {
                        DataSet ds = new DataSet();
                        ds = objmain.Get_Pick_Detail(Convert.ToInt32(Notfound.location), Notfound.email);
                        DataColumn pending_pick = new DataColumn();
                        pending_pick.ColumnName = "pending_pick";
                        pending_pick.DefaultValue = ds.Tables[1].Rows[0][0].ToString();
                        ds.Tables[0].Columns.Add(pending_pick);
                        return objmain.Add_Condition(ds.Tables[0], "Error", "Sorry, This item is currently not available in warehouse but Encounter a Problem in API");
                    }
                }
                else
                {
                    DataSet ds = new DataSet();
                    ds = objmain.Get_Pick_Detail(Convert.ToInt32(Notfound.location), Notfound.email);
                    DataColumn pending_pick = new DataColumn();
                    pending_pick.ColumnName = "pending_pick";
                    pending_pick.DefaultValue = ds.Tables[1].Rows[0][0].ToString();
                    ds.Tables[0].Columns.Add(pending_pick);
                    return objmain.Add_Condition(ds.Tables[0], "Error", error[0].InnerXml);
                }
            }
            //check for true
            XmlNodeList success = xmltest.GetElementsByTagName("return_value");
            if (success.Count > 0)
            {
                int NotfoundPass = objmain.Not_Found_Pash_ToManage_Qty(Notfound, "NOTFOUNDPASS");
                if (NotfoundPass > 0)
                {
                    DataSet ds = new DataSet();
                    ds = objmain.Get_Pick_Detail(Convert.ToInt32(Notfound.location), Notfound.email);
                    DataColumn pending_pick = new DataColumn();
                    pending_pick.ColumnName = "pending_pick";
                    pending_pick.DefaultValue = ds.Tables[1].Rows[0][0].ToString();
                    ds.Tables[0].Columns.Add(pending_pick);
                    return objmain.Add_Condition(ds.Tables[0], "True", "New Line Inserted Continue Scaning Process....");
                }
                else
                {
                    DataSet ds = new DataSet();
                    ds = objmain.Get_Pick_Detail(Convert.ToInt32(Notfound.location), Notfound.email);
                    DataColumn pending_pick = new DataColumn();
                    pending_pick.ColumnName = "pending_pick";
                    pending_pick.DefaultValue = ds.Tables[1].Rows[0][0].ToString();
                    ds.Tables[0].Columns.Add(pending_pick);
                    return objmain.Add_Condition(ds.Tables[0], "Error", string.Concat("Not Found Pass But Quantity Not Manage of This Barcode:", Notfound.barcode));
                }
            }
            return new JsonResult("API Busted");
        }
        public JsonResult All_Pending_pick([FromBody] Bindpickk Pending)
        {
            Main objmain = new Main();
            DataTable dt = objmain.allpending_pick(Pending.email, Pending.selectLoc);
            if (dt.Rows.Count == 0)
            {
                dt.Columns.Add("condition", typeof(System.String));
                dt.Columns.Add("Message", typeof(System.String));
                dt.Rows.Add();
                dt.Rows[0]["condition"] = "False";
                dt.Rows[0]["Message"] = "There is no Pick For You Enjoy :-)";
                return objmain.DataTableToJsonWithJsonNet(dt);
            }
            return objmain.DataTableToJsonWithJsonNet(dt);
        }
        public JsonResult Pick_Info([FromBody] complete_pickno info_rec)
        {
            Main objmain = new Main();
            return objmain.specific_pickDetails(info_rec.pickno);

        }
        public JsonResult For_Complete_pick([FromBody]complete_pickno complete)    // close this pick by this api 
        {
            Main objmain = new Main();
            string result = objmain.Pick_Completed_Try(complete);

            DataSet ds = new DataSet();
            ds = objmain.Get_Pick_Detail(Convert.ToInt32(complete.location), complete.email);
            DataColumn pending_pick = new DataColumn();
            pending_pick.ColumnName = "pending_pick";
            pending_pick.DefaultValue = ds.Tables[1].Rows[0][0].ToString();
            ds.Tables[0].Columns.Add(pending_pick);

            XmlDocument xmltest = new XmlDocument();
            xmltest.LoadXml(result);
            XmlNodeList error = xmltest.GetElementsByTagName("faultstring");

            if (error.Count > 0)
            {
                return objmain.Add_Condition(ds.Tables[0], "Error", error[0].InnerXml);
            }
            XmlNodeList success = xmltest.GetElementsByTagName("return_value");
            if (success.Count > 0)
            {
                int resultcomplete = objmain.Pick_Increase_Qty_Procedure(complete, "Complete");
                if (resultcomplete > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("condition", typeof(System.String));
                    dt.Columns.Add("Message", typeof(System.String));
                    dt.Rows.Add();
                    dt.Rows[0]["condition"] = "True";
                    dt.Rows[0]["Message"] = "Your Pick is Done";
                    return objmain.DataTableToJsonWithJsonNet(dt);
                }
                return objmain.Add_Condition(ds.Tables[0], "Error", "Pick Not Completed Due To Some DataBase Problem");
            }
            return new JsonResult("API bust");
        }
    }
}
