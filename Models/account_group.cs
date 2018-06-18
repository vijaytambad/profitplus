using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ProfitPlus.Helpers;

namespace ProfitPlus.Models
{
    public class Account_group
    {
					public int groupid{get;set;}
			public String groupname{get;set;}
			public int parentid{get;set;}

		
		public DataTable GetOrderedRecords(int CurPage, int PerPage, String OrderBy) {
                int start = (CurPage-1) * PerPage +1;
                int endrec = start + PerPage - 1;
                String Qry = "SELECT A.groupid,A.groupname,A1.parentname FROM ( SELECT row_number() OVER (ORDER BY "+OrderBy+") AS rownum, *   FROM   Account_group ) AS A inner join account_parent as A1 on A.parentid=A1.parentid";
                    Qry+= " WHERE A.rownum BETWEEN "+start.ToString()+" AND "+endrec.ToString();
                    return DBClass.GetAllRecords(Qry);
            }

            public Int32 GetOrderedRecordsCount(String OrderBy)
            {
                
                String Qry = "SELECT * FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   Account_group ) AS A";
                return DBClass.GetAllRecords(Qry).Rows.Count ;
            }
			
		public Boolean InsertAccount_group(FormCollection f) {
            GenHelper g = new GenHelper();
            String Sqltext = "insert into Account_group (groupid,groupname,parentid) values(@groupid,@groupname,@parentid)";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
			cmd.Parameters.AddWithValue("@groupid", g.GetNextId("account_group","groupid"));
            cmd.Parameters.AddWithValue("@groupname", f["groupname"]);
cmd.Parameters.AddWithValue("@parentid", f["parentid"]);

            int ab=cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true;else return false ;
        }

        public Boolean UpdateAccount_group(FormCollection f)
        {
            String Sqltext = "update  Account_group set groupid =@groupid,groupname =@groupname,parentid =@parentid where groupid=@groupid";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@groupid", f["groupid"]);

cmd.Parameters.AddWithValue("@groupname", f["groupname"]);
cmd.Parameters.AddWithValue("@parentid", f["parentid"]);

            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }

        public Boolean DeleteRecord(int id) {
            String Sqltext = "delete from Account_group where groupid=@id";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@id", id);
            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }
        public Boolean IsAvailable(String groupname)
        {
            DataTable dt = DBClass.GetAllRecords("select groupname from account_group where groupname='" + groupname + "'");
            if (dt.Rows.Count > 0) return false; else return true;

        }
    }
}