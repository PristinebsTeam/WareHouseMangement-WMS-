using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PristinefulfilApiPackage.Model.Slotpick;
using PristinefulfilApiPackage.App_Data;
using System.Data;
using System.Xml;

namespace PristinefulfilApiPackage.Controllers
{
    [Produces("application/json")]
    [Route("api/Slotpick/[Action]")]
    public class SlotpickController : Controller
    {
        [HttpPost]
        public JsonResult getslot([FromBody] slotPicker slotpick)
        {
            try
            {
                Main objmain = new Main();
                return objmain.getslotData(slotpick);
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }
        [HttpPost]
        public JsonResult Relese_slot_without_complete([FromBody] slotNO slot)
        {
            try
            {
                Main objmain = new Main();
                return objmain.relese_slot(slot);
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }
        [HttpPost]

        public JsonResult Complete_slot_pick([FromBody] slotNO slotNO)
        {
            try
            {
                Main objmain = new Main();
                DataSet ds = objmain.Send_NAV_Slot_Complete(slotNO);
                if (ds.Tables[1].Rows.Count > 0)
                {
                    string apiresult = ds.Tables[1].Rows[0][0].ToString();
                    XmlDocument xmltest = new XmlDocument();
                    xmltest.LoadXml(apiresult);
                    XmlNodeList error = xmltest.GetElementsByTagName("faultstring");
                    if (error.Count > 0)
                    {
                        return objmain.Add_Condition(ds.Tables[0], "Error", error[0].InnerXml);
                    }
                    XmlNodeList success = xmltest.GetElementsByTagName("return_value");
                    if (success.Count > 0)
                    {
                        DataTable dt = objmain.slotPick_complete(slotNO);
                        return objmain.DataTableToJsonWithJsonNet(dt);
                    }
                }
                return new JsonResult("API BUSTED");
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }
    }
}