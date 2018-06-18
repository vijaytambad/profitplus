using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ProfitPlus.Helpers;

namespace ProfitPlus.Models
{
    public class Users
    {
					public int id{get;set;}
			public String username{get;set;}
			public String password{get;set;}
			public int sectionid{get;set;}
			public int divid{get;set;}
			public String mobileno{get;set;}
			public int usertypeid{get;set;}

		
		public DataTable GetOrderedRecords(int CurPage, int PerPage, String OrderBy) {
                int start = (CurPage-1) * PerPage +1;
                int endrec = start + PerPage - 1;
                String Qry = "SELECT A.id,A.username,A.password,s.sectionname,d.divname,u.usertypename FROM ( SELECT row_number() OVER (ORDER BY "+OrderBy+") AS rownum, *   FROM   Users ) AS A inner join section_master as s on A.sectionid=s.sectionid inner join div_master as d on d.divid=A.divid inner join user_types as u on A.usertypeid=u.usertypeid";
                    Qry+= " WHERE A.rownum BETWEEN "+start.ToString()+" AND "+endrec.ToString();
                    return DBClass.GetAllRecords(Qry);
            }

            public Int32 GetOrderedRecordsCount(String OrderBy)
            {
                
                String Qry = "SELECT * FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   Users ) AS A";
                return DBClass.GetAllRecords(Qry).Rows.Count ;
            }
			
		public Boolean InsertUsers(FormCollection f) {
            GenHelper g = new GenHelper();
            String Sqltext = "insert into Users (id,username,password,sectionid,divid,mobileno,usertypeid) values(@id,@username,@password,@sectionid,@divid,@mobileno,@usertypeid)";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
			cmd.Parameters.AddWithValue("@id", g.GetNextId("Users","id"));
            cmd.Parameters.AddWithValue("@username", f["username"]);
            cmd.Parameters.AddWithValue("@password", f["password"]);
            cmd.Parameters.AddWithValue("@sectionid", f["sectionid"]);
            cmd.Parameters.AddWithValue("@divid", f["divid"]);
            cmd.Parameters.AddWithValue("@mobileno", f["mobileno"]);
            cmd.Parameters.AddWithValue("@usertypeid", f["usertypeid"]);

            int ab=cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true;else return false ;
        }

        public Boolean UpdateUsers(FormCollection f)
        {
            String Sqltext = "update  Users set id =@id,username =@username,password =@password,sectionid =@sectionid,divid =@divid,mobileno =@mobileno,usertypeid =@usertypeid where id=@id";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@id", f["id"]);
            cmd.Parameters.AddWithValue("@username", f["username"]);
            cmd.Parameters.AddWithValue("@password", f["password"]);
            cmd.Parameters.AddWithValue("@sectionid", f["sectionid"]);
            cmd.Parameters.AddWithValue("@divid", f["divid"]);
            cmd.Parameters.AddWithValue("@mobileno", f["mobileno"]);
            cmd.Parameters.AddWithValue("@usertypeid", f["usertypeid"]);

            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }

        public Boolean DeleteRecord(int id) {
            String Sqltext = "delete from Users where id=@id";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@id", id);
            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }
        public DataTable GetUserRecord(Int32 userid)
        {
            DataTable dt = DBClass.GetAllRecords("select * from users where id=" + userid);
            return dt;
        
        }
    }
}