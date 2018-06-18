using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace ProfitPlus.Helpers
{
    public class DBClass
    {
        public static SqlConnection mycon()
        {
            String Constr = ConfigurationManager.ConnectionStrings["Remotestr"].ConnectionString;
            SqlConnection cn = new SqlConnection();
            try
            {
                cn.ConnectionString = Constr;
                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }
                return cn;
            }
            catch (Exception ex)
            {
                GenHelper g = new GenHelper();
                g.logerror(ex);
                return cn;
            }
        }

        public static Boolean NonQuery(String Qry)
        {
            Boolean Executed = false;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = mycon();
                cmd.CommandText = Qry;
                int numrow = cmd.ExecuteNonQuery();
                if (numrow > 0) Executed = true;
                cmd.Connection.Close();
                return Executed;
            }
            catch (Exception ex)
            {
                GenHelper g = new GenHelper();
                g.logerror(ex);
                return Executed;
            }
        }

        public static DataTable GetAllRecords(String qry)
        {
            DataTable ds = new DataTable();
            try
            {
                SqlDataAdapter da;
                SqlConnection cn = mycon();
                da = new SqlDataAdapter(qry, cn);
                da.Fill(ds);
                cn.Close();
                return ds;
            }
            catch (Exception ex) {
                GenHelper g = new GenHelper();
                g.logerror(ex);
                return ds;
            }

        }

        public static DataTable GetData(String qry)
        {
            SqlCommand  cm;
            SqlConnection cn = mycon();
            DataTable dt = new DataTable();
            cm = new SqlCommand (qry, cn);
            dt.Load(cm.ExecuteReader());
            cn.Close();
            return dt;
        }

    }

}