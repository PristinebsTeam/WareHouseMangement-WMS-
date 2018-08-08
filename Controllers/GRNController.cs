using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PristinefulfilApiPackage.App_Data;
using PristinefulfilApiPackage.Model.Grn;

namespace PristinefulfilApiPackage.Controllers
{
    [Produces("application/json")]
    [Route("api/GRN/[Action]")]
    public class GRNController : Controller
    {
        public string Post_Grn([FromBody]GRN Grn)         //for navision to insert new pick in database
        {
            Main objmain = new Main();
            string navresult = "Unsucces GRN No." + Grn.header.GRN_BoxNo;
            DataSet result = objmain.Insert_Grn_Header(Grn.header);
            try
            {
                if (result.Tables[0].Rows.Count> 0)
                {
                    if (result.Tables[0].Rows[0]["condition"].ToString().Equals("True"))
                    {
                        DataTable dt = new DataTable();
                        dt = ToDataTable(Grn.lines);
                        if (dt.Rows.Count > 0)
                        {
                            using (SqlConnection con = objmain.GetConnection())   // insert line in bulk in pick line table
                            {
                                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                                {
                                    //Set the database table name
                                    sqlBulkCopy.DestinationTableName = "Grn_Line";
                                    sqlBulkCopy.WriteToServer(dt);
                                    con.Close();
                                }
                            }
                            navresult = "Succes GRN No." + Grn.header.GRN_BoxNo;
                        }
                    }
                    else if (result.Tables[0].Rows[0]["condition"].ToString().Equals("False")){
                        navresult = "Unsucces GRNboxNO :" + Grn.header.GRN_BoxNo +" Due to "+ result.Tables[0].Rows[0]["Message"];
                    }
                }
                else
                {
                    navresult = "Unsucces Grn" + Grn.header.GRN_BoxNo;
                }
            }
            catch (Exception ee)
            {
                navresult = "Exception GRN No." + Grn.header.GRN_BoxNo;
            }
            return navresult;
        }
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
        }
        public JsonResult ForInsertBulkApiHit(ScanBoxId boxid) {
            Main objmain = new Main();
            DataSet ds = new DataSet();
            DataTable result = new DataTable();
            DataColumn condition = new DataColumn();
            DataColumn Message = new DataColumn();
            condition.DataType = System.Type.GetType("System.String");
            Message.DataType = System.Type.GetType("System.String");
            condition.Caption = "condition";
            Message.Caption = "Message";
            condition.ColumnName = "condition";
            Message.ColumnName = "Message";
            if (ds.Tables[0].Rows.Count>0) {
                string apiresult = ds.Tables[1].Rows[0][0].ToString();
                XmlDocument xmltest = new XmlDocument();
                xmltest.LoadXml(apiresult);
                XmlNodeList error = xmltest.GetElementsByTagName("faultstring");
                if (error.Count > 0)
                {
                    condition.DefaultValue = "False";
                    Message.DefaultValue = error[0].InnerXml;
                }
                XmlNodeList success = xmltest.GetElementsByTagName("return_value");
                if (success.Count > 0) {
                    condition.DefaultValue = "True";
                    Message.DefaultValue = success[0].InnerXml;
                }
                result.Columns.Add(condition);
                result.Columns.Add(Message);
                Message.SetOrdinal(0);
                condition.SetOrdinal(0);
            }
            return objmain.DataTableToJsonWithJsonNet(result);
        }

        public JsonResult Get_GRN_Details([FromBody] GRNdetails gRNdetails)
        {
            Main objmain = new Main();
            DataSet ds = objmain.getgrndetails(gRNdetails);
            return objmain.DataTableToJsonWithJsonNet(ds.Tables[0]);
        }


    }
 
}

