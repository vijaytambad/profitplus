using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Mvc;
using ProfitPlus.Helpers;
using System.Data.SqlClient;
namespace ProfitPlus.Models
{
    public class CadtBpo
    {
        public String bpotype = "Exp";
        public int transtype = 6;
        public DataTable GetPartyTrans(int partyid) {
           /* String Sql1 = "SELECT party_master.partyid,party_master.partyname,party_ledger.dbamount,(party_ledger.cramount-paidamt) topay,0 thistrans,paidamt,party_ledger.dbamount,party_ledger.cramount,party_ledger.trdate,party_ledger.voucherno ";
            Sql1+=" FROM party_master INNER JOIN party_ledger ON party_master.partyid = party_ledger.partyid";
            Sql1 += " where party_master.partyid=" + partyid.ToString(); */
            String Sql1 = "Select voucher_master.*,party_master.partyname FROM voucher_master INNER JOIN party_master ON voucher_master.partyid = party_master.partyid where voucher_master.partyid=party_master.partyid and voucher_master.divid=1 and voucher_master.sectionid=1 and voucher_master.status =0";

            return DBClass.GetData(Sql1);

        }


        public Boolean SaveBpo(String bpos,String datebpo,DateTime fst, DateTime fend,Int32 divid, String divname,Int32 sectionid,Int32 uid){
            SqlTransaction tr= null;
            GenHelper g = new GenHelper();
            String[] SelectedBpos = bpos.Split('|');
            Int32 id = g.GetNextId("bpo_master", "id");
            int did = g.GetNextId("bpo_trans", "id");
            String bponumber = g.GetNewVoucherNo(this.transtype, fst, fend, divid, divname);
            SqlConnection con = DBClass.mycon();
            tr = con.BeginTransaction();
            try
            {
                String Sql = "insert into bpo_master(id,bponumber,bpotype,bpodate,sectionid,divid,userid,enteredon,typeofbpo)";
                Sql += " values(@id,@bponumber,@bpotype,@bpodate,@sectionid,@divid,@userid,@enteredon,@typeofbpo)";
                SqlCommand cmd = new SqlCommand(Sql, con);
                cmd.Parameters.AddWithValue("@id", g.GetNextId("bpo_master", "id"));
                cmd.Parameters.AddWithValue("@bponumber", bponumber);
                cmd.Parameters.AddWithValue("@bpotype", this.transtype);
                cmd.Parameters.AddWithValue("@bpodate", datebpo);
                cmd.Parameters.AddWithValue("@sectionid", sectionid);
                cmd.Parameters.AddWithValue("@userid", uid);
                cmd.Parameters.AddWithValue("@divid", divid);
                cmd.Parameters.AddWithValue("@enteredon", DateTime.Today);
                cmd.Parameters.AddWithValue("@typeofbpo", this.bpotype);
                cmd.Transaction = tr;
                cmd.ExecuteNonQuery();
                String Sql1;
                SqlCommand Cmd1;
                for (int i = 0; i < SelectedBpos.Length - 1; i++) {
                    DataTable dt = DBClass.GetData("select * from voucher_master where id=" + SelectedBpos[i].ToString());
                    Sql1 = "insert into bpo_trans(id,bponumber,accountnumber,partyid,sectionid,userid,enteredon,dramount,status,voucherno)";
                    Sql1 += " values(@id,@bponumber,@accountnumber,@partyid,@sectionid,@userid,@enteredon,@dramount,@status,@vcno)";
                    SqlCommand cmd1 = new SqlCommand(Sql1, con);
                    cmd1.Parameters.AddWithValue("@id",  did);
                    cmd1.Parameters.AddWithValue("@bponumber", bponumber);
                    cmd1.Parameters.AddWithValue("@accountnumber", dt.Rows[0]["accountnumber"]);
                    cmd1.Parameters.AddWithValue("@partyid", dt.Rows[0]["partyid"]);
                    cmd1.Parameters.AddWithValue("@sectionid", sectionid);
                    cmd1.Parameters.AddWithValue("@userid", uid);
                    cmd1.Parameters.AddWithValue("@enteredon", DateTime.Today);
                    cmd1.Parameters.AddWithValue("@dramount", dt.Rows[0]["dueamount"]);
                    cmd1.Parameters.AddWithValue("@vcno", dt.Rows[0]["voucherno"]);
                    cmd1.Parameters.AddWithValue("@status", 0);
                    cmd1.Transaction = tr;
                    cmd1.ExecuteNonQuery();
                    did++;
                    SqlCommand cmd2;
                    String Sql2 = "update voucher_master set status=@status where id="+SelectedBpos[i].ToString();
                    cmd2 = new SqlCommand(Sql2, con);
                    cmd2.Parameters.AddWithValue("@status", 1);
                    cmd2.Transaction = tr;
                    cmd2.ExecuteNonQuery();
                }

                
                tr.Commit();
                return true;
            }

            catch(Exception ex) {
                g.logerror(ex);
                tr.Rollback();
                return false;
            }
            
        }
    }
}