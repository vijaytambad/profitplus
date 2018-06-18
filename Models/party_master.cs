using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ProfitPlus.Helpers;

namespace ProfitPlus.Models
{
    public class Party_master
    {
			public int partyid{get;set;}
			public String partyname{get;set;}
			public int divid{get;set;}
			public int sectionid{get;set;}
            public String MobileNo { get; set; }
            public String PhoneNo {get;set;}
            public String pincode { get; set; }
            public String city { get; set; }
            public String state { get; set; }
            public String GSTNo { get; set; }
            public String TANNo { get; set; }
            public String PANNo { get; set; }

            public DataTable GetOrderedRecords(int CurPage, int PerPage, String OrderBy)
            {
                int start = (CurPage - 1) * PerPage + 1;
                int endrec = start + PerPage - 1;
                String Qry = "SELECT A.partyid,A.partyname,d.divname,s.sectionname FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   party_master ) AS A inner join div_master as d on A.divid=d.divid inner join section_master as s on A.sectionid=s.sectionid";
                Qry += " WHERE A.rownum BETWEEN " + start.ToString() + " AND " + endrec.ToString();
                return DBClass.GetAllRecords(Qry);
            }

            public Int32 GetOrderedRecordsCount(String OrderBy)
            {

                String Qry = "SELECT * FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   party_master ) AS A";
                return DBClass.GetAllRecords(Qry).Rows.Count;
            }

		
		public Boolean InsertParty_master(FormCollection f) {
            GenHelper g = new GenHelper();
            String Sqltext = "insert into Party_master (partyid,partyname,divid,sectionid,Address,city,state,pincode,MobileNo,PhoneNo,GSTNo,TANNo,PANNo) values(@partyid,@partyname,@divid,@sectionid,@Address,@city,@state,@pincode,@MobileNo,@PhoneNo,@GSTNo,@TANNo,@PANNo)";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
			cmd.Parameters.AddWithValue("@partyid", g.GetNextId("Party_master","partyid"));
            cmd.Parameters.AddWithValue("@partyname", f["partyname"]);
            cmd.Parameters.AddWithValue("@divid", f["divid"]);
            cmd.Parameters.AddWithValue("@sectionid", f["sectionid"]);
            cmd.Parameters.AddWithValue("@Address", f["Address"]);
            cmd.Parameters.AddWithValue("@city", f["city"]);
            cmd.Parameters.AddWithValue("@state", f["state"]);
            cmd.Parameters.AddWithValue("@pincode", f["pincode"]);
            cmd.Parameters.AddWithValue("@MobileNo", f["MobileNo"]);
            cmd.Parameters.AddWithValue("@PhoneNo", f["PhoneNo"]);
            //cmd.Parameters.AddWithValue("@sectionid", f["MobileNo"]);
            cmd.Parameters.AddWithValue("@GSTNo", f["GSTNo"]);
            cmd.Parameters.AddWithValue("@TANNo", f["TANNo"]);
            cmd.Parameters.AddWithValue("@PANNo", f["PANNo"]);
            int ab=cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true;else return false ;
        }

        public Boolean UpdateParty_master(FormCollection f)
        {
            String Sqltext = "update  Party_master set partyid =@partyid,partyname =@partyname,divid =@divid,sectionid =@sectionid,Address=@Address,city=@city,state=@state,pincode=@pincode,MobileNo=@MobileNo,PhoneNo=@PhoneNo,GSTNo=@GSTNo,TANNo=@TANNo,PANNo=@PANNo where partyid=@partyid";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@partyid", f["partyid"]);
            cmd.Parameters.AddWithValue("@partyname", f["partyname"]);
            cmd.Parameters.AddWithValue("@divid", f["divid"]);
            cmd.Parameters.AddWithValue("@sectionid", f["sectionid"]);
            cmd.Parameters.AddWithValue("@Address", f["Address"]);
            cmd.Parameters.AddWithValue("@city", f["city"]);
            cmd.Parameters.AddWithValue("@state", f["state"]);
            cmd.Parameters.AddWithValue("@pincode", f["pincode"]);
            cmd.Parameters.AddWithValue("@MobileNo", f["MobileNo"]);
            cmd.Parameters.AddWithValue("@PhoneNo", f["PhoneNo"]);
            cmd.Parameters.AddWithValue("@GSTNo", f["GSTNo"]);
            cmd.Parameters.AddWithValue("@TANNo", f["TANNo"]);
            cmd.Parameters.AddWithValue("@PANNo", f["PANNo"]);
            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }

        public Boolean DeleteRecord(int id) {
            String Sqltext = "delete from Party_master where partyid=@id";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@id", id);
            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }
        public Boolean IsAvailable(String partyname, int divid)
        {

            DataTable dt = DBClass.GetAllRecords("select partyname from party_master where partyname='" + partyname + "' and divid=" + divid);
            if (dt.Rows.Count > 0) return false; else return true;

        }
    }
}