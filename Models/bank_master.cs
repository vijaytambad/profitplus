using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ProfitPlus.Helpers;

namespace ProfitPlus.Models
{
    public class Bank_master
    {
					public int bankid{get;set;}
			public String bankname{get;set;}
			public String accountnumber{get;set;}
			public String bankaccno{get;set;}
			public int chequestart{get;set;}
			public int chequeend{get;set;}
			public int divid{get;set;}
			public int nextchequeno{get;set;}

		
		public DataTable GetOrderedRecords(int CurPage, int PerPage, String OrderBy) {
                int start = (CurPage-1) * PerPage +1;
                int endrec = start + PerPage - 1;
                String Qry = "SELECT * FROM ( SELECT row_number() OVER (ORDER BY "+OrderBy+") AS rownum, *   FROM   Bank_master ) AS A";
                    Qry+= " WHERE A.rownum BETWEEN "+start.ToString()+" AND "+endrec.ToString();
                    return DBClass.GetAllRecords(Qry);
            }

            public Int32 GetOrderedRecordsCount(String OrderBy)
            {
                
                String Qry = "SELECT * FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   Bank_master ) AS A";
                return DBClass.GetAllRecords(Qry).Rows.Count ;
            }
			
		public Boolean InsertBank_master(FormCollection f) {
            GenHelper g = new GenHelper();
            String Sqltext = "insert into Bank_master (bankid,bankname,accountnumber,bankaccno,chequestart,chequeend,divid,nextchequeno) values(@bankid,@bankname,@accountnumber,@bankaccno,@chequestart,@chequeend,@divid,@nextchequeno)";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
			cmd.Parameters.AddWithValue("@bankid", g.GetNextId("Bank_master","bankid"));
            cmd.Parameters.AddWithValue("@bankname", f["bankname"]);
cmd.Parameters.AddWithValue("@accountnumber", f["accountnumber"]);
cmd.Parameters.AddWithValue("@bankaccno", f["bankaccno"]);
cmd.Parameters.AddWithValue("@chequestart", f["chequestart"]);
cmd.Parameters.AddWithValue("@chequeend", f["chequeend"]);
cmd.Parameters.AddWithValue("@divid", f["divid"]);
cmd.Parameters.AddWithValue("@nextchequeno", f["nextchequeno"]);

            int ab=cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true;else return false ;
        }

        public Boolean UpdateBank_master(FormCollection f)
        {
            String Sqltext = "update  Bank_master set bankid =@bankid,bankname =@bankname,accountnumber =@accountnumber,bankaccno =@bankaccno,chequestart =@chequestart,chequeend =@chequeend,divid =@divid,nextchequeno =@nextchequeno where bankid=@bankid";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@bankid", f["bankid"]);

cmd.Parameters.AddWithValue("@bankname", f["bankname"]);
cmd.Parameters.AddWithValue("@accountnumber", f["accountnumber"]);
cmd.Parameters.AddWithValue("@bankaccno", f["bankaccno"]);
cmd.Parameters.AddWithValue("@chequestart", f["chequestart"]);
cmd.Parameters.AddWithValue("@chequeend", f["chequeend"]);
cmd.Parameters.AddWithValue("@divid", f["divid"]);
cmd.Parameters.AddWithValue("@nextchequeno", f["nextchequeno"]);

            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }

        public Boolean DeleteRecord(int id) {
            String Sqltext = "delete from Bank_master where bankid=@id";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@id", id);
            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }
    }
}