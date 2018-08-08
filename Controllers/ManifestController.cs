using System;
using System.Data;
using System.Net;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PristinefulfilApiPackage.App_Data;
using PristinefulfilApiPackage.Model.Manifest;

namespace PristinefulfilApiPackage.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class ManifestController : Controller
    {
        [HttpPost]
        public JsonResult Get_Manifest_Detail([FromBody]Manifest manidata)           //bind pick data by this method
        {
            Main objmain = new Main();
            JObject details = new JObject();
            DataTable dt = new DataTable();
            dt = objmain.Get_Dropdown_List_Values(manidata);
            if (dt.Rows.Count > 0)
            {
                string[] user1 = dt.Rows[0]["Manifest_username"].ToString().Split("\\");
                CredentialCache myCache = new CredentialCache();
                myCache.Add(new Uri(dt.Rows[0]["manifest_odata"].ToString()), "NTLM", new NetworkCredential(user1[1], dt.Rows[0]["manifest_password"].ToString(), user1[0]));
                MyWebClient WebClient = new MyWebClient();
                WebClient.Credentials = myCache;
                string Result = WebClient.DownloadString(new Uri(dt.Rows[0]["manifest_odata"].ToString()));
                details = JObject.Parse(Result);
            }
            return new JsonResult(details);
        }
        public JsonResult Get_Manifest_Full_Detail(string location, string dsp_code, string createdby)           //bind  data by this method
        {
            Main objmain = new Main();
            DataSet ds = new DataSet();
            ds = objmain.manifest_header_insert(location, createdby, dsp_code);
            return objmain.DataTableToJsonWithJsonNet(ds.Tables[0]);
        }
        public JsonResult manifest_header_insert([FromBody] Manifest manidata)
        {
            Main objmain = new Main();
            string[] finalresult = new string[2];
            DataSet ds = new DataSet();
            ds = objmain.manifest_header_insert(manidata.locatin,manidata.createdby,manidata.dsp_code);
            try
            {
                if (ds.Tables[1].Rows.Count > 0)
                {
                    string navisionxml = ds.Tables[1].Rows[0][0].ToString();
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(navisionxml);
                    XmlNodeList error = xml.GetElementsByTagName("faultstring");
                    if (error.Count > 0)
                    {
                        DataTable dt = new DataTable();

                        dt.Columns.Add("condition", typeof(System.String));
                        dt.Columns.Add("Message", typeof(System.String));
                        dt.Rows.Add();
                        dt.Rows[0]["condition"] = "False";
                        dt.Rows[0]["Message"] = error[0].InnerXml;   
                        //call another function WrongHeader
                        int result = objmain.manifest_flag_work(manidata, "WrongHeader");
                        return objmain.DataTableToJsonWithJsonNet(dt);
                    }
                    XmlNodeList success = xml.GetElementsByTagName("return_value");
                    if (success.Count > 0)
                    {
                        return Get_Manifest_Full_Detail(manidata.locatin, manidata.dsp_code, manidata.createdby);
                    }
                }
            }
            catch (Exception)
            {
                return Get_Manifest_Full_Detail(manidata.locatin,manidata.dsp_code, manidata.createdby);
            }
            return new JsonResult(finalresult);
        }
        // insert line by navision
        public string Insert_Menifest_Line([FromBody]Manifest_Line manidata)
        {
            Main objmain = new Main();
            int result = objmain.manifest_Line_insert(manidata);
            if (result > 0)
            {
                return "Susssfull Insert";
            }
            else
            {
                return "UnSusssfull Insert";
            }

        }
        //bind data
        public JsonResult Delete_Manifest([FromBody]Manifest_Line line)
        {
            Main objmain = new Main();
            string[] returnarray = new string[2];
            string result = objmain.manifest_API_header_delete(line);
            XmlDocument Navisionxml = new XmlDocument();
            Navisionxml.LoadXml(result);
            XmlNodeList error = Navisionxml.GetElementsByTagName("faultstring");
            if (error.Count > 0)
            {
                returnarray[0] = "false";
                returnarray[1] = error[0].InnerXml;
            }
            XmlNodeList success = Navisionxml.GetElementsByTagName("return_value");
            if (success.Count > 0)
            {
                returnarray[0] = "true";
                returnarray[1] = "Done";
                int Deletedone = objmain.Delete_Manifest_Header_by_Manifestno(line, "DeleteByManifestNo");
            }
           
            return new JsonResult(returnarray);

        }
        //scan Awbno
        public JsonResult Scan_Awb_No_ByUser([FromBody]Manifest_Line line)
        {
            Main objmain = new Main();
            string[] result = new string[2];
            string NavXml = objmain.Scan_Awb_No_ByUser(line);
            XmlDocument apiXml = new XmlDocument();
            apiXml.LoadXml(NavXml);
            XmlNodeList error = apiXml.GetElementsByTagName("faultstring");
            if (error.Count > 0)
            {
                DataTable dt = new DataTable();

                dt.Columns.Add("condition", typeof(System.String));
                dt.Columns.Add("Message", typeof(System.String));
                dt.Rows.Add();
                dt.Rows[0]["condition"] = "False";
                dt.Rows[0]["Message"] = error[0].InnerXml;
                return objmain.DataTableToJsonWithJsonNet(dt);
            }
            XmlNodeList success = apiXml.GetElementsByTagName("return_value");
            if (success.Count > 0)
            {
              return  Get_Manifest_Full_Detail(line.locatin, line.dsp_partner_code, line.createdby);
            }
            return new JsonResult("BAD");
        }
        //delete Manifest Line   
        public JsonResult manifest_API_delete([FromBody]Manifest_Line line)
        {
            Main objmain = new Main();
            string[] returnarray = new string[2];
            string result = objmain.manifest_API_delete(line);
            XmlDocument Navisionxml = new XmlDocument();
            Navisionxml.LoadXml(result);
            XmlNodeList error = Navisionxml.GetElementsByTagName("faultstring");
            if (error.Count > 0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("condition", typeof(System.String));
                dt.Columns.Add("Message", typeof(System.String));
                dt.Rows.Add();
                dt.Rows[0]["condition"] = "False";
                dt.Rows[0]["Message"] = error[0].InnerXml;
                return objmain.DataTableToJsonWithJsonNet(dt);
            }
            XmlNodeList success = Navisionxml.GetElementsByTagName("return_value");
            if (success.Count > 0)
            {
                returnarray[0] = "true";
                returnarray[1] = "Done";
                int Deleledata = objmain.Delete_Manifest_line_Manifestno(line, "Manifestlinedelete");
                return Get_Manifest_Full_Detail(line.locatin, line.dsp_partner_code, line.createdby);
            }
            return new JsonResult("BAD");
        }
        public JsonResult manifest_API_Complete([FromBody]Manifest_Line line)
        {
            Main objmain = new Main();
            string[] returnarray = new string[2];
            string result = objmain.manifest_API_Completed(line);
            XmlDocument Navisionxml = new XmlDocument();
            Navisionxml.LoadXml(result);
            XmlNodeList error = Navisionxml.GetElementsByTagName("faultstring");
            if (error.Count > 0)
            {
                DataTable dt = new DataTable();

                dt.Columns.Add("condition", typeof(System.String));
                dt.Columns.Add("Message", typeof(System.String));
                dt.Rows.Add();
                dt.Rows[0]["condition"] = "False";
                dt.Rows[0]["Message"] = error[0].InnerXml;
                return objmain.DataTableToJsonWithJsonNet(dt);
            }
            XmlNodeList success = Navisionxml.GetElementsByTagName("return_value");
            if (success.Count > 0)
            {
                DataTable dt = new DataTable();

                dt.Columns.Add("condition", typeof(System.String));
                dt.Columns.Add("Message", typeof(System.String));
                dt.Rows.Add();
                dt.Rows[0]["condition"] = "True";
                dt.Rows[0]["Message"] = "Done";
                
                int Deleledata = objmain.Delete_Manifest_line_Manifestno(line, "Manifestcomplete");  //update heder done =1
                return objmain.DataTableToJsonWithJsonNet(dt);
            }
            return new JsonResult("BAD");
        }
    }
    //for read html response give by navision
}
