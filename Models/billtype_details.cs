using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ProfitPlus.Helpers;

namespace ProfitPlus.Models
{
    public class Billtype_details
    {
					public int id{get;set;}
			public int billtypeid{get;set;}
			public String accountnumber{get;set;}
			public String type{get;set;}


            public DataTable GetOrderedRecords(int CurPage, int PerPage, int billtypeid, String OrderBy)
            {
                int start = (CurPage - 1) * PerPage + 1;
                int endrec = start + PerPage - 1;
                String Qry = "SELECT * FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   Billtype_details ) AS A";
                Qry += " Where billtypeid=" + billtypeid.ToString() + " and A.rownum BETWEEN " + start.ToString() + " AND " + endrec.ToString();
                return DBClass.GetAllRecords(Qry);
            }

            public Int32 GetOrderedRecordsCount(String OrderBy)
            {
                
                String Qry = "SELECT * FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   Billtype_details ) AS A";
                return DBClass.GetAllRecords(Qry).Rows.Count ;
            }
			 
            public DataTable GetOrderedRecordsbilltype(int billtypeid, String OrderBy)
            {
                String Qry = "SELECT * FROM billtype_details Where billtypeid=" + billtypeid;
                return DBClass.GetAllRecords(Qry);
            }

       /**     public Int32 GetOrderedRecordsCountbilltype(int btypeid, String OrderBy)
            {

                String Qry = "SELECT * FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   Billtype_details ) AS A Where billtypeid=" + billtypeid.ToString();
                return DBClass.GetAllRecords(Qry).Rows.Count;
            }  **/

      /**      public Boolean InsertBilltype_details(FormCollection f)
            {
                GenHelper g = new GenHelper();
                String Sqltext = "insert into Billtype_details (id,billtypeid,accountnumber,type) values(@id,@billtypeid,@accountnumber,@type)";
                SqlConnection con = DBClass.mycon();
                SqlCommand cmd = new SqlCommand(Sqltext, con);
                cmd.Parameters.AddWithValue("@id", g.GetNextId("Billtype_details", "id"));
                cmd.Parameters.AddWithValue("@billtypeid", f["billtypeid"]);
                cmd.Parameters.AddWithValue("@accountnumber", f["accountnumber"]);
                cmd.Parameters.AddWithValue("@type", f["type"]);

                int ab = cmd.ExecuteNonQuery();
                con.Close();
                if (ab > 0) return true; else return false;
            }  **/
		public Boolean InsertBilltype_details(FormCollection f) {
            GenHelper g = new GenHelper();
            String Sqltext = "insert into Billtype_details (id,billtypeid,accountnumber,type) values(@id,@billtypeid,@accountnumber,@type)";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
			cmd.Parameters.AddWithValue("@id", g.GetNextId("Billtype_details","id"));
            cmd.Parameters.AddWithValue("@billtypeid", f["billtypeid"]);
cmd.Parameters.AddWithValue("@accountnumber", f["accountnumber"]);
cmd.Parameters.AddWithValue("@type", f["type"]);

            int ab=cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true;else return false ;
        }

        public Boolean UpdateBilltype_details(FormCollection f)
        {
            String Sqltext = "update  Billtype_details set id =@id,billtypeid =@billtypeid,accountnumber =@accountnumber,type=@type where id=@id";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@id", f["id"]);

cmd.Parameters.AddWithValue("@billtypeid", f["billtypeid"]);
cmd.Parameters.AddWithValue("@accountnumber", f["accountnumber"]);
cmd.Parameters.AddWithValue("@type", f["type"]);

            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }

        public Boolean DeleteRecord(int id) {
            String Sqltext = "delete from Billtype_details where id=@id";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@id", id);
            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }
    }
}