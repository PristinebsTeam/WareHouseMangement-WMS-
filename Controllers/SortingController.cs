using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using PristinefulfilApiPackage.Model.Sorting;
using PristinefulfilApiPackage.App_Data;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PristinefulfilApiPackage.Controllers
{
    [Produces("application/json")]
    [Route("api/[Controller]/[action]")]
    public class SortingController : Controller
    {
        [HttpGet]
        public JsonResult pickDropodown()
        {
            Main objmain = new Main();
            DataSet ds = new DataSet();
            ds = objmain.pickDropDownList();
            return objmain.DataTableToJsonWithJsonNet(ds.Tables[0]);
        }

        [HttpPost]
        public JsonResult Header([FromBody] Sorting sort)
        {
            Main objmain = new Main();
            DataSet ds = new DataSet();
            ds = objmain.Sorting_header(sort.assign_person, sort.email/*, sort.pick_no*/);
            return objmain.DataTableToJsonWithJsonNet(ds.Tables[0]);
        }

        [HttpPost]
        public void Slot_Sync_NAV([FromBody] Sort NAVSort)
        {
            Main objmain = new Main();
            objmain.nav_slot_sync(NAVSort.clear_slot);
        }

        [HttpPost]
        public JsonResult Itemscan([FromBody] scanitem SItem)
        {
            Main objmain = new Main();
            DataSet ds = new DataSet();
            try
            {
                //if (SItem.order_number.ToString() == "") // to check that it is a first item scan or rest
                //{
                ds = objmain.regular_sorting_item_scan(SItem.item_barcode, SItem.assign_person, SItem.email/*,SItem.pickno*/);
                if (ds.Tables[3].Rows[0][0].ToString() == "New")
                {
                    //ds = objmain.sorting_item_scan(SItem.item_barcode, SItem.assign_person);// NAV Communication for slot alloction//
                    try
                    {
                        if (ds.Tables[1].Rows.Count > 0)
                        {
                            string apiresult = ds.Tables[1].Rows[0][0].ToString();
                            XmlDocument xmltest = new XmlDocument();
                            xmltest.LoadXml(apiresult);
                            XmlNodeList error = xmltest.GetElementsByTagName("faultstring");// No slot available or another problem incounter
                            if (error.Count > 0)
                            {
                                string erroroutput = error[0].InnerXml;
                                ds.Tables[1].Columns.Add("condition", typeof(System.String));
                                ds.Tables[1].Columns.Add("Message", typeof(System.String));
                                ds.Tables[1].Rows[0]["condition"] = "False";
                                ds.Tables[1].Rows[0]["Message"] = erroroutput;
                                return objmain.DataTableToJsonWithJsonNet(ds.Tables[1]);         // UI return if any NAV error occur                     
                            }
                            XmlNodeList success = xmltest.GetElementsByTagName("return_value");// Slot is allocated in NAV
                            if (success.Count > 0)
                            {
                                string successoutput = success[0].InnerXml;
                                ds = objmain.sorting_Flag_work(ds.Tables[2].Rows[0][0].ToString(), ds.Tables[2].Rows[0][3].ToString(), ds.Tables[2].Rows[0][1].ToString(), ds.Tables[2].Rows[0][4].ToString(), ds.Tables[2].Rows[0]["Location"].ToString()/*, ds.Tables[2].Rows[0]["Pick_num"].ToString()*/, "API_RETURN_TURE");    //increase his sorting qty     
                                if (ds.Tables[0].Rows[0][0].ToString() == "This order is already listed under sorting") // When there is order in sorting 
                                    return objmain.DataTableToJsonWithJsonNet(ds.Tables[0]);
                                else
                                    return objmain.DataTableToJsonWithJsonNet(ds.Tables[0]);
                            }
                            return new JsonResult("BAD");
                        }
                        else
                        {
                            var documentdetail = "BAD";
                            return new JsonResult(documentdetail);
                        }
                    }
                    catch (Exception ee)
                    {
                        if (ds.Tables[2].Rows.Count > 0)
                            return objmain.DataTableToJsonWithJsonNet(ds.Tables[2]);
                        else
                            return new JsonResult(ee.Message);
                    }
                }
                else if (ds.Tables[3].Rows[0][0].ToString() == "Exists")
                {
                    return objmain.DataTableToJsonWithJsonNet(ds.Tables[0]);
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
        public JsonResult SortingComplete([FromBody] ScanCom scanCom)
        {
            Main objmain = new Main();
            DataTable dt = new DataTable();
            dt = objmain.SortingComplete(scanCom.order_no.ToString(), scanCom.SlotbarCode);
            return objmain.DataTableToJsonWithJsonNet(dt);
        }

        [HttpPost]
        public JsonResult itemdetails_by_order([FromBody] ScanCom oder)
        {
            Main objmain = new Main();
            DataTable dt = new DataTable();
            dt = objmain.get_item_info(oder.order_no.ToString());
            return objmain.DataTableToJsonWithJsonNet(dt);
        }
    }
}