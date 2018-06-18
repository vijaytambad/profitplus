using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ProfitPlus.Helpers;

namespace ProfitPlus.Models
{
    public class User_permissions
    {
					public int id{get;set;}
			public int usertypeid{get;set;}
			public int menuid{get;set;}

		
		public DataTable GetOrderedRecords(int CurPage, int PerPage, String OrderBy) {
                int start = (CurPage-1) * PerPage +1;
                int endrec = start + PerPage - 1;
                String Qry = "SELECT A.id,u.usertypename,m.menuname FROM ( SELECT row_number() OVER (ORDER BY "+OrderBy+") AS rownum, *   FROM   User_permissions ) AS A inner join user_types as u on A.usertypeid=u.usertypeid inner join user_menus as m on A.menuid=m.menuid";
                    Qry+= " WHERE A.rownum BETWEEN "+start.ToString()+" AND "+endrec.ToString();
                    return DBClass.GetAllRecords(Qry);
            }

            public Int32 GetOrderedRecordsCount(String OrderBy)
            {
                
                String Qry = "SELECT * FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   User_permissions ) AS A";
                return DBClass.GetAllRecords(Qry).Rows.Count ;
            }
			
		public Boolean InsertUser_permissions(FormCollection f) {
            GenHelper g = new GenHelper();
            String Sqltext = "insert into User_permissions (id,usertypeid,menuid) values(@id,@usertypeid,@menuid)";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
			cmd.Parameters.AddWithValue("@id", g.GetNextId("User_permissions","id"));
            cmd.Parameters.AddWithValue("@usertypeid", f["usertypeid"]);
cmd.Parameters.AddWithValue("@menuid", f["menuid"]);

            int ab=cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true;else return false ;
        }

        public Boolean UpdateUser_permissions(FormCollection f)
        {
            String Sqltext = "update  User_permissions set id =@id,usertypeid =@usertypeid,menuid =@menuid where id=@id";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@id", f["id"]);

cmd.Parameters.AddWithValue("@usertypeid", f["usertypeid"]);
cmd.Parameters.AddWithValue("@menuid", f["menuid"]);

            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }

        public Boolean DeleteRecord(int id) {
            String Sqltext = "delete from User_permissions where id=@id";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@id", id);
            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }
    }
}