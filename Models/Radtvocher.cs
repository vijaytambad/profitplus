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
    public class Radtvocher
    {
        public int id { get; set; }
        public int sectionid { get; set; }
        public int divid { get; set; }
        DateTime fst { get; set; }
        DateTime fend { get; set; }
        String divname { get; set; }
        int userid { get; set; }
        int transtype = 4;


        public Boolean savevoucherdata(FormCollection f,DataTable userdt,DataTable Cardt,DateTime fst,DateTime fend,int sectionid,int divid,String divname,int userid,ref String trid)
        {
            SqlTransaction tr = null;
            GenHelper g = new GenHelper();
            trid = g.GetNewVoucherNo(transtype, fst, fend, divid, divname);
            SqlConnection con = DBClass.mycon();

            tr = con.BeginTransaction();
            try
            {
                String vouqry = "insert into voucher_master(id,voucherno,vodate,billtypeid,divid,userid,partyid,enteredon,trtypeid,sectionid,depotid)";
                vouqry += " values(@id,@voucherno,@vodate,@billtypeid,@divid,@userid,@partyid,@enteredon,@trtypeid,@sectionid,@depotid)";
                SqlCommand cmdvqry = new SqlCommand(vouqry, con);
                cmdvqry.Parameters.AddWithValue("@id", g.GetNextId("voucher_master", "id"));
                cmdvqry.Parameters.AddWithValue("@voucherno", trid);
                cmdvqry.Parameters.AddWithValue("@vodate", f["vdate"]);
                cmdvqry.Parameters.AddWithValue("@billtypeid", f["billtypeid"]);
                cmdvqry.Parameters.AddWithValue("@divid", divid);
                cmdvqry.Parameters.AddWithValue("@userid", userid);
                cmdvqry.Parameters.AddWithValue("@partyid", f["partyid"]);
                cmdvqry.Parameters.AddWithValue("@trtypeid", this.transtype);
                cmdvqry.Parameters.AddWithValue("@sectionid", sectionid);
                cmdvqry.Parameters.AddWithValue("@enteredon", DateTime.Now);
                cmdvqry.Parameters.AddWithValue("@depotid",f["depotid"]);
                cmdvqry.Transaction = tr;
                int a = cmdvqry.ExecuteNonQuery();

                Int32 newid = g.GetNextId("accounttrans", "id");
                String Trqry = "insert into accounttrans(id,trtype,voucherno,accno,trdate,dramount,cramount,partyid,divid,sectionid,userid,enteredon,depotid)";
                Trqry += " values(@id,@trtype,@voucherno,@accno,@trdate,@dramount,@cramount,@partyid,@divid,@sectionid,@userid,@enteredon,@depotid)";
                for (int i = 0; i < Cardt.Rows.Count; i++)
                {
                    if (Convert.ToInt32(Cardt.Rows[i]["dramount"]) > 0 || Convert.ToInt32(Cardt.Rows[i]["cramount"]) > 0)
                    {
                        SqlCommand cmdtrqry = new SqlCommand(Trqry, con);
                        cmdtrqry.Parameters.AddWithValue("@id", newid);
                        cmdtrqry.Parameters.AddWithValue("@trtype", this.transtype);
                        cmdtrqry.Parameters.AddWithValue("@voucherno", trid);
                        cmdtrqry.Parameters.AddWithValue("@accno", Cardt.Rows[i]["accountnumber"].ToString());
                        cmdtrqry.Parameters.AddWithValue("@trdate", f["vdate"]);
                        cmdtrqry.Parameters.AddWithValue("@dramount", Cardt.Rows[i]["dramount"].ToString());
                        cmdtrqry.Parameters.AddWithValue("@cramount", Cardt.Rows[i]["cramount"].ToString());
                        cmdtrqry.Parameters.AddWithValue("@partyid", f["partyid"]);
                        cmdtrqry.Parameters.AddWithValue("@divid", divid);
                        cmdtrqry.Parameters.AddWithValue("@sectionid", sectionid);
                        cmdtrqry.Parameters.AddWithValue("@userid", userid);
                        cmdtrqry.Parameters.AddWithValue("@depotid", f["depotid"]);
                        cmdtrqry.Parameters.AddWithValue("@enteredon", DateTime.Now);
                        cmdtrqry.Transaction = tr;
                        int a1 = cmdtrqry.ExecuteNonQuery();
                        newid++;
                    }
                }


                String upautoid = "update autoidgen set radtvoc=radtvoc+1 where divid=" + divid;
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
          
        
        return true;
        }



    }
}