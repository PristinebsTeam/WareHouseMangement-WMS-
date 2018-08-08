using System;
using System.Data;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using PristinefulfilApiPackage.App_Data;
using PristinefulfilApiPackage.Model.Physical;

namespace PristinefulfilApiPackage.Controllers
{
    [Produces("application/json")]
    [Route("api/Physical/[Action]")]
    public class PhysicalController : Controller
    {
        public JsonResult Get_PhysicalDetail([FromBody]FirstData data)
        {
            Main objmain = new Main();
            DataSet ds = objmain.getphysical(data.email, data.location);
            try
            {
                if (ds.Tables[1].Rows[0][0].ToString() == "200")
                {
                    return objmain.DataTableToJsonWithJsonNet(ds.Tables[0]);
                }
                else if (ds.Tables[1].Rows[0][0].ToString() == "500")
                {
                    objmain.deletePhysicalHeader(data.email, data.location);

                    string apiresult = ds.Tables[2].Rows[0][0].ToString();
                    XmlDocument xmltest = new XmlDocument();
                    xmltest.LoadXml(apiresult);
                    XmlNodeList error = xmltest.GetElementsByTagName("faultstring");
                    if (error.Count > 0)
                    {
                        string erroroutput = error[0].InnerXml;
                        ds.Tables[2].Columns.Add("condition", typeof(System.String));
                        ds.Tables[2].Columns.Add("Message", typeof(System.String));
                        ds.Tables[2].Rows[0]["condition"] = "False";
                        ds.Tables[2].Rows[0]["Message"] = erroroutput;
                        return objmain.DataTableToJsonWithJsonNet(ds.Tables[2]);
                    }
                }
                return new JsonResult("BAD");
            }
            catch (Exception)
            {
                return objmain.DataTableToJsonWithJsonNet(ds.Tables[0]);
            }
        }
        public JsonResult bincodeScan([FromBody] Binscan binscan)
        {
            Main objmain = new Main();
            DataTable dt = new DataTable();
            dt = objmain.Toshelf_Check(binscan.bincode);
            return objmain.DataTableToJsonWithJsonNet(dt);
        }
        public JsonResult ItemScan_Physical([FromBody] ItemCodeScan codeScan)
        {
            Main objmain = new Main();
            DataSet ds = new DataSet();
            ds = objmain.itemcheck(codeScan.itemcode);
            if (ds.Tables[0].Rows.Count > 0)
            {
                ds = objmain.physical_ItemScan(codeScan.physcialHeaderNum, codeScan.bincode, codeScan.itemcode, codeScan.loc, codeScan.createdby);
                try
                {
                    if (ds.Tables[2].Rows.Count > 0)
                    {
                        XmlDocument xmltest = new XmlDocument();
                        string apiresult = ds.Tables[2].Rows[0][0].ToString();
                        xmltest.LoadXml(ds.Tables[2].Rows[0][0].ToString());
                        XmlNodeList error = xmltest.GetElementsByTagName("faultstring");

                        if (error.Count > 0)
                        {
                            DataSet localds = new DataSet();
                            if (ds.Tables[3].Rows[0][0].ToString().Equals("Old"))
                            {
                                localds = objmain.physical_counting_flag("Error_Old", codeScan.physcialHeaderNum, codeScan.bincode, codeScan.itemcode,  0, codeScan.createdby);
                                return objmain.Add_Condition(localds.Tables[0], "Error", error[0].InnerXml);
                            }
                            else if (ds.Tables[3].Rows[0][0].ToString().Equals("New"))
                            {
                                localds = objmain.physical_counting_flag("Error_New", codeScan.physcialHeaderNum, codeScan.bincode, codeScan.itemcode,  0, codeScan.createdby);
                                if (localds.Tables[0].Rows.Count > 0)
                                    return objmain.Add_Condition(localds.Tables[0], "Error", error[0].InnerXml);
                                else
                                {
                                    DataSet dod = objmain.getphysical(codeScan.createdby, codeScan.loc);
                                   return objmain.Add_Condition(dod.Tables[0], "Error", error[0].InnerXml);
                                }
                            }
                            return objmain.Add_Condition(ds.Tables[0], "Error", error[0].InnerXml);
                        }
                        XmlNodeList success = xmltest.GetElementsByTagName("return_value");
                        if (success.Count > 0)
                        {
                            string[] xmlstringtrue = new string[5];
                            try
                            {
                                xmlstringtrue = success[0].InnerText.ToString().Split("....");
                                if (xmlstringtrue[1].ToString().Equals("TRUE"))
                                {
                                    DataSet localds = new DataSet();
                                    DataRow[] results = ds.Tables[0].Select("Physical_header_no= '" + codeScan.physcialHeaderNum + "'");
                                    int qtyavailabel = Convert.ToInt32(results[0]["QtyAvailable"].ToString());
                                    if (Convert.ToInt32(xmlstringtrue[3].ToString()) != qtyavailabel)
                                    {
                                        localds = objmain.physical_counting_flag("New_Qty", codeScan.physcialHeaderNum, codeScan.bincode, codeScan.itemcode,  Convert.ToInt32(xmlstringtrue[3].ToString()), codeScan.createdby);
                                        return objmain.Add_Condition(localds.Tables[0], "True", "Scan");
                                    }
                                    else if (ds.Tables[3].Rows[0][0].ToString().Equals("New"))
                                    {
                                        localds = objmain.physical_counting_flag("New", codeScan.physcialHeaderNum, codeScan.bincode, codeScan.itemcode,  Convert.ToInt32(xmlstringtrue[3].ToString()), codeScan.createdby);
                                        return objmain.Add_Condition(localds.Tables[0], "True", "Scan");
                                    }

                                }
                            }
                            catch (Exception ee)
                            {
                                throw ee;
                            }

                        }
                    }
                }
                catch (Exception ee)
                {
                    throw ee;
                }
            }
            return objmain.DataTableToJsonWithJsonNet(ds.Tables[0]);
        }
        public JsonResult Complete_click([FromBody] Complete complete)
        {
            Main objmain = new Main();
            DataSet ds = new DataSet();
            ds = objmain.physical_complete(complete);
            if (ds.Tables[1].Rows.Count > 0)
            {
                XmlDocument xmldata = new XmlDocument();
                xmldata.LoadXml(ds.Tables[1].Rows[0][0].ToString());
                XmlNodeList error = xmldata.GetElementsByTagName("faultstring");
                if (error.Count > 0)
                {
                    DataSet localds = objmain.getphysical(complete.assign_person, complete.location);
                    return objmain.Add_Condition(localds.Tables[0], "Error", error[0].InnerXml);
                }
                XmlNodeList success = xmldata.GetElementsByTagName("return_value");
                if (success.Count > 0)
                {
                    int i = objmain.physical_counting_flag_complete("CompletePass", complete.physcalHeaderNum);
                    if (i > 0)
                    {
                        return objmain.Add_Condition(ds.Tables[0], "True", "Call Header API Now");
                    }
                }
            }
            return new JsonResult("API or DataBase Procedure Busted");
        }
    }
}