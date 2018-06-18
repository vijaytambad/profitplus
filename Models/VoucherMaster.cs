using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ProfitPlus.Helpers;

namespace ProfitPlus.Models
{
    public class VoucherMaster
    {

        public int trtype { get; set; }
        public String accno { get; set; }
        public float dramount { get; set; }
        public float cramount { get; set; }

        public DataTable GetOrderedRecords(int CurPage, int PerPage, String OrderBy)
        {
            int start = (CurPage - 1) * PerPage + 1;
            int endrec = start + PerPage - 1;
            String Qry = "SELECT A.groupid,A.groupname,A1.parentname FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   Account_group ) AS A inner join account_parent as A1 on A.parentid=A1.parentid";
            Qry += " WHERE A.rownum BETWEEN " + start.ToString() + " AND " + endrec.ToString();
            return DBClass.GetAllRecords(Qry);
        }

        public Int32 GetOrderedRecordsCount(String OrderBy)
        {

            String Qry = "SELECT * FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   Account_group ) AS A";
            return DBClass.GetAllRecords(Qry).Rows.Count;
        }

        public Boolean Insertvoucherdata(FormCollection f,int divid,int sectionid,int userid)
        {       

            GenHelper g = new GenHelper();
            String Sqltext = "insert into accounttrans(id,trtype,accno,sectionid,divid,dramount,cramount,userid) values(@id,@trtype,@accno,@sectionid,@divid,@dramount,@cramount,@userid)";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@id", g.GetNextId("accounttrans", "id"));
            cmd.Parameters.AddWithValue("@trtype", f["trtype"]);
            cmd.Parameters.AddWithValue("@accno", f["accno"]);
            cmd.Parameters.AddWithValue("@sectionid", sectionid);
            cmd.Parameters.AddWithValue("@divid", divid);
            cmd.Parameters.AddWithValue("@dramount", f["dramount"]);
            cmd.Parameters.AddWithValue("@cramount", f["cramount"]);
            cmd.Parameters.AddWithValue("@userid", userid);

            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }
        public Boolean DeleteRecord(int id)
        {
            String Sqltext = "delete from accounttrans where id="+id;
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
           // cmd.Parameters.AddWithValue("@id", id);
            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }

    }
}