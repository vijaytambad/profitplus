using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ProfitPlus.Helpers;

namespace ProfitPlus.Models
{
    public class Account_parent
    {
					public int parentid{get;set;}
			public String parentname{get;set;}

		
		public DataTable GetOrderedRecords(int CurPage, int PerPage, String OrderBy) {
                int start = (CurPage-1) * PerPage +1;
                int endrec = start + PerPage - 1;
                String Qry = "SELECT A.parentid,A.parentname FROM ( SELECT row_number() OVER (ORDER BY "+OrderBy+") AS rownum, *   FROM   Account_parent ) AS A";
                    Qry+= " WHERE A.rownum BETWEEN "+start.ToString()+" AND "+endrec.ToString();
                    return DBClass.GetAllRecords(Qry);
            }

            public Int32 GetOrderedRecordsCount(String OrderBy)
            {
                
                String Qry = "SELECT * FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   Account_parent ) AS A";
                return DBClass.GetAllRecords(Qry).Rows.Count ;
            }
			
		public Boolean InsertAccount_parent(FormCollection f) {
            GenHelper g = new GenHelper();
            String Sqltext = "insert into Account_parent (parentid,parentname) values(@parentid,@parentname)";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
			cmd.Parameters.AddWithValue("@parentid", g.GetNextId("Account_parent","parentid"));
            cmd.Parameters.AddWithValue("@parentname", f["parentname"]);

            int ab=cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true;else return false ;
        }

        public Boolean UpdateAccount_parent(FormCollection f)
        {
            String Sqltext = "update  Account_parent set parentid =@parentid,parentname =@parentname where parentid=@parentid";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@parentid", f["parentid"]);

cmd.Parameters.AddWithValue("@parentname", f["parentname"]);

            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }

        public Boolean DeleteRecord(int id) {
            String Sqltext = "delete from Account_parent where parentid=@id";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@id", id);
            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }
        public Boolean IsAvailable(String parentname)
        {
            DataTable dt = DBClass.GetAllRecords("select parentname from account_parent where parentname='" + parentname + "'");
            if (dt.Rows.Count > 0) return false; else return true;

        }
    }
}