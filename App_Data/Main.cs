using System;
using System.Data;
using System.Data.SqlClient;
using PristinefulfilApiPackage.Model.Pick;
using PristinefulfilApiPackage.Model.Manifest;
using PristinefulfilApiPackage.Model;
using PristinefulfilApiPackage.Model.GateEntry;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using PristinefulfilApiPackage.Model.Grn;
using PristinefulfilApiPackage.Model.Physical;
using PristinefulfilApiPackage.Model.Slotpick;

namespace PristinefulfilApiPackage.App_Data
{
    public class Main : connection
    {
        //Convert DataTable to JsonResult//
        public JsonResult DataTableToJsonWithJsonNet(DataTable table)
        {
            JArray array = JArray.Parse(JsonConvert.SerializeObject(table));
            return new JsonResult(array);
        }
        public JsonResult Add_Condition(DataTable dt, string flag, string message)
        {
            Main objmain = new Main();
            DataColumn condition = new DataColumn();
            DataColumn Message = new DataColumn();
            condition.DataType = System.Type.GetType("System.String");
            Message.DataType = System.Type.GetType("System.String");
            condition.Caption = "condition";
            Message.Caption = "Message";
            condition.ColumnName = "condition";
            Message.ColumnName = "Message";
            if (flag == "Error")
            {
                condition.DefaultValue = "False";
                Message.DefaultValue = message;

            }
            else if (flag == "True")
            {
                condition.DefaultValue = "True";
                Message.DefaultValue = message;
            }

            dt.Columns.Add(condition);
            dt.Columns.Add(Message);
            Message.SetOrdinal(0);
            condition.SetOrdinal(0);
            return objmain.DataTableToJsonWithJsonNet(dt);
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

        //Bind Navbardata
        public DataSet Get_Nav_BarDynamic_Data(string flag, string email, int parent_id)
        {
            DataSet dt = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("Role_Permision", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@flag", flag);
                objcommand.Parameters.AddWithValue("@email", email);
                objcommand.Parameters.AddWithValue("@page_permission_id", parent_id);
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }



        //check the login detail
        public DataSet user_access_layer(string email, string password, string flag)
        {
            DataSet dt = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("LoginaccessLayer", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@Email_id", email);
                objcommand.Parameters.AddWithValue("@Password", password);
                objcommand.Parameters.AddWithValue("@flag", flag);
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        //for get document Detail 
        public DataSet Get_DocumentDetail(int loc, string userid)
        {
            DataSet ds = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("shelftransfer", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@loc", loc);
                objcommand.Parameters.AddWithValue("@createdby", userid);

                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }


        internal DataSet NewuserRequest(SignUP sign)
        {
            SqlConnection con = GetConnection();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("signup_new", con);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@username", sign.name);
                da.SelectCommand.Parameters.AddWithValue("@email", sign.email);
                da.SelectCommand.Parameters.AddWithValue("@password", sign.password);
                da.SelectCommand.Parameters.AddWithValue("@location", sign.location);
                da.SelectCommand.Parameters.AddWithValue("@role", sign.role);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            finally
            {
                CloseConnection(ref con);
            }
        }
   
        //insert scaned item by user
        public string Insert_Item_Entry_Lines(string docno, string loc, string from, string TOshelf, string barcd, string createdby, string qty)
        {
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("barcodescanASR", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@docno", docno);
                objcommand.Parameters.AddWithValue("@loc", loc);
                objcommand.Parameters.AddWithValue("@from", from);
                objcommand.Parameters.AddWithValue("@TOshelf", TOshelf);
                objcommand.Parameters.AddWithValue("@barcd", barcd);
                objcommand.Parameters.AddWithValue("@createdby", createdby);
                objcommand.Parameters.AddWithValue("@qty", qty);
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(ds);
                objcommand.Dispose();
                return ds.Tables[0].Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                return "false";
                throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        //get result from navision
        public DataSet Shelf_Transfer_API_Procedure(string doc_no)
        {
            DataSet dt = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("Shelf_Transfer_API_Procedure_260418", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@orderno", doc_no);
                objcommand.Parameters.AddWithValue("@result", "");
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        //check request it is delete or not
        public int verifyinfo_ASR_Procedure(string doc_no, string itemcode, string flag, string toshelf, int qty)
        {
            DataSet dt = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("verifyinfo_ASR", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@doc_no", doc_no);
                objcommand.Parameters.AddWithValue("@barcode", itemcode);
                objcommand.Parameters.AddWithValue("@flag", flag);
                objcommand.Parameters.AddWithValue("@toshelf", toshelf);
                objcommand.Parameters.AddWithValue("@qttybyuser", qty);
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(dt);
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
                throw ex;

            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        public string Complete_shelf(string doc_no)
        {
            DataSet dt = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("Shelf_Transfer_API_Procedure_done", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@docno", doc_no);
                objcommand.Parameters.AddWithValue("@result", "");
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(dt);
                return dt.Tables[1].Rows[0][0].ToString();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        //get api result for delete
        public DataSet verifyinfo_ASR_Procedure_forconfermdelete(string doc_no, string itemcode, string flag, string toshelf, int qty)
        {
            DataSet ds = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("verifyinfo_ASR", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@doc_no", doc_no);
                objcommand.Parameters.AddWithValue("@barcode", itemcode);
                objcommand.Parameters.AddWithValue("@flag", flag);
                objcommand.Parameters.AddWithValue("@toshelf", toshelf);
                objcommand.Parameters.AddWithValue("@qttybyuser", qty);
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        public DataTable Toshelf_Check(string toshelf)
        {
            DataTable dt = new DataTable();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("shelf", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@shelf", toshelf);
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                return null;
                throw ex;

            }
            finally
            {
                CloseConnection(ref cn);
            }
        }

        //get data for user display
        public DataSet Get_Pick_Detail(int loc, string userid)
        {
            DataSet ds = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("Pick_Detail", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@loc", loc);
                objcommand.Parameters.AddWithValue("@AssignedTo", userid);

                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(ds);
                return ds;
            }
            catch (Exception)
            {
                return ds;
                //  throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        public int Cancel_Pick_Order(string orderno)
        {
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("update Pick_Line set Status='CANCELED' where order_no='" + orderno + "'", cn);
                int result = objcommand.ExecuteNonQuery();
                return result;
            }
            catch (Exception)
            {
                return 0;
                //  throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        public DataTable check_Scan_Bin_Detail(Scanbin shelf)
        {
            DataSet ds = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("select * from Pick_Line where Shelf_Code='" + shelf.scanbincode + "'", cn);
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }

        internal JsonResult specific_pickDetails(string pickno)
        {
            SqlConnection con = GetConnection();
            DataTable dt = new DataTable();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("select h.Pick_no as pickno,(select loc_name from Location_master where auto_id=h.Location) as locationcode,h.AssignedTo as assgineto,substring(h.Created_On,0,11) as date,substring(h.Created_On,11,19) as time,l.Pick_Line_No as pick_lineNo,l.Action_Type as activity_type,l.Source_no as sourceno,l.Source_Line_No as source_lineno,l.source_document as source_document,l.Item_no as itemno,l.Item_desc as description,l.Original_qty as qty,l.Shelf_Code as bincode,l.order_id as order_id,l.Qty_To_Pick as Qty_To_Pick,l.order_no,l.Pick_Line_No as Pick_Line_No,l.barcode,l.Status from Pick_header h inner join Pick_Line l on h.Pick_no=l.Pick_Header_No where  Done = 0 and l.Status!='CANCELED' and h.Pick_no = '"+pickno+ "' order by l.Status asc,l.Shelf_Code asc ", con);
                da.Fill(dt);
                return DataTableToJsonWithJsonNet(dt);
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref con);
            }
        }

        public DataTable check_Scan_Barcode_Detail(Scanbarcode barcode)
        {
            DataSet ds = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("select * from Pick_Line where barcode='" + barcode.barcodeno + "' and Shelf_Code='" + barcode.bicode + "'", cn);
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        //for scaning api hit 
        public DataTable pick_Transfer_API_Procedure(Scanbarcode barcodeDetail)
        {
            DataSet dt = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("pick_Transfer_API_Procedure", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@Pick_Header_No", barcodeDetail.pickno);
                objcommand.Parameters.AddWithValue("@shelf", barcodeDetail.bicode);
                objcommand.Parameters.AddWithValue("@item", barcodeDetail.barcodeno);
                objcommand.Parameters.AddWithValue("@pick_line_no", barcodeDetail.Pick_Line_No);
                objcommand.Parameters.AddWithValue("@result", "");
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(dt);
                return dt.Tables[1];
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        public int Pick_Increase_Qty_Procedure(Scanbarcode barcodeDetail, string flag)
        {
            DataSet dt = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("Pick_Increase_Qty", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@pick_header_no", barcodeDetail.pickno);
                objcommand.Parameters.AddWithValue("@shelf", barcodeDetail.bicode);
                objcommand.Parameters.AddWithValue("@barcode", barcodeDetail.barcodeno);
                objcommand.Parameters.AddWithValue("@pick_line_no", barcodeDetail.Pick_Line_No);
                objcommand.Parameters.AddWithValue("@flag", flag);
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                int result = objcommand.ExecuteNonQuery();
                return result;
            }
            catch (Exception)
            {
                return 0;
                //throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        public int Pick_Increase_Qty_Procedure(complete_pickno pickn, string flag)   //for complete done after navision give response true
        {
            DataSet dt = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("Pick_Increase_Qty", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@pick_header_no", pickn.pickno);
                objcommand.Parameters.AddWithValue("@shelf", "");
                objcommand.Parameters.AddWithValue("@barcode", "");
                objcommand.Parameters.AddWithValue("@pick_line_no", 0);
                objcommand.Parameters.AddWithValue("@flag", flag);
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                int result = objcommand.ExecuteNonQuery();
                return result;
            }
            catch (Exception)
            {
                return 0;
                //throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        //end pick work
        //insert pick line for notfoundcase
        public int Insert_Pick_Line(pickZivline line)
        {
            DataSet dt = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("insert into Pick_Line(Warehouse_docno,warehouse_line,Pick_Header_No,Pick_Line_No,Action_Type,Source_no,Source_Line_No,source_document,Location,Item_no,Item_desc,Original_qty,Shelf_Code,order_no,barcode,sales_Line_parent,Assignto,sub_pick) values('" + line.Warehouse_docno + "'," + line.warehouse_line + ",'" + line.SubpickNo + "','" + line.pick_lineNo + "','" + line.activity_type + "','" + line.sourceno + "','" + line.source_lineno + "','" + line.source_document + "','" + line.locationcode + "','" + line.itemno + "','" + line.description + "','" + line.qty + "','" + line.bincode + "','" + line.order_no + "','" + line.barcode + "','" + Convert.ToInt32(line.sales_Line_parent) + "',  '" + line.assgineto + "', '"+ line.pickno+"')", cn);
                int result = objcommand.ExecuteNonQuery();
                return result;
            }
            catch (Exception)
            {
                return 0;
                //throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        //not found case by pick
        public string Pick_Not_Found_Api_Hit(NotFoundCaseData Notfound)
        {
            DataSet dt = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("pick_Transfer_API_Procedure_notFound", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@Pick_Header_No", Notfound.pickno);
                objcommand.Parameters.AddWithValue("@shelf", Notfound.scanbin);
                objcommand.Parameters.AddWithValue("@item", Notfound.barcode);
                objcommand.Parameters.AddWithValue("@pick_line_no", Notfound.Pick_Line_No);
                objcommand.Parameters.AddWithValue("@result", "");
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(dt);
                string result = dt.Tables[1].Rows[0][0].ToString();
                return result;
            }
            catch (Exception)
            {
                return "0";
                //throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }

        internal DataTable allpending_pick(string assignPerson , string location)
        {
            SqlConnection con = GetConnection();
            DataTable dt = new DataTable();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("with TAble1 as(select distinct Pick_Line.Pick_Header_No, sum(Pick_Line.Original_qty) as Orignal_qty , sum(Pick_Line.Qty_To_Pick) as Pick_qty, Pick_header.Created_On from Pick_Line inner join Pick_header on Pick_header.Pick_no = Pick_Line.Pick_Header_No where Pick_header.AssignedTo = '"+ assignPerson+"' and Pick_header.Location = '"+ location+ "' and Pick_header.Done = 0  group by Pick_Line.Pick_Header_No , Pick_header.Created_On) select * from TAble1 where Orignal_qty != 0 ", con);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref con);
            }
        }
        public string Pick_Completed_Try(complete_pickno complete)
        {
            DataSet dt = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("pick_Transfer_API_Procedure_complete", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@Pick_Header_Num", complete.pickno);
                objcommand.Parameters.AddWithValue("@result", "");
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(dt);
                string result = dt.Tables[1].Rows[0][0].ToString();
                return result;
            }
            catch (Exception)
            {
                return "false";
                //throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        public int Not_Found_Pash_ToManage_Qty(NotFoundCaseData Notfound, string flag)
        {
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("Pick_Increase_Qty", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@flag", flag);
                objcommand.Parameters.AddWithValue("@pick_header_no", Notfound.pickno);
                objcommand.Parameters.AddWithValue("@shelf", Notfound.scanbin);
                objcommand.Parameters.AddWithValue("@barcode", Notfound.barcode);
                objcommand.Parameters.AddWithValue("@pick_line_no", Notfound.Pick_Line_No);
                int result = objcommand.ExecuteNonQuery();
                return result;
            }
            catch (Exception)
            {
                return 0;
                //throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        //Anoop
        public int Insert_item(Iteminfo iteminfo)
        {
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("new_item", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@item_code", iteminfo.item_code);
                objcommand.Parameters.AddWithValue("@descrip", iteminfo.descrip);
                objcommand.Parameters.AddWithValue("@barcode", iteminfo.barcode);
                return objcommand.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return 0;
                //throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }

        }
        public int Insert_bin(Bininfo bininfo)
        {
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("new_bin", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@shelf_id", bininfo.shelf_id);
                return objcommand.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return 0;
                //throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }

        }
        //Arjun
        public DataTable NoTfoundclick(NotFoundCaseData Notfound)
        {
            DataSet dt = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("NoTfoundclick", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@pick_header_no ", Notfound.pickno);
                objcommand.Parameters.AddWithValue("@shelf", Notfound.scanbin);
                objcommand.Parameters.AddWithValue("@barcode", Notfound.barcode);
                objcommand.Parameters.AddWithValue("@pick_line_no", Notfound.Pick_Line_No);
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(dt);
                return dt.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }

        }
        //menifest work start
        public DataSet manifest_header_insert(string location, string createdby, string dsp_code)
        {
            DataSet ds = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("manifest_header_insert", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@loc", location);
                objcommand.Parameters.AddWithValue("@createdby", createdby);
                objcommand.Parameters.AddWithValue("@dsp_code", dsp_code);
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(ds);
                return ds;
            }
            catch (Exception)
            {
                ds = null;
                return ds;
                //throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }

        }
        public int manifest_flag_work(Manifest manidata, string flag)
        {
            DataSet ds = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("manifest_flag_work", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@loc", manidata.locatin);
                objcommand.Parameters.AddWithValue("@createdby", manidata.createdby);
                objcommand.Parameters.AddWithValue("@dsp_code", manidata.dsp_code);
                objcommand.Parameters.AddWithValue("@flag", flag);
                int result = objcommand.ExecuteNonQuery();
                return result;
            }
            catch (Exception)
            {
                return 0;
                //throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }

        }
        //Insert Manifest Line
        public int manifest_Line_insert(Manifest_Line manidata)
        {
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                string query = "insert into manifest_lines(manifest_no,line_id,createdon,dsp_partner_code,customer_id,cust_name,cust_add,cust_city,cust_state,cust_pincode,cust_phoneno,awb_no,weight,country,order_no,dsp_partner_name,second_courier_dispatch,posted_invoice_no,pay_method,cancelled_order,totalamount)" +
                               "values('" + manidata.manifest_no + "','" + manidata.line_id + "',Convert(nvarchar,GETDATE(),121),'" + manidata.dsp_partner_code + "','" + manidata.customer_id + "','" + manidata.cust_name + "','" + manidata.cust_add + "','" + manidata.cust_city + "','" + manidata.cust_state + "','" + manidata.cust_pincode + "','" + manidata.cust_phoneno + "','" + manidata.awb_no + "'," + manidata.weight + ",'" + manidata.country + "','" + manidata.order_no + "','" + manidata.dsp_partner_name + "','" + manidata.second_courier_dispatch + "','" + manidata.posted_invoice_no + "','" + manidata.pay_method + "','" + manidata.cancelled_order + "'," + manidata.totalamount + ")";
                objcommand = new SqlCommand(query, cn);
                return objcommand.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return 0;
                //throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }

        }
        public int Delete_Manifest_Header_by_Manifestno(Manifest_Line line, string flag)
        {
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("manifest_flag_work", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@mainfestno", line.manifest_no);
                objcommand.Parameters.AddWithValue("@flag", flag);
                int result = objcommand.ExecuteNonQuery();
                return result;
            }
            catch (Exception)
            {
                return 0;
                //throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }

        }
        public string Scan_Awb_No_ByUser(Manifest_Line line)
        {
            DataSet ds = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("manifest_API_scan", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@manifest_no", line.manifest_no);
                objcommand.Parameters.AddWithValue("@awb_no", line.awb_no);
                objcommand.Parameters.AddWithValue("@result", "");
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(ds);
                return ds.Tables[1].Rows[0][0].ToString();
            }
            catch (Exception)
            {
                return "";
                //throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }

        }
        public string manifest_API_delete(Manifest_Line line)
        {
            DataSet ds = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("manifest_API_delete", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@manifest_no", line.manifest_no);
                objcommand.Parameters.AddWithValue("@maniline_no", line.line_id);
                objcommand.Parameters.AddWithValue("@result", "");
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(ds);
                return ds.Tables[1].Rows[0][0].ToString();
            }
            catch (Exception)
            {
                return "";
                //throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }

        }
        //get respose by navision delete the line
        public int Delete_Manifest_line_Manifestno(Manifest_Line line, string flag)
        {
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("manifest_flag_work", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@mainfestno", line.manifest_no);
                objcommand.Parameters.AddWithValue("@manifestline_no", line.line_id);
                objcommand.Parameters.AddWithValue("@flag", flag);
                int result = objcommand.ExecuteNonQuery();
                return result;
            }
            catch (Exception)
            {
                return 0;
                //throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }

        }
        //complete manifest
        public string manifest_API_Completed(Manifest_Line line)
        {
            DataSet ds = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("manifest_API_complete", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@manifest_no", line.manifest_no);
                objcommand.Parameters.AddWithValue("@result", "");
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(ds);
                return ds.Tables[1].Rows[0][0].ToString();
            }
            catch (Exception)
            {
                return "";
                //throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }

        }
        //take respose to navavision to delete data
        public string manifest_API_header_delete(Manifest_Line line)
        {
            DataSet ds = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("manifest_API_header_delete", cn);
                objcommand.CommandType = CommandType.StoredProcedure;
                objcommand.Parameters.AddWithValue("@manifest_no", line.manifest_no);
                objcommand.Parameters.AddWithValue("@result", "");
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(ds);
                return ds.Tables[1].Rows[0][0].ToString();
            }
            catch (Exception)
            {
                return "";
                //throw ex;
            }
            finally
            {
                CloseConnection(ref cn);
            }

        }
        public DataTable Get_Dropdown_List_Values(Manifest mani)
        {
            DataSet ds = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("select manifest_odata,Manifest_username,manifest_password from Location_master where Auto_id=" + mani.locatin + "", cn);
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref cn);
            }

        }
        // Sorting Started//
        internal DataSet pickDropDownList()
        {
            SqlConnection con = GetConnection();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("select distinct Pick_no from Pick_header ph inner join Pick_Line pl on ph.Pick_no = pl.Pick_Header_No where ph.Done = 1 and pl.order_block = 0", con);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            finally
            {
                CloseConnection(ref con);
            }
        }
        public DataSet Sorting_header(string assign, string email/*, string pickno*/)
        {
            DataSet ds = new DataSet();
            SqlConnection con = GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("Sorting_header_create", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@assign_person", assign);
                cmd.Parameters.AddWithValue("@email", email);
                //cmd.Parameters.AddWithValue("@pick_no", pickno);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                return ds;
            }
            catch (Exception)
            {
                return ds;
            }
            finally
            {
                CloseConnection(ref con);
            }
        }
        public void nav_slot_sync(string slot_number)
        {
            SqlConnection con = GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("update slot set order_no = 0 where slot_id = " + slot_number + "", con);
                cmd.ExecuteNonQuery();
                CloseConnection(ref con);
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref con);
            }
        }
        internal DataTable SortingComplete(string orderno, string slotbarcode)
        {
            SqlConnection con = GetConnection();
            try
            {
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter("sorting_Complete", con);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@order_no", orderno);
                da.SelectCommand.Parameters.AddWithValue("@sortbarcode", slotbarcode); 
                da.Fill(ds);

                return ds.Tables[0];
            }
            catch (Exception ee)
            {
                throw ee;
            }

            finally
            {
                CloseConnection(ref con);
            }
        }
        internal DataSet regular_sorting_item_scan(string item_barcode, string assign_person, string email/*, string pickno*/)
        {
            SqlConnection con = GetConnection();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("sorting_new_amand", con);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@item_code", item_barcode);
                da.SelectCommand.Parameters.AddWithValue("@assign_per", assign_person);
                da.SelectCommand.Parameters.AddWithValue("@email", email);
                // da.SelectCommand.Parameters.AddWithValue("@pickHeader_no", pickno);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception ee)
            {
                throw ee;
            }

            finally
            {
                CloseConnection(ref con);
            }
        }
        internal DataSet sorting_Flag_work(object orderNO, object itemNO, object slotNO, object assige_person, string location, /*string pickno, */string flag)
        {
            SqlConnection con = GetConnection();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("sorting_flag", con);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@order_no", orderNO);
                da.SelectCommand.Parameters.AddWithValue("@slot", slotNO);
                da.SelectCommand.Parameters.AddWithValue("@item_number", itemNO);
                da.SelectCommand.Parameters.AddWithValue("@assign_per", assige_person);
                da.SelectCommand.Parameters.AddWithValue("@loc", location);
                //da.SelectCommand.Parameters.AddWithValue("@pick_num", pickno);
                da.SelectCommand.Parameters.AddWithValue("@flag", flag);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception ee)
            {
                throw ee;
            }

            finally
            {
                CloseConnection(ref con);
            }
        }
        internal DataTable get_item_info(string orderNO)
        {
            DataSet ds = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("select item_barcode,item_desc,sum(Original_qty) as Original_QTY,sum(qty) as Scanned_qty from sorting_line where order_no=" + orderNO + " group by item_barcode,item_desc ", cn);
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        //Sorting End //
        //gateentry Start//
        internal DataSet newGateEntryHeader(gateEntryfirst entryfirst)
        {
            SqlConnection con = GetConnection();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("gateentry_new", con);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@location", Convert.ToInt16(entryfirst.location.ToString()));
                da.SelectCommand.Parameters.AddWithValue("@createdby", entryfirst.maker);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception ee)
            {
                throw ee;
            }

            finally
            {
                CloseConnection(ref con);
            }
        }
        internal DataTable vendor_nav(gateVendorAPI vendorAPI)
        {
            DataSet ds = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("select gateEntry_Vendor_Api_url,gateEntry_username,gateEntry_password from Location_master where Auto_id=" + Convert.ToInt32(vendorAPI.location) + "", cn);
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        internal DataTable location_nav(gateVendorAPI aPI)
        {
            DataSet ds = new DataSet();
            SqlConnection cn = GetConnection();
            try
            {
                SqlCommand objcommand;
                objcommand = new SqlCommand("select getEntry_Location_API_URL,gateEntry_username,gateEntry_password from Location_master where Auto_id=" + Convert.ToInt32(aPI.location) + "", cn);
                SqlDataAdapter da = new SqlDataAdapter(objcommand);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        internal DataSet NavSendData(string doctype, string gateEntry, string vendor_Num, string location, string createdby)
        {
            SqlConnection cn = GetConnection();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("GateEntry_API_Vendor_send", cn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@doctype", doctype);
                da.SelectCommand.Parameters.AddWithValue("@gateEntry_no", gateEntry);
                da.SelectCommand.Parameters.AddWithValue("@vendor_no", vendor_Num);
                da.SelectCommand.Parameters.AddWithValue("@location_name", location);
                da.SelectCommand.Parameters.AddWithValue("@created_by", createdby);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        internal JsonResult getRoleinfo()
        {
            DataSet ds = new DataSet();
            SqlConnection con = GetConnection();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT role_id ,role_name FROM Role_mst ", con);
                da.Fill(ds);
                return DataTableToJsonWithJsonNet(ds.Tables[0]);
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref con);
            }
        }
        internal JsonResult allgateEntery()
        {
            SqlConnection con = GetConnection();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("select * from GateEntry where VehicleNo = '' order by id desc", con);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return DataTableToJsonWithJsonNet(ds.Tables[0]);
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref con);
            }
        }
        internal JsonResult specific_gateentry(string gateEntrynumber)
        {
            SqlConnection con = GetConnection();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("select * from GateEntry where GateEntryNo = '" + gateEntrynumber + "'", con);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return DataTableToJsonWithJsonNet(ds.Tables[0]);
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref con);
            }
        }
        internal void delete_previousdata(string assignto)
        {
            SqlConnection con = GetConnection();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("delete from gateEntry_DocNum where Assign_to = '"+ assignto + "'; delete from gateEntry_tranfer_order where assign_to = '" + assignto + "';", con);
                DataSet ds = new DataSet();
                da.Fill(ds);
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref con);
            }
        }
        internal JsonResult get_TO_DOCnum_UI(string assginto, string dropdownvalue, string type)
        {
            SqlConnection con = GetConnection();
            try
            {
                DataTable dt = new DataTable();
                if (type == "Transfer Order")
                {
                    SqlDataAdapter da = new SqlDataAdapter("select Transfer_order from gateEntry_tranfer_order where assign_to = '" + assginto + "'", con);
                    da.Fill(dt);                  
                }
                else if (type == "Purchase Order")
                {
                    SqlDataAdapter da = new SqlDataAdapter("select Doc_Number from gateEntry_DocNum where assign_to = '"+assginto+"'", con);
                    da.Fill(dt);                  
                }
                return DataTableToJsonWithJsonNet(dt);
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref con);
            }

        }
        internal DataSet GateEntryUpdateinfo(GateEntryValue gateEntryValue)
        {
            SqlConnection con = GetConnection();
            try
            {
                
                SqlDataAdapter da = new SqlDataAdapter("GateEntryInfo_update", con);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@gateEntry_number", gateEntryValue.gateentry_number);
                da.SelectCommand.Parameters.AddWithValue("@freight", gateEntryValue.Freight);
                da.SelectCommand.Parameters.AddWithValue("@freight_amount", gateEntryValue.Freight_Amount);
                da.SelectCommand.Parameters.AddWithValue("@Doctype", gateEntryValue.DocType);
                da.SelectCommand.Parameters.AddWithValue("@purchase_number", gateEntryValue.Purchase_number);
                da.SelectCommand.Parameters.AddWithValue("@transfer_num", gateEntryValue.Transfer_number);
                da.SelectCommand.Parameters.AddWithValue("@trasnfer_location", gateEntryValue.Transfer_location);
                da.SelectCommand.Parameters.AddWithValue("@vendor_no", gateEntryValue.VendorNo);
                da.SelectCommand.Parameters.AddWithValue("@vehicleNo", gateEntryValue.VehicleNo);
                da.SelectCommand.Parameters.AddWithValue("@assignPerson", gateEntryValue.assignPerson);
                da.SelectCommand.Parameters.AddWithValue("@no_of_box", gateEntryValue.No_of_Boxes);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref con);
            }
        }
        

        //Physical Counting Start

        internal DataSet getphysical(string email, string location)
        {
            SqlConnection cn = GetConnection();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("Physical_header", cn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@loc", Convert.ToInt32(location));
                da.SelectCommand.Parameters.AddWithValue("@createdby", email);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        internal void deletePhysicalHeader(string email, string location)
        {
            SqlConnection con = GetConnection();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("delete from physical_counting_header where CreatedBy = '" + email + "' and Location = '" + location + "' and Done = 0", con);
                DataSet ds = new DataSet();
                da.Fill(ds);
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref con);
            }
        }

        internal DataSet Insert_Grn_Header(GRN_Header grnheaderValue)
        {
            SqlConnection con = GetConnection();
            try
            {

                SqlDataAdapter da = new SqlDataAdapter("GRN_header_insert", con);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@GRN_BoxNo", grnheaderValue.GRN_BoxNo);
                da.SelectCommand.Parameters.AddWithValue("@Gate_Entry_No", grnheaderValue.Gate_Entry_No);
                da.SelectCommand.Parameters.AddWithValue("@Create_DateTime", grnheaderValue.Create_DateTime);
                da.SelectCommand.Parameters.AddWithValue("@Vendor_Code", grnheaderValue.Vendor_Code);
                da.SelectCommand.Parameters.AddWithValue("@Location_code", grnheaderValue.Location_code);
                da.SelectCommand.Parameters.AddWithValue("@Doc_No", grnheaderValue.Doc_No);
                da.SelectCommand.Parameters.AddWithValue("@TotalBox", grnheaderValue.TotalBox);
                da.SelectCommand.Parameters.AddWithValue("@status", grnheaderValue.status);
                da.SelectCommand.Parameters.AddWithValue("@Vendor_BoxId", grnheaderValue.Vendor_BoxId);
                da.SelectCommand.Parameters.AddWithValue("@EntryBy", grnheaderValue.EntryBy);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref con);
            }
        }
        internal DataSet Scan_Box_id(ScanBoxId scanboxid)
        {
            SqlConnection con = GetConnection();
            try
            {

                SqlDataAdapter da = new SqlDataAdapter("GRN_Box_ID_API_Send", con);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@box_id", scanboxid.box_id);
                da.SelectCommand.Parameters.AddWithValue("@createdby", scanboxid.createdby);
                da.SelectCommand.Parameters.AddWithValue("@location", scanboxid.location);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref con);
            }
        }

        internal DataSet physical_ItemScan(string physcialHeaderNum, string bincode, string itemcode,string loc,string createdby)
        {
            SqlConnection cn = GetConnection();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("physical_item_scan", cn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@physical_header_num", physcialHeaderNum);
                da.SelectCommand.Parameters.AddWithValue("@loc", loc);
                da.SelectCommand.Parameters.AddWithValue("@TOshelf", bincode);
                da.SelectCommand.Parameters.AddWithValue("@barcd", itemcode);
                da.SelectCommand.Parameters.AddWithValue("@createdby", createdby);

                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        internal DataSet physical_counting_flag(string flag, string physical_header,string shelf, string itemcode, int qty_available, string createdby)
        {
            SqlConnection cn = GetConnection();
            try
            {
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter("physical_counting_flag", cn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@flag", flag);
                da.SelectCommand.Parameters.AddWithValue("@physical_counting_header", physical_header);
                da.SelectCommand.Parameters.AddWithValue("@shelf", shelf);
                da.SelectCommand.Parameters.AddWithValue("@itembarcode", itemcode);
                da.SelectCommand.Parameters.AddWithValue("@qty_available", qty_available);
                da.SelectCommand.Parameters.AddWithValue("@createdby", createdby);
                da.Fill(ds);
                return ds;
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        internal DataSet itemcheck(string itemcode)
        {
            DataSet ds = new DataSet();
            SqlConnection con = GetConnection();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("select * from item_mst with(Index(IX_Barcode)) where barcode='" + itemcode+"'", con);
                da.Fill(ds);
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref con);
            }
            return ds;
        }
        internal DataSet physical_complete(Complete complete)
        {
            SqlConnection cn = GetConnection();
            try
            {
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter("Physical_API_complete", cn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@phy_counting_header", complete.physcalHeaderNum);
                da.Fill(ds);
                return ds;
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        internal int physical_counting_flag_complete(string flag, string physcalHeaderNum)
        {
            SqlConnection cn = GetConnection();
            try
            {
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter("physical_counting_flag", cn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@flag", flag);
                da.SelectCommand.Parameters.AddWithValue("@physical_counting_header", physcalHeaderNum);
                return da.SelectCommand.ExecuteNonQuery();
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }

        //GRN_Process

        internal DataSet getgrndetails(GRNdetails gRNdetails)
        {
            SqlConnection cn = GetConnection();
            try
            {
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter("GRN_Details", cn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@grn_boxno", gRNdetails.GrnBoxid);
                da.Fill(ds);
                return ds;
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }


        internal int androidVersion()
        {
            SqlConnection con = GetConnection();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("select top 1 latest_version from Android_version order by latest_version desc", con);
                return Convert.ToInt32(da.SelectCommand.ExecuteScalar());
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref con);
            }
        }

        internal JsonResult activeuser()
        {
            SqlConnection con = GetConnection();
            DataTable dt = new DataTable();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("select Email_id from user_mst where active = 1", con);
                da.Fill(dt);
                return DataTableToJsonWithJsonNet(dt);
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref con);
            }
        }

        internal JsonResult getslotData(slotPicker slotpick)
        {
            SqlConnection cn = GetConnection();
            try
            {
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter("slotpick", cn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@assignto", slotpick.email);
                da.SelectCommand.Parameters.AddWithValue("@location", slotpick.location);
                da.Fill(ds);
                return DataTableToJsonWithJsonNet(ds.Tables[0]);
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }

        internal JsonResult relese_slot(slotNO slot)
        {
            SqlConnection cn = GetConnection();
            try
            {
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter("slotpick_Release", cn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@slotnum", slot.slotnumber);
                da.Fill(ds);
                return DataTableToJsonWithJsonNet(ds.Tables[0]);
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
        internal DataSet Send_NAV_Slot_Complete(slotNO slotNO)
        {
            SqlConnection cn = GetConnection();
            try
            {
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter("Slotpick_API_Complete", cn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@slotnum", slotNO.slotnumber);
                da.SelectCommand.Parameters.AddWithValue("@location", slotNO.location);
                da.Fill(ds);
                return ds;
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }

        internal DataTable slotPick_complete(slotNO slotNO)
        {
            SqlConnection cn = GetConnection();
            try
            {
                DataTable ds = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter("slotpick_complete", cn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@slotnum", slotNO.slotnumber);
                da.Fill(ds);
                return ds;
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                CloseConnection(ref cn);
            }
        }
    }
    public class MyWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = (WebRequest)base.GetWebRequest(address);

            if (request is HttpWebRequest)
            {
                var myWebRequest = request as HttpWebRequest;
                myWebRequest.UnsafeAuthenticatedConnectionSharing = true;
                myWebRequest.KeepAlive = true;
            }

            return request;
        }
    }
}