using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ProfitPlus.Helpers;

namespace ProfitPlus.Models
{
    public class User_menus
    {
					public int menuid{get;set;}
			public String menuname{get;set;}
			public String menuurl{get;set;}
			public int parentid{get;set;}

		
		public DataTable GetOrderedRecords(int CurPage, int PerPage, String OrderBy) {
                int start = (CurPage-1) * PerPage +1;
                int endrec = start + PerPage - 1;
                String Qry = "SELECT A.menuid,A.menuname,A.menuurl FROM ( SELECT row_number() OVER (ORDER BY "+OrderBy+") AS rownum, *   FROM   User_menus ) AS A";
                    Qry+= " WHERE A.rownum BETWEEN "+start.ToString()+" AND "+endrec.ToString();
                    return DBClass.GetAllRecords(Qry);
            }

            public Int32 GetOrderedRecordsCount(String OrderBy)
            {
                
                String Qry = "SELECT * FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   User_menus ) AS A";
                return DBClass.GetAllRecords(Qry).Rows.Count ;
            }
			
		public Boolean InsertUser_menus(FormCollection f) {
            GenHelper g = new GenHelper();
            String Sqltext = "insert into User_menus (menuid,menuname,menuurl,parentid) values(@menuid,@menuname,@menuurl,@parentid)";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
			cmd.Parameters.AddWithValue("@menuid", g.GetNextId("User_menus","menuid"));
            cmd.Parameters.AddWithValue("@menuname", f["menuname"]);
cmd.Parameters.AddWithValue("@menuurl", f["menuurl"]);
cmd.Parameters.AddWithValue("@parentid", f["parentid"]);

            int ab=cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true;else return false ;
        }

        public Boolean UpdateUser_menus(FormCollection f)
        {
            String Sqltext = "update  User_menus set menuid =@menuid,menuname =@menuname,menuurl =@menuurl,parentid =@parentid where menuid=@menuid";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@menuid", f["menuid"]);

cmd.Parameters.AddWithValue("@menuname", f["menuname"]);
cmd.Parameters.AddWithValue("@menuurl", f["menuurl"]);
cmd.Parameters.AddWithValue("@parentid", f["parentid"]);

            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }

        public Boolean DeleteRecord(int id) {
            String Sqltext = "delete from User_menus where menuid=@id";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@id", id);
            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }
        public Boolean IsAvailable(String menuname)
        {
            DataTable dt = DBClass.GetAllRecords("select menuname from user_menus where menuname='" + menuname + "'");
            if (dt.Rows.Count > 0) return false; else return true;

        }
    }
}