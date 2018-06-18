using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ProfitPlus.Helpers;

namespace ProfitPlus.Models
{
    public class Depot_master
    {
					public int depotid{get;set;}
			public String depotname{get;set;}
			public int divid{get;set;}

		
		public DataTable GetOrderedRecords(int CurPage, int PerPage, String OrderBy) {
                int start = (CurPage-1) * PerPage +1;
                int endrec = start + PerPage - 1;
                String Qry = "SELECT A.depotid as depotid,A.depotname,d.divname FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   depot_master ) AS A inner join div_master as d on d.divid=A.divid";
                    Qry+= " WHERE A.rownum BETWEEN "+start.ToString()+" AND "+endrec.ToString();
                    return DBClass.GetAllRecords(Qry);
            }

            public Int32 GetOrderedRecordsCount(String OrderBy)
            {
                
                String Qry = "SELECT * FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   Depot_master ) AS A";
                return DBClass.GetAllRecords(Qry).Rows.Count ;
            }
			
		public Boolean InsertDepot_master(FormCollection f) {
            GenHelper g = new GenHelper();
            String Sqltext = "insert into Depot_master (depotid,depotname,divid) values(@depotid,@depotname,@divid)";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
			cmd.Parameters.AddWithValue("@depotid", g.GetNextId("Depot_master","depotid"));
            cmd.Parameters.AddWithValue("@depotname", f["depotname"]);
cmd.Parameters.AddWithValue("@divid", f["divid"]);

            int ab=cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true;else return false ;
        }

        public Boolean UpdateDepot_master(FormCollection f)
        {
            String Sqltext = "update  Depot_master set depotid =@depotid,depotname =@depotname,divid =@divid where depotid=@depotid";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@depotid", f["depotid"]);

cmd.Parameters.AddWithValue("@depotname", f["depotname"]);
cmd.Parameters.AddWithValue("@divid", f["divid"]);

            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }

        public Boolean DeleteRecord(int id) {
            String Sqltext = "delete from Depot_master where depotid=@id";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@id", id);
            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }
        public Boolean IsAvailable(String depotname)
        {
            DataTable dt = DBClass.GetAllRecords("select depotname from depot_master where depotname='" + depotname + "'");
            if (dt.Rows.Count > 0) return false; else return true;

        }
    }
}