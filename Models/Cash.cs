using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using ProfitPlus.Helpers;
using System.Web.Mvc;
using System.Data.SqlClient;

namespace ProfitPlus.Models
{
    public class Cash
    {
        private int transtype=10;
        
        public DataTable GetBpos(String Bpno,int divid){
            String Sql = "SELECT bp.id,bp.bponumber,bp.accountnumber,bp.paidamt,bp.partyid,pt.partyname,bp.dramount,";
            Sql += "'               ' as chequeno,'           ' as chequedate,'                ' as 'accountno',";
            Sql += " 0 as ToPay,bp.voucherno,'          ' as paytype FROM dbo.bpo_trans AS bp ";
                   Sql+=" INNER JOIN party_master AS pt ON bp.partyid=pt.partyid where bp.bponumber='" + Bpno + "' and bp.status=0 and bp.divid="+divid.ToString();
            return DBClass.GetData(Sql);

        }


        public DataTable FindBpo(Int32 BpoNum) {
            String Sql = "Select *,(dramount-paidamt) as topayamt";
            
            Sql+=" from bpo_trans where id="+BpoNum.ToString();
            return DBClass.GetData(Sql);
        }

        public DataTable GetBankAccounts(Int32 divid) {
            String Sql = "select accountnumber,bankname from bank_master where divid="+divid.ToString();
            return DBClass.GetData(Sql);
        }

