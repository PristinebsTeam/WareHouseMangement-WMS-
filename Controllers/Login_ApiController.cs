using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PristinefulfilApiPackage.App_Data;
using PristinefulfilApiPackage.Model;


namespace PristinefulfilApiPackage.Controllers
{
    [Produces("application/json")]
    [Route("api/[Controller]/[action]")]

    public class Login_ApiController : Controller
    {
        [HttpGet]
        public string Grantaccess()
        {
            return "Apiconnection is Done";
        }
        [HttpPost]
        public JsonResult Get_Login_Access([FromBody]Login_Android login)
        {
            Main objmain = new Main();
            DataSet result = new DataSet();
            string[] apiresult = new string[2];
            string email = login.email.ToString();
            string password = login.password.ToString();
            result = objmain.user_access_layer(email, password, "login");

            if (result.Tables[0].Rows.Count > 0)
            {
                result.Tables[0].Columns.Add("condition", typeof(System.String));
                result.Tables[0].Columns.Add("Message", typeof(System.String));

                int active = Convert.ToInt32(result.Tables[0].Rows[0]["active"].ToString());
                if (active == 0)
                {
                    result.Tables[0].Rows[0]["condition"] = "True";
                    result.Tables[0].Rows[0]["Message"] = result.Tables[0].Rows[0]["Name"].ToString();
                    return objmain.DataTableToJsonWithJsonNet(result.Tables[0]);
                }
                else
                {
                    result.Tables[0].Rows[0]["condition"] = "False";
                    result.Tables[0].Rows[0]["Message"] = "Already Login !";
                    return objmain.DataTableToJsonWithJsonNet(result.Tables[0]);
                }
            }
            else
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("condition", typeof(System.String));
                dt.Columns.Add("Message", typeof(System.String));
                dt.Rows.Add();
                dt.Rows[0]["condition"] = "False";
                dt.Rows[0]["Message"] = "Unauthorized User !";
                return objmain.DataTableToJsonWithJsonNet(dt);
            }

            
        }
        [HttpPost]
        public JsonResult Get_Navbar_Data([FromBody]AndroidNavBardata navbar)
        {
            Main objmain = new Main();
            DataSet ds = new DataSet();
            ds = objmain.Get_Nav_BarDynamic_Data(navbar.flag, navbar.email, navbar.parent_id);
            return objmain.DataTableToJsonWithJsonNet(ds.Tables[0]);
        }

        [HttpPost]
        public JsonResult SignUp_user([FromBody] SignUP sign)
        {
            Main objmain = new Main();
            DataSet ds = new DataSet();
            ds = objmain.NewuserRequest(sign);
            return objmain.DataTableToJsonWithJsonNet(ds.Tables[0]);
        }

        [HttpGet]
        public JsonResult RoleDropDown()
        {
            Main objmain = new Main();
            return objmain.getRoleinfo();
        }
        public string Logout([FromBody] logout logout)
        {
            Main objmain = new Main();
            DataSet ds = new DataSet();
            try
            {              
                ds = objmain.user_access_layer(logout.emailaddress.ToString(), "", "logout");
                if (ds.Tables[0].Rows.Count > 0) {
                    return "logout";
                }
            }
            catch (Exception)
            {
            }
            return "logout";
        }
        [HttpGet]
        public int android_version_check()
        {
            Main objmain = new Main();
            int version = objmain.androidVersion();
            return version;
        }
        [HttpGet]
        public JsonResult list_active_user()
        {
            Main objmain = new Main();
            return objmain.activeuser();
        }
    }
}