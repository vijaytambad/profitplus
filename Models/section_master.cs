using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ProfitPlus.Helpers;

namespace ProfitPlus.Models
{
    public class Section_master
    {
			public int sectionid{get;set;}
			public String sectionname{get;set;}

		
		public DataTable GetOrderedRecords(int CurPage, int PerPage, String OrderBy) {
                int start = (CurPage-1) * PerPage +1;
                int endrec = start + PerPage - 1;
                String Qry = "SELECT A.sectionid,A.sectionname FROM ( SELECT row_number() OVER (ORDER BY "+OrderBy+") AS rownum, *   FROM   Section_master ) AS A";
                    Qry+= " WHERE A.rownum BETWEEN "+start.ToString()+" AND "+endrec.ToString();
                    return DBClass.GetAllRecords(Qry);
            }

            public Int32 GetOrderedRecordsCount(String OrderBy)
            {
                
                String Qry = "SELECT * FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   Section_master ) AS A";
                return DBClass.GetAllRecords(Qry).Rows.Count ;
            }
			
		public Boolean InsertSection_master(FormCollection f) {
            GenHelper g = new GenHelper();
            String Sqltext = "insert into Section_master (sectionid,sectionname) values(@sectionid,@sectionname)";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
			cmd.Parameters.AddWithValue("@sectionid", g.GetNextId("Section_master","sectionid"));
            cmd.Parameters.AddWithValue("@sectionname", f["sectionname"]);

            int ab=cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true;else return false ;
        }

        public Boolean UpdateSection_master(FormCollection f)
        {
            String Sqltext = "update  Section_master set sectionid =@sectionid,sectionname =@sectionname where sectionid=@sectionid";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@sectionid", f["sectionid"]);

cmd.Parameters.AddWithValue("@sectionname", f["sectionname"]);

            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }

        public Boolean DeleteRecord(int id, ref String st) {
            String Sqltext = "delete from Section_master where sectionid=@id";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@id", id);
            try
            {
                int ab = cmd.ExecuteNonQuery();
                con.Close();
                if (ab > 0) return true; else return false;
            }
            catch(Exception ex){
                st=ex.Message;
                return false;
            }
        }
        public Boolean IsAvailable(String sectionname)
        {
            DataTable dt = DBClass.GetAllRecords("select sectionname from section_master where sectionname='" + sectionname + "'");
            if (dt.Rows.Count > 0) return false; else return true;

        }
    }
}