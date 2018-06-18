using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ProfitPlus.Models;
using ProfitPlus.Helpers;

namespace ProfitPlus.Models
{
    public class Cadtvoucher
    {

        public int id { get; set; }
        public String voucherno { get; set; }
        public DateTime vodate { get; set; }
        public int billtypeid { get; set; }
        public int sectionid { get; set; }
        public int divid { get; set; }
        public int userid { get; set; }
        public int partyid { get; set; }
        public String rvnumber { get; set; }
        public String ponumber { get; set; }
        public String invoiceno { get; set; }
        public int status { get; set; }
        public DateTime enteredon { get; set; }
        public DateTime rvdate { get; set; }
        public DateTime podate { get; set; }
        public DateTime invdate { get; set; }
        public int transtype = 1;


        public Boolean IsValid(DateTime vdate)
        {
            Boolean status = false;
            DataTable dt = DBClass.GetAllRecords("select * from finyear");
            if (vdate.Date >= Convert.ToDateTime(dt.Rows[0]["finyearbegin"]) && vdate.Date <= Convert.ToDateTime(dt.Rows[0]["finyearend"]))
            {
                status = true;
            }
            else
            {
                status = false;
            }
            return status;
        }
        public Boolean savevoucherdata(FormCollection f, DataTable userdt, DataTable Cardt, ref String trid, DateTime fst, DateTime fend, int divid, String divname, int userid, int sectionid,double payamt)
        {
            SqlTransaction tr = null;
            GenHelper g = new GenHelper();
            trid = g.GetNewVoucherNo(transtype, fst, fend, divid, divname);
            SqlConnection con = DBClass.mycon();
            
            tr = con.BeginTransaction();
            try
            {
                String Sql = "insert into voucher_master(id,voucherno,vodate,billtypeid,divid,rvnumber,ponumber,invoiceno,userid,partyid,enteredon,trtypeid,sectionid,rvdate,podate,invdate)";
                Sql += " values(@id,@voucherno,@vodate,@billtypeid,@divid,@rvnumber,@ponumber,@invoiceno,@userid,@partyid,@enteredon,@trtypeid,@sectionid,@rvdate,@podate,@invdate)";
                SqlCommand cmd = new SqlCommand(Sql,con);
                cmd.Parameters.AddWithValue("@id", g.GetNextId("voucher_master", "id"));
                cmd.Parameters.AddWithValue("@voucherno", trid);
                cmd.Parameters.AddWithValue("@vodate", f["vdate"]);
                cmd.Parameters.AddWithValue("@billtypeid", f["billtypeid"]);
                cmd.Parameters.AddWithValue("@divid", divid);
                cmd.Parameters.AddWithValue("@rvnumber", f["rvno"]);
                cmd.Parameters.AddWithValue("@ponumber", f["pono"]);
                cmd.Parameters.AddWithValue("@invoiceno", f["invoice"]);
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@partyid", f["partyid"]);
                cmd.Parameters.AddWithValue("@trtypeid", this.transtype);
                cmd.Parameters.AddWithValue("@sectionid", sectionid);
                cmd.Parameters.AddWithValue("@rvdate", f["rvdate"]);
                cmd.Parameters.AddWithValue("@podate", f["podate"]);
                cmd.Parameters.AddWithValue("@invdate", f["invoicedate"]);
                cmd.Parameters.AddWithValue("@enteredon", DateTime.Today);
                cmd.Transaction = tr;
                int a = cmd.ExecuteNonQuery();


                Int32 newid = g.GetNextId("accounttrans","id");
                String Trqry = "insert into accounttrans(id,trtype,voucherno,accno,trdate,dramount,cramount,partyid,divid,sectionid,userid,enteredon)";
                Trqry += " values(@id,@trtype,@voucherno,@accno,@trdate,@dramount,@cramount,@partyid,@divid,@sectionid,@userid,@enteredon)";
                for (int i = 0; i < Cardt.Rows.Count; i++)
                {
                if(Convert.ToInt32(Cardt.Rows[i]["dramount"])>0 || Convert.ToInt32(Cardt.Rows[i]["cramount"])>0)
                  {
                SqlCommand cmd1 = new SqlCommand(Trqry, con);

                    cmd1.Parameters.AddWithValue("@id", newid);
                    cmd1.Parameters.AddWithValue("@trtype", this.transtype);
                    cmd1.Parameters.AddWithValue("@voucherno", trid);
                    cmd1.Parameters.AddWithValue("@accno", Cardt.Rows[i]["accountnumber"]);
                    cmd1.Parameters.AddWithValue("@trdate", f["vdate"]);
                    cmd1.Parameters.AddWithValue("@dramount", Cardt.Rows[i]["dramount"]);
                    cmd1.Parameters.AddWithValue("@cramount", Cardt.Rows[i]["cramount"]);
                    cmd1.Parameters.AddWithValue("@partyid", f["partyid"]);
                    cmd1.Parameters.AddWithValue("@divid", divid);
                    cmd1.Parameters.AddWithValue("@sectionid", sectionid);
                    cmd1.Parameters.AddWithValue("@userid", userid);
                    cmd1.Parameters.AddWithValue("@enteredon", DateTime.Now);
                    cmd1.Transaction = tr;
                    int a1 = cmd1.ExecuteNonQuery();
                    newid++;
                    }
                }

                Int32 newplid = g.GetNextId("party_ledger","id");
                String Plqry = "insert into party_ledger(id,partyid,divid,dbamount,trdate,voucherno)";
                Plqry += " values(@id,@partyid,@divid,@dbamount,@trdate,@voucherno)";
                SqlCommand cmdpl = new SqlCommand(Plqry,con);
                cmdpl.Parameters.AddWithValue("@id", newplid);
                cmdpl.Parameters.AddWithValue("@partyid",f["partyid"]);
                cmdpl.Parameters.AddWithValue("@divid",divid);
                cmdpl.Parameters.AddWithValue("@dbamount",payamt);
                cmdpl.Parameters.AddWithValue("@trdate", f["vdate"]);
                cmdpl.Parameters.AddWithValue("@voucherno",trid);
               
                cmdpl.Transaction = tr;
                int a2 = cmdpl.ExecuteNonQuery();


                String upautoid = "update autoidgen set cadtvoc=cadtvoc+1 where divid=" + divid;
                SqlCommand cmdupd = new SqlCommand(upautoid, con);
                cmdupd.Transaction = tr;
                int a4 = cmdupd.ExecuteNonQuery();
                tr.Commit();
                return true;

            }
            catch (Exception e)
            {
                tr.Rollback();
                return false;

            }

            finally
            { 
            
            }
        }


    }
}