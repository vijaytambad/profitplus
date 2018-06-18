using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ProfitPlus.Helpers;

namespace ProfitPlus.Models
{
    public class Div_master
    {
					public int divid{get;set;}
			public String divname{get;set;}

		
		public DataTable GetOrderedRecords(int CurPage, int PerPage, String OrderBy) {
                int start = (CurPage-1) * PerPage +1;
                int endrec = start + PerPage - 1;
                String Qry = "SELECT A.divid,A.divname FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   Div_master ) AS A";
                    Qry+= " WHERE A.rownum BETWEEN "+start.ToString()+" AND "+endrec.ToString();
                    return DBClass.GetAllRecords(Qry);
            }

            public Int32 GetOrderedRecordsCount(String OrderBy)
            {
                
                String Qry = "SELECT * FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   Div_master ) AS A";
                return DBClass.GetAllRecords(Qry).Rows.Count ;
            }
			
		public Boolean InsertDiv_master(FormCollection f) {
           
            GenHelper g = new GenHelper();
            String Sqltext = "insert into Div_master (divid,divname) values(@divid,@divname)";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            int newid=g.GetNextId("Div_master","divid");
			cmd.Parameters.AddWithValue("@divid", newid);
            cmd.Parameters.AddWithValue("@divname", f["divname"]);
            int ab=cmd.ExecuteNonQuery();
            String Sql = "insert into autoidgen(id,cadtvoc,padtvoc,sadtvoc,radtvoc,adjvoc,cadtbpo,padtbpo,radtbpo,sadtbpo,divid,cash) values(@id,0,0,0,0,0,0,0,0,0,@divid,0)";
            SqlCommand cmd1 = new SqlCommand(Sql,con);
            cmd1.Parameters.AddWithValue("@id",g.GetNextId("autoidgen","id"));
            cmd1.Parameters.AddWithValue("@divid",newid);
            int ab1 = cmd1.ExecuteNonQuery();
            con.Close();
            if (ab>0 && ab1>0) return true;else return false ;

        }

        public Boolean UpdateDiv_master(FormCollection f)
        {
            String Sqltext = "update  Div_master set divid =@divid,divname =@divname where divid=@divid";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@divid", f["divid"]);

cmd.Parameters.AddWithValue("@divname", f["divname"]);

            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }

        public Boolean DeleteRecord(int id) {
            String Sqltext = "delete from Div_master where divid=@id";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@id", id);
            try
            {
                int ab = cmd.ExecuteNonQuery();
                con.Close();
                if (ab > 0) return true; else return false;
            }
            catch (Exception ex) {
                return false;
            }
        }
        public Boolean IsAvailable(String divname)
        {
            DataTable dt = DBClass.GetAllRecords("select divname from div_master where divname='" + divname + "'");
            if (dt.Rows.Count > 0) return false; else return true;

        }

    }
}