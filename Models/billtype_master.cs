using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ProfitPlus.Helpers;

namespace ProfitPlus.Models
{
    public class Billtype_master
    {
					public int billtypeid{get;set;}
			public String billtypename{get;set;}
			public int sectionid{get;set;}
			public int divid{get;set;}
			public int active{get;set;}

		
		public DataTable GetOrderedRecords(int CurPage, int PerPage, String OrderBy) {
                int start = (CurPage-1) * PerPage +1;
                int endrec = start + PerPage - 1;
                String Qry = "SELECT A.billtypeid,A.billtypename,s.sectionname,d.divname FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   Billtype_master ) AS A inner join section_master as s on A.sectionid=s.sectionid inner join div_master as d on A.divid=d.divid";
                    Qry+= " WHERE A.rownum BETWEEN "+start.ToString()+" AND "+endrec.ToString();
                    return DBClass.GetAllRecords(Qry);
            }

            public Int32 GetOrderedRecordsCount(String OrderBy)
            {
                
                String Qry = "SELECT * FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   Billtype_master ) AS A";
                return DBClass.GetAllRecords(Qry).Rows.Count ;
            }
			
		public Boolean InsertBilltype_master(FormCollection f) {
            GenHelper g = new GenHelper();
            String Sqltext = "insert into Billtype_master (billtypeid,billtypename,sectionid,divid) values(@billtypeid,@billtypename,@sectionid,@divid)";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
			cmd.Parameters.AddWithValue("@billtypeid", g.GetNextId("Billtype_master","billtypeid"));
            cmd.Parameters.AddWithValue("@billtypename", f["billtypename"]);
            cmd.Parameters.AddWithValue("@sectionid", f["sectionid"]);
            cmd.Parameters.AddWithValue("@divid", f["divid"]);
         //   cmd.Parameters.AddWithValue("@active", f["active"]);

            int ab=cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true;else return false ;
        }

        public Boolean UpdateBilltype_master(FormCollection f)
        {
            String Sqltext = "update  Billtype_master set billtypeid =@billtypeid,billtypename =@billtypename,sectionid =@sectionid,divid =@divid,active =@active where billtypeid=@billtypeid";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@billtypeid", f["billtypeid"]);

        cmd.Parameters.AddWithValue("@billtypename", f["billtypename"]);
        cmd.Parameters.AddWithValue("@sectionid", f["sectionid"]);
        cmd.Parameters.AddWithValue("@divid", f["divid"]);
        cmd.Parameters.AddWithValue("@active", f["active"]);

            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }

        public Boolean DeleteRecord(int id) {
            String Sqltext = "delete from Billtype_master where billtypeid=@id";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@id", id);
            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }
        public Boolean IsAvailable(String billtypename)
        {
            DataTable dt = DBClass.GetAllRecords("select billtypename from billtype_master where billtypename='" + billtypename + "'");
            if (dt.Rows.Count > 0) return false; else return true;

        }
        public String Netpayable(Int32 billtypeid)
        {
            DataTable dt = DBClass.GetData("select accountnumber from billtype_master where billtypeid=" + billtypeid);
            return dt.Rows[0]["accountnumber"].ToString();
        
        }
    }
}