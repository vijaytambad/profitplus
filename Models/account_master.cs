using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ProfitPlus.Helpers;

namespace ProfitPlus.Models
{
    public class Account_master
    {
					public int accountid{get;set;}
			public String accountnumber{get;set;}
			public String accountname{get;set;}
			public int parentid{get;set;}
			public int groupid{get;set;}
			public int sectionid{get;set;}
			public int allow{get;set;}
			public String active{get;set;}

		
		public DataTable GetOrderedRecords(int CurPage, int PerPage, String OrderBy) {
                int start = (CurPage-1) * PerPage +1;
                int endrec = start + PerPage - 1;
                String Qry = "SELECT A.accountid,A.accountnumber,A.accountname,p.parentname,g.groupname,s.sectionname,A.allow FROM ( SELECT row_number() OVER (ORDER BY "+OrderBy+") AS rownum, *   FROM   Account_master ) AS A inner join account_parent as p on A.parentid=p.parentid inner join account_group as g on g.groupid=A.groupid inner join section_master as s on A.sectionid=s.sectionid";
                    Qry+= " WHERE A.rownum BETWEEN "+start.ToString()+" AND "+endrec.ToString();
                    return DBClass.GetAllRecords(Qry);
            }

            public Int32 GetOrderedRecordsCount(String OrderBy)
            {
                
                String Qry = "SELECT * FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   Account_master ) AS A";
                return DBClass.GetAllRecords(Qry).Rows.Count ;
            }
			
		public Boolean InsertAccount_master(FormCollection f) {
            GenHelper g = new GenHelper();
            String Sqltext = "insert into Account_master (accountid,accountnumber,accountname,parentid,groupid,sectionid,allow) values(@accountid,@accountnumber,@accountname,@parentid,@groupid,@sectionid,@allow)";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
			cmd.Parameters.AddWithValue("@accountid", g.GetNextId("Account_master","accountid"));
            cmd.Parameters.AddWithValue("@accountnumber", f["accountnumber"]);
            cmd.Parameters.AddWithValue("@accountname", f["accountname"]);
            cmd.Parameters.AddWithValue("@parentid", f["parentid"]);
            cmd.Parameters.AddWithValue("@groupid", f["groupid"]);
            cmd.Parameters.AddWithValue("@sectionid", f["sectionid"]);
            cmd.Parameters.AddWithValue("@allow", f["allow"]);
            //cmd.Parameters.AddWithValue("@active", f["active"]);

            int ab=cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true;else return false ;
        }

        public Boolean UpdateAccount_master(FormCollection f)
        {
            String Sqltext = "update  Account_master set accountid =@accountid,accountnumber =@accountnumber,accountname =@accountname,parentid =@parentid,groupid =@groupid,sectionid =@sectionid,allow =@allow,active =@active where accountid=@accountid";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@accountid", f["accountid"]);

cmd.Parameters.AddWithValue("@accountnumber", f["accountnumber"]);
cmd.Parameters.AddWithValue("@accountname", f["accountname"]);
cmd.Parameters.AddWithValue("@parentid", f["parentid"]);
cmd.Parameters.AddWithValue("@groupid", f["groupid"]);
cmd.Parameters.AddWithValue("@sectionid", f["sectionid"]);
cmd.Parameters.AddWithValue("@allow", f["allow"]);
cmd.Parameters.AddWithValue("@active", f["active"]);

            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }

        public Boolean DeleteRecord(int id) {
            String Sqltext = "delete from Account_master where accountid=@id";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@id", id);
            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }
        public Boolean IsAvailable(String accountnumber)
        {
            DataTable dt = DBClass.GetAllRecords("select accountnumber from account_master where accountnumber='" + accountnumber + "'");
            if (dt.Rows.Count > 0) return false; else return true;

        }

        
    }
}