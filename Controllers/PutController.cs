using System;
using System.Data;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using PristinefulfilApiPackage.App_Data;
using PristinefulfilApiPackage.Model;

namespace PristinefulfilApiPackage.Controllers
{
    [Produces("application/json")]
    [Route("api/[Controller]/[Action]")]
    public class PutController : Controller
    {
        [HttpPost]
        public JsonResult Insert_Item_Entry_line([FromBody]InsertBarcode barcode)
        {
            Main objmain = new Main();
            string scanbarcode = barcode.scanbarcode12;
            string documentno = barcode.documentno;
            string fromshelf = barcode.documentfromshelf;
            string TOshelf = barcode.toshefinputbyuser;
            string createdby = barcode.email;
            string locationofitem = barcode.Location;
            string result = objmain.Insert_Item_Entry_Lines(documentno, locationofitem, fromshelf, TOshelf, scanbarcode, createdby, "null");

            if (result != "false")
            {
                return Shelf_Transfer_API_Procedure(documentno, locationofitem, fromshelf, TOshelf, scanbarcode, createdby, locationofitem);// NAV Line Entry Hit
            }
            else
            {
                DataSet result1 = new DataSet();
                result1 = objmain.Get_DocumentDetail(Convert.ToInt32(locationofitem), createdby);
                string flag = "Error";
                return objmain.Add_Condition(result1.Tables[0], flag, "Item is Not Found in our System");
            }

        }
        [HttpPost]
        public JsonResult Delete_Item_Entry_line_qty([FromBody]Editbarcode editbarqty)
        {
            Main objmain = new Main();
            DataSet result = new DataSet();
            string useremail = editbarqty.email;
            result = objmain.verifyinfo_ASR_Procedure_forconfermdelete(editbarqty.DocNo, editbarqty.barcode, "MAAR", editbarqty.toshelf, Convert.ToInt32(editbarqty.Qty));
            if (result.Tables[1].Rows.Count > 0)
            {

                string finalresult = result.Tables[1].Rows[0][0].ToString();
                XmlDocument xmltest = new XmlDocument();
                xmltest.LoadXml(finalresult);
                XmlNodeList error = xmltest.GetElementsByTagName("faultstring");
                if (error.Count > 0)
                {
                    string erroroutput = error[0].InnerXml;
                    result = objmain.Get_DocumentDetail(Convert.ToUInt16(editbarqty.Location), useremail);
                    string flag = "Error";
                    return objmain.Add_Condition(result.Tables[0], flag, erroroutput);
                }
                XmlNodeList success = xmltest.GetElementsByTagName("return_value");
                if (success.Count > 0)
                {
                    int verifyinfo_ASR = objmain.verifyinfo_ASR_Procedure(editbarqty.DocNo, editbarqty.barcode, "Delete_confirm", editbarqty.toshelf, Convert.ToInt32(editbarqty.Qty));
                    if (verifyinfo_ASR > 0)
                    {
                        result = objmain.Get_DocumentDetail(Convert.ToUInt16(editbarqty.Location), useremail);
                        string flag = "True";
                        return objmain.Add_Condition(result.Tables[0], flag, "Item Deleted");
                    }
                }
            }
            return new JsonResult("BAD");
        }
        [HttpPost]
        public JsonResult Get_DocumentDetail([FromBody]Location loc)
        {
            Main objmain = new Main();
            DataSet ds = new DataSet();
            ds = objmain.Get_DocumentDetail(Convert.ToInt32(loc.loc), loc.email);
            if (ds.Tables[0].Rows[0]["error"].ToString().Length > 2)
            {
                string flag = "Error";
                return objmain.Add_Condition(ds.Tables[0], flag, "Check for last error");
            }
            else
            {
                string flag = "True";
                return objmain.Add_Condition(ds.Tables[0], flag, "No Error");
            }
        }
        public JsonResult Shelf_Transfer_API_Procedure(string documentno, string locationofitem, string fromshelf, string TOshelf, string scanbarcode, string createdby ,string loca)  //([FromBody]GetdocumentNo docno)
        {
            Main objmain = new Main();
            string valid = null;
            string errorbynavision = null;
            DataSet result = new DataSet();
            result = objmain.Shelf_Transfer_API_Procedure(documentno);
            if (result.Tables[1].Rows.Count > 0)
            {
                string finalresult = result.Tables[1].Rows[0][0].ToString();
                XmlDocument xmltest = new XmlDocument();
                xmltest.LoadXml(finalresult);
                XmlNodeList error = xmltest.GetElementsByTagName("faultstring");
                if (error.Count > 0)
                {
                    valid = "false".ToUpper();
                    errorbynavision = error[0].InnerXml;
                    int verifyinfo_ASR = objmain.verifyinfo_ASR_Procedure(documentno, errorbynavision, "ERROR", "", 0);
                    if (verifyinfo_ASR > 0)
                    {
                        //for delete procedure line
                        int deleted = objmain.verifyinfo_ASR_Procedure(documentno, scanbarcode, "UPDS", TOshelf, 0);
                        result = objmain.Get_DocumentDetail(Convert.ToInt32(loca), createdby);
                        string flag = "Error";
                        return objmain.Add_Condition(result.Tables[0], flag, errorbynavision);
                    }
                }
                XmlNodeList success = xmltest.GetElementsByTagName("return_value");
                if (success.Count > 0)
                { 
                    result = objmain.Get_DocumentDetail(Convert.ToInt32(loca), createdby);
                    string flag = "True";
                    return objmain.Add_Condition(result.Tables[0], flag, "Command successfully Executed");
                }
            }
            return new JsonResult("BAD");
        }
        public JsonResult Next_Docno([FromBody]Put_complete docno)// Complete Clik
        {
            Main objmain = new Main();
            DataSet ds = new DataSet();
            string[] result = new string[2];
            string navisionxml = objmain.Complete_shelf(docno.docno);
            XmlDocument xmltest = new XmlDocument();
            xmltest.LoadXml(navisionxml);
            XmlNodeList error = xmltest.GetElementsByTagName("faultstring");
            if (error.Count > 0)
            {
                ds = objmain.Get_DocumentDetail(Convert.ToInt32(docno.location), docno.email);
                string flag = "Error";
                return objmain.Add_Condition(ds.Tables[0], flag, error[0].InnerXml);
            }
            XmlNodeList success = xmltest.GetElementsByTagName("return_value");
            if (success.Count > 0)
            {
                string successoutput = success[0].InnerXml;
                int verifyinfo_ASR = objmain.verifyinfo_ASR_Procedure(docno.docno, "", "HOGAYA", "", 0);
                if (verifyinfo_ASR > 0)
                {
                    ds = objmain.Get_DocumentDetail(Convert.ToInt32(docno.location), docno.email);
                    string flag = "True";
                    return objmain.Add_Condition(ds.Tables[0], flag, "Close Bin Successfuly");
                }
                else
                {
                    ds = objmain.Get_DocumentDetail(Convert.ToInt32(docno.location), docno.email);
                    string flag = "False";
                    return objmain.Add_Condition(ds.Tables[0], flag, "Get Some Error To Close Bin");
                }
            }
            return new JsonResult("BAD");
        }
        [HttpPost]
        public JsonResult Toshelf_Check_DB([FromBody]GetdocumentNo toshelf)
        {
            Main objmain = new Main();
            DataTable dt = new DataTable();
            dt = objmain.Toshelf_Check(toshelf.bincode);
            return objmain.DataTableToJsonWithJsonNet(dt);
        }
    }
}