        public DataTable baclist(Int32 divid) {
            String Sql = "select bankname,accountnumber from bank_master where divid=" + divid.ToString();
            return DBClass.GetData(Sql);
        }
        public Boolean SaveCash(FormCollection f, DataTable userdt, DataTable Cardt, DateTime fst, DateTime fend, int sectionid, int divid, String divname, int userid, ref String trid)
        {
            /*  Steps for saving Cash Entry
            *   Generate new cash Trans id
            *   Save Cash Master Record
            *   Save Cash Trans Record
            *   Add Transaction to Transaction table
            *   Find Bpo Entry And Update The Record Status and amount paid
            *   Find Party in Party Master Post Debit Entry for that bpo 
            *   update autoid generation table
            */
            SqlTransaction tr = null;
            GenHelper g = new GenHelper();
            Int32 id = g.GetNextId("cash_master", "id");
            Int32 cashtrid = g.GetNextId("cash_trans", "id");
            String CashNo = g.GetNewVoucherNo(transtype, fst, fend, divid, divname);
            trid = CashNo;
            SqlConnection con = DBClass.mycon();
             tr= con.BeginTransaction();
             try
             {
                 String Qry = "insert into cash_master(id,cashid,cashdate,status) values(@cid,@cashid,@cashdate,@status)";
                 SqlCommand cmd = new SqlCommand(Qry, con);
                 cmd.Parameters.AddWithValue("@cid", id);
                 cmd.Parameters.AddWithValue("@cashid", CashNo);
                 cmd.Parameters.AddWithValue("@cashdate", DateTime.Today);
                 cmd.Parameters.AddWithValue("@status", 0);
                 cmd.Transaction = tr;
                 cmd.ExecuteNonQuery();

                 String Sql = "insert into cash_trans(id,cashid,bponumber,paidamt,chequeno,accountnumber,chequedate,paymenttype) ";
                 //Sql += "values(" + cashtrid.ToString() + ",'" + CashNo + "','" + Cardt.Rows[i]["bponumber"] + "'," + Cardt.Rows[i]["paidamt"] + ",'";
                 //Sql += Cardt.Rows[i]["chequeno"] + "','" + Cardt.Rows[i]["accountnumber"] + "','" + Cardt.Rows[i]["chequedate"] + "'," + Cardt.Rows[i]["paytype"].ToString()+")";
                 Sql += " values(@cid,@cashid,@bponumber,@paidamt,@chequeno,@accountnumber,@chequedate,@paymenttype)";
                 for (int i = 0; i < Cardt.Rows.Count; i++)
                 {
                     if (Convert.ToDouble(Cardt.Rows[i]["paidamt"]) > 0)
                     {

                         SqlCommand trcmd = new SqlCommand(Sql, con);
                         trcmd.Parameters.AddWithValue("@cid", cashtrid.ToString());
                         trcmd.Parameters.AddWithValue("@cashid", CashNo);
                         trcmd.Parameters.AddWithValue("@bponumber", Cardt.Rows[i]["bponumber"]);
                         trcmd.Parameters.AddWithValue("@paidamt", Cardt.Rows[i]["paidamt"]);
                         trcmd.Parameters.AddWithValue("@chequeno", Cardt.Rows[i]["chequeno"]);
                         trcmd.Parameters.AddWithValue("@accountnumber", Cardt.Rows[i]["accountnumber"]);
                         trcmd.Parameters.AddWithValue("@chequedate", Cardt.Rows[i]["chequedate"]);
                         trcmd.Parameters.AddWithValue("@paymenttype", Cardt.Rows[i]["paytype"].ToString());
                         trcmd.Transaction = tr;
                         trcmd.ExecuteNonQuery();
                         cashtrid++;
                     }
                 }

                 /* find Bpo and update the status */


                 for (int i = 0; i < Cardt.Rows.Count; i++)
                 {
                     String BpoSql = "update bpo_trans set paidamt=paidamt+@paidamt,status=@status where bponumber='" + Convert.ToString(Cardt.Rows[i]["bponumber"]) + "'";
                     int bpostatus = SetBpoStatus(Convert.ToDouble(Cardt.Rows[i]["paidamt"]), Convert.ToString(Cardt.Rows[i]["bponumber"]), divid);
                     SqlCommand cmdbpo = new SqlCommand(BpoSql, con);
                     cmdbpo.Parameters.AddWithValue("@paidamt", Cardt.Rows[i]["paidamt"]);
                     cmdbpo.Parameters.AddWithValue("@status", bpostatus);
                     cmdbpo.Transaction = tr;
                     cmdbpo.ExecuteNonQuery();
                 }

                 /* party postings */

                 for (int i = 0; i < Cardt.Rows.Count; i++)
                 {
                     Int32 pid = g.GetNextId("party_ledger", "id");
                     String PrtSql = "insert into party_ledger(id,partyid,divid,dbamount,cramount,trdate,voucherno,cashid,bponumber)";
                     PrtSql += "values(@id,@partyid,@divid,@dbamount,@cramount,@trdate,@voucherno,@cashid,@bponumber)";

                     SqlCommand cmdprt = new SqlCommand(PrtSql, con);
                     cmdprt.Parameters.AddWithValue("@id", pid);
                     cmdprt.Parameters.AddWithValue("@partyid", Cardt.Rows[i]["partyid"]);
                     cmdprt.Parameters.AddWithValue("@divid", divid);
                     cmdprt.Parameters.AddWithValue("@dbamount", Cardt.Rows[i]["paidamt"]);
                     cmdprt.Parameters.AddWithValue("@cramount", 0);
                     cmdprt.Parameters.AddWithValue("@voucherno", Cardt.Rows[i]["voucherno"]);
                     cmdprt.Parameters.AddWithValue("@bponumber", Cardt.Rows[i]["bponumber"]);
                     cmdprt.Parameters.AddWithValue("@cashid", cashtrid);
                     cmdprt.Parameters.AddWithValue("@trdate", Cardt.Rows[i]["chequedate"]);
                     cmdprt.Transaction = tr;
                     cmdprt.ExecuteNonQuery();
                 }

                 Int32 newid = g.GetNextId("accounttrans", "id");
                 String Trqry = "insert into accounttrans (id,trtype,voucherno,accountnumber,trdate,narration,dramount,cramount,partyid,divid,sectionid,userid,enteredon) ";
                 Trqry += " values(@id,@trtype,@voucherno,@accno,@trdate,@narration,@dramount,@cramount,@partyid,@divid,@sectionid,@userid,@enteredon)";
                 for (int i = 0; i < Cardt.Rows.Count; i++)
                 {
                     if (Convert.ToInt32(Cardt.Rows[i]["paidamt"]) > 0)
                     {
                         SqlCommand cmdtr = new SqlCommand(Trqry, con);

                         cmdtr.Parameters.AddWithValue("@id", newid);
                         cmdtr.Parameters.AddWithValue("@trtype", this.transtype);
                         //cmdtr.Parameters.AddWithValue(@transid,
                         cmdtr.Parameters.AddWithValue("@voucherno", Cardt.Rows[i]["voucherno"]);
                         cmdtr.Parameters.AddWithValue("@accno", Cardt.Rows[i]["accountnumber"].ToString());
                         cmdtr.Parameters.AddWithValue("@trdate", Cardt.Rows[i]["chequedate"]);
                         cmdtr.Parameters.AddWithValue("@narration", "My Narration");
                         cmdtr.Parameters.AddWithValue("@dramount", Cardt.Rows[i]["paidamt"].ToString());
                         cmdtr.Parameters.AddWithValue("@cramount", 0);
                         cmdtr.Parameters.AddWithValue("@partyid", Cardt.Rows[i]["partyid"]);
                         cmdtr.Parameters.AddWithValue("@divid", divid);
                         cmdtr.Parameters.AddWithValue("@sectionid", sectionid);
                         cmdtr.Parameters.AddWithValue("@userid", userid);
                         cmdtr.Parameters.AddWithValue("@enteredon", DateTime.Today);
                         //cmdtr.Parameters.AddWithValue("@depoid",
                         cmdtr.Transaction = tr;
                         cmdtr.ExecuteNonQuery();
                         newid++;
                         SqlCommand cmdtr1 = new SqlCommand(Trqry, con);

                         cmdtr1.Parameters.AddWithValue("@id", newid);
                         cmdtr1.Parameters.AddWithValue("@trtype", this.transtype);
                         //cmdtr.Parameters.AddWithValue(@transid,
                         cmdtr1.Parameters.AddWithValue("@voucherno", Cardt.Rows[i]["voucherno"]);
                         cmdtr1.Parameters.AddWithValue("@accno", Cardt.Rows[i]["accountno"].ToString());
                         cmdtr1.Parameters.AddWithValue("@trdate", Cardt.Rows[i]["chequedate"]);
                         cmdtr1.Parameters.AddWithValue("@narration", "My Narration");
                         cmdtr1.Parameters.AddWithValue("@dramount", 0);
                         cmdtr1.Parameters.AddWithValue("@cramount", Cardt.Rows[i]["paidamt"].ToString());
                         cmdtr1.Parameters.AddWithValue("@partyid", Cardt.Rows[i]["partyid"]);
                         cmdtr1.Parameters.AddWithValue("@divid", divid);
                         cmdtr1.Parameters.AddWithValue("@sectionid", sectionid);
                         cmdtr1.Parameters.AddWithValue("@userid", userid);
                         cmdtr1.Parameters.AddWithValue("@enteredon", DateTime.Today);
                         //cmdtr.Parameters.AddWithValue("@depoid",
                         cmdtr1.Transaction = tr;
                         cmdtr1.ExecuteNonQuery();
                     }
                 }

                 String AutoQry = "update autoidgen set cash=cash+1 where divid=" + divid.ToString();
                 SqlCommand cmdauto = new SqlCommand(AutoQry, con);
                 cmdauto.Transaction = tr;
                 cmdauto.ExecuteNonQuery();
                 tr.Commit();
                 return true;
             }
             catch (Exception ex)
             {
                 g.logerror(ex);
                 tr.Rollback();
                 return false;
             }
             finally
             {
                 con.Close();
             }
        }

        

        public int SetBpoStatus(double tramount,String bponum,int mydivid){
            int status = 0;
            String SqlBpo = "select * from bpo_trans where bponumber='" + bponum + "' and divid=" + mydivid.ToString();
            DataTable bpotable = DBClass.GetData(SqlBpo);
            if (Convert.ToDouble(tramount) + Convert.ToDouble(tramount) + Convert.ToDouble(bpotable.Rows[0]["paidamt"]) >= Convert.ToDouble(bpotable.Rows[0]["dramount"]))
                status = 1;
            return status;
        }

    }
}