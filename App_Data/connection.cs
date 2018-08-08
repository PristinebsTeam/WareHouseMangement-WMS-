using System;
using System.Data;
using System.Data.SqlClient;

namespace PristinefulfilApiPackage.App_Data
{
    public class connection
    {
        public string GetDataString()
        {
            //String connection = "Data Source=localhost;Initial Catalog=Zivame_Dev;Integrated Security=False;User Id=sa;Password=dg@#DEew3;";
            String connection = "Data Source=LOCAL-SERVER;Initial Catalog=Zivame_Dev;Integrated Security=False;User Id=sa;Password=local@123;";
            return connection;
        }
        public SqlConnection GetConnection()
        {
            SqlConnection cn = new SqlConnection(GetDataString());
            if (cn.State == ConnectionState.Closed)
                cn.Open();
            return cn;
        }
        public void CloseConnection(ref SqlConnection cn)
        {
            cn.Close();
            cn.Dispose();
        }
    }
}
