using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ProfitPlus.Helpers;

namespace ProfitPlus.Models
{
    public class User_types
    {
					public int usertypeid{get;set;}
			public String usertypename{get;set;}

		
		public DataTable GetOrderedRecords(int CurPage, int PerPage, String OrderBy) {
                int start = (CurPage-1) * PerPage +1;
                int endrec = start + PerPage - 1;
                String Qry = "SELECT * FROM ( SELECT row_number() OVER (ORDER BY "+OrderBy+") AS rownum, *   FROM   User_types ) AS A";
                    Qry+= " WHERE A.rownum BETWEEN "+start.ToString()+" AND "+endrec.ToString();
                    return DBClass.GetAllRecords(Qry);
            }

            public Int32 GetOrderedRecordsCount(String OrderBy)
            {
                
                String Qry = "SELECT * FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   User_types ) AS A";
                return DBClass.GetAllRecords(Qry).Rows.Count ;
            }
			
		public Boolean InsertUser_types(FormCollection f) {
            GenHelper g = new GenHelper();
            String Sqltext = "insert into User_types (usertypeid,usertypename) values(@usertypeid,@usertypename)";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
			cmd.Parameters.AddWithValue("@usertypeid", g.GetNextId("User_types","usertypeid"));
            cmd.Parameters.AddWithValue("@usertypename", f["usertypename"]);

            int ab=cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true;else return false ;
        }

        public Boolean UpdateUser_types(FormCollection f)
        {
            String Sqltext = "update  User_types set usertypeid =@usertypeid,usertypename =@usertypename where usertypeid=@usertypeid";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@usertypeid", f["usertypeid"]);

            cmd.Parameters.AddWithValue("@usertypename", f["usertypename"]);

            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }

        public Boolean DeleteRecord(int id) {
            String Sqltext = "delete from User_types where usertypeid=@id";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@id", id);
            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }

        public Boolean SavePer(String PerStr,int groupid) {
            GenHelper g = new GenHelper();
            String[] lst = PerStr.Substring(0,PerStr.Length-1).Split('|');
            String Qry; String Qry1;
            if (lst.Length > 0) {
                Qry1 = "delete from user_permissions where usertypeid="+groupid.ToString();
                DBClass.NonQuery(Qry1);
                for (int i = 0; i < lst.Length; i++)
                {
                    Qry = "insert into User_permissions(id,usertypeid,menuid) values(" + g.GetNextId("user_permissions", "id").ToString() + "," + groupid.ToString() + "," + lst[i] + ")";
                    if(!DBClass.NonQuery(Qry))return false;
                }
                return true;
            }
            return false;
        }
        public Boolean IsAvailable(String usertypename)
        {
            DataTable dt = DBClass.GetAllRecords("select usertypename from user_types where usertypename='" + usertypename + "'");
            if (dt.Rows.Count > 0) return false; else return true;

        }
    }
}