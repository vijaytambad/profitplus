using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ProfitPlus.Helpers;

namespace ProfitPlus.Models
{
    public class RadtVoucher_master
    {
			public int id{get;set;}
			public String voucherno{get;set;}
			public String vodate{get;set;}
			public int billtypeid{get;set;}
			public int sectionid{get;set;}
			public int divid{get;set;}
			public int depoid{get;set;}
			public String rvnumber{get;set;}
			public String ponumber{get;set;}
			public String invoiceno{get;set;}
			public int userid{get;set;}
			public int status{get;set;}
			public int enteredon{get;set;}
            public int transtype = 4;
		
		public DataTable GetOrderedRecords(int CurPage, int PerPage, String OrderBy) {
                int start = (CurPage-1) * PerPage +1;
                int endrec = start + PerPage - 1;
                String Qry = "SELECT * FROM ( SELECT row_number() OVER (ORDER BY "+OrderBy+") AS rownum, *   FROM   Voucher_master ) AS A";
                    Qry+= " WHERE A.rownum BETWEEN "+start.ToString()+" AND "+endrec.ToString();
                    return DBClass.GetAllRecords(Qry);
            }

            public Int32 GetOrderedRecordsCount(String OrderBy)
            {
                
                String Qry = "SELECT * FROM ( SELECT row_number() OVER (ORDER BY " + OrderBy + ") AS rownum, *   FROM   Voucher_master ) AS A";
                return DBClass.GetAllRecords(Qry).Rows.Count ;
            }
			
		public Boolean InsertVoucher_master(FormCollection f) {
            GenHelper g = new GenHelper();
            String Sqltext = "insert into Voucher_master (id,voucherno,vodate,billtypeid,sectionid,divid,depoid,rvnumber,ponumber,invoiceno,userid,status,enteredon) values(@id,@voucherno,@vodate,@billtypeid,@sectionid,@divid,@depoid,@rvnumber,@ponumber,@invoiceno,@userid,@status,@enteredon)";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
			cmd.Parameters.AddWithValue("@id", g.GetNextId("Voucher_master","id"));
            cmd.Parameters.AddWithValue("@voucherno", f["voucherno"]);
cmd.Parameters.AddWithValue("@vodate", f["vodate"]);
cmd.Parameters.AddWithValue("@billtypeid", f["billtypeid"]);
cmd.Parameters.AddWithValue("@sectionid", f["sectionid"]);
cmd.Parameters.AddWithValue("@divid", f["divid"]);
cmd.Parameters.AddWithValue("@depoid", f["depoid"]);
cmd.Parameters.AddWithValue("@rvnumber", f["rvnumber"]);
cmd.Parameters.AddWithValue("@ponumber", f["ponumber"]);
cmd.Parameters.AddWithValue("@invoiceno", f["invoiceno"]);
cmd.Parameters.AddWithValue("@userid", f["userid"]);
cmd.Parameters.AddWithValue("@status", f["status"]);
cmd.Parameters.AddWithValue("@enteredon", f["enteredon"]);

            int ab=cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true;else return false ;
        }

        public Boolean UpdateVoucher_master(FormCollection f)
        {
            String Sqltext = "update  Voucher_master set id =@id,voucherno =@voucherno,vodate =@vodate,billtypeid =@billtypeid,sectionid =@sectionid,divid =@divid,depoid =@depoid,rvnumber =@rvnumber,ponumber =@ponumber,invoiceno =@invoiceno,userid =@userid,status =@status,enteredon =@enteredon where id=@id";
            SqlConnection con = DBClass.mycon();
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@id", f["id"]);

cmd.Parameters.AddWithValue("@voucherno", f["voucherno"]);
cmd.Parameters.AddWithValue("@vodate", f["vodate"]);
cmd.Parameters.AddWithValue("@billtypeid", f["billtypeid"]);
cmd.Parameters.AddWithValue("@sectionid", f["sectionid"]);
cmd.Parameters.AddWithValue("@divid", f["divid"]);
cmd.Parameters.AddWithValue("@depoid", f["depoid"]);
cmd.Parameters.AddWithValue("@rvnumber", f["rvnumber"]);
cmd.Parameters.AddWithValue("@ponumber", f["ponumber"]);
cmd.Parameters.AddWithValue("@invoiceno", f["invoiceno"]);
cmd.Parameters.AddWithValue("@userid", f["userid"]);
cmd.Parameters.AddWithValue("@status", f["status"]);
cmd.Parameters.AddWithValue("@enteredon", f["enteredon"]);

            int ab = cmd.ExecuteNonQuery();
            con.Close();
            if (ab > 0) return true; else return false;
        }

        public Boolean DeleteRecord(int id,ref String st) {
            String Sqltext = "delete from Voucher_master where id=@id";
            SqlConnection con = DBClass.mycon(); 
            SqlCommand cmd = new SqlCommand(Sqltext, con);
            cmd.Parameters.AddWithValue("@id", id);
            try
            {
                int ab = cmd.ExecuteNonQuery();
                con.Close();
                if (ab > 0) return true; else return false;
            }
            catch (SqlException ex)
            {
                if(ex.Number==547)
                st="Related Records Exist In Other Tables. Delete Records from Other Tables Fist to Delete This Record";
				con.Close();
                return false;
            }
            catch (Exception ex) {
				st=ex.Message;
				con.Close();
                return false;
            }
        }

        public Boolean SaveMyVoucher(FormCollection f, DataTable udt, DataTable cdt, ref string trid, DateTime finst, DateTime finend,int divisionid,String divname,int secid,int uid,Double payamt,String accno)
        {
            /*  begin transaction - generate voucher no - insert record into table voucher master - 
             *  insert record into transaction table - insert record into party ledger if party selected 
             *  update table autoidgen with the new voucher no 
             */

            
            SqlTransaction tr = null;
            GenHelper g = new GenHelper();

            trid = g.GetNewVoucherNo(transtype, finst, finend, divisionid,divname);

            SqlConnection con = DBClass.mycon();
            tr = con.BeginTransaction();
            try
            {
            String Sql = "insert into voucher_master (id,voucherno,vodate,billtypeid,rvnumber,ponumber,invoiceno,sectionid,divid,userid,enteredon,partyid,trtypeid,rvdate, podate,invdate,dueamount,accountnumber) ";
            Sql += "values(@id,@voucherno,@vodate,@billtypeid,@rvnumber,@ponumber,@invoiceno,@sectionid,@divid,@userid,@enteredon,@partyid,@trtypeid,@rvdate, @podate,@invdate,@topayamt,@accountnumber)";
                //SqlConnection mcon = DBClass.mycon();
               // String Sql = "insert into voucher_master (id,voucherno,vodate,billtypeid,partyid)";
                //Sql += " values(@id,@voucherno,@vodate,@billtypeid,@partyid)";

                SqlCommand cmd = new SqlCommand(Sql, con);
                cmd.Parameters.AddWithValue("@id", g.GetNextId("voucher_master", "id"));
                cmd.Parameters.AddWithValue("@voucherno", trid);
                cmd.Parameters.AddWithValue("@vodate", f["vdate"]);
                cmd.Parameters.AddWithValue("@billtypeid", f["billtypeid"]);
                cmd.Parameters.AddWithValue("@rvnumber", f["rvno"]);
                cmd.Parameters.AddWithValue("@ponumber", f["pono"]);
                cmd.Parameters.AddWithValue("@invoiceno", f["invoice"]);
                cmd.Parameters.AddWithValue("@rvdate", f["rvdate"]);
                cmd.Parameters.AddWithValue("@podate", f["podate"]);
                cmd.Parameters.AddWithValue("@invdate", f["invdate"]);
                cmd.Parameters.AddWithValue("@partyid", f["partyid"]);
                cmd.Parameters.AddWithValue("@accountnumber", accno);
                cmd.Parameters.AddWithValue("@topayamt", payamt.ToString());
                cmd.Parameters.AddWithValue("@trtypeid",this.transtype);
                cmd.Parameters.AddWithValue("@sectionid", secid);
                cmd.Parameters.AddWithValue("@userid", uid);
                cmd.Parameters.AddWithValue("@divid", divisionid);
                cmd.Parameters.AddWithValue("@enteredon",DateTime.Today) ;
                cmd.Transaction = tr;
                int a = cmd.ExecuteNonQuery();

                Int32 newid=g.GetNextId("accounttrans", "id");
                String Trqry = "insert into accounttrans (id,trtype,voucherno,accno,trdate,narration,dramount,cramount,partyid,divid,sectionid,userid,enteredon) ";
                Trqry += " values(@id,@trtype,@voucherno,@accno,@trdate,@narration,@dramount,@cramount,@partyid,@divid,@sectionid,@userid,@enteredon)";
                for(int i =0;i<cdt.Rows.Count;i++){
                    if ( Convert.ToInt32( cdt.Rows[i]["cramount"] )> 0 || Convert.ToInt32(cdt.Rows[i]["dramount"] )> 0)
                    {
                        SqlCommand cmdtr = new SqlCommand(Trqry, con);

                        cmdtr.Parameters.AddWithValue("@id", newid);
                        cmdtr.Parameters.AddWithValue("@trtype", this.transtype);
                        //cmdtr.Parameters.AddWithValue(@transid,
                        cmdtr.Parameters.AddWithValue("@voucherno", trid);
                        cmdtr.Parameters.AddWithValue("@accno", cdt.Rows[i]["accountnumber"].ToString());
                        cmdtr.Parameters.AddWithValue("@trdate", f["vdate"]);
                        cmdtr.Parameters.AddWithValue("@narration", "My Narration");
                        cmdtr.Parameters.AddWithValue("@dramount", cdt.Rows[i]["dramount"].ToString());
                        cmdtr.Parameters.AddWithValue("@cramount", cdt.Rows[i]["cramount"].ToString());
                        cmdtr.Parameters.AddWithValue("@partyid", f["partyid"]);
                        cmdtr.Parameters.AddWithValue("@divid", divisionid);
                        cmdtr.Parameters.AddWithValue("@sectionid", secid);
                        cmdtr.Parameters.AddWithValue("@userid", userid);
                        cmdtr.Parameters.AddWithValue("@enteredon", DateTime.Today);
                        //cmdtr.Parameters.AddWithValue("@depoid",
                        cmdtr.Transaction = tr;
                        cmdtr.ExecuteNonQuery();
                        newid++;
                    }
                }
                String PrtySql="insert into party_ledger (partyid,id,divid,dbamount,cramount,trdate,narration,voucherno) ";
                PrtySql+=" values(@partyid,@id,@divid,@dbamount,@cramount,@trdate,@narration,@voucherno)";
                SqlCommand cmdprty = new SqlCommand(PrtySql, con);
                Int32 prtytrid = g.GetNextId("party_ledger", "id");
                for (int i = 0; i < cdt.Rows.Count; i++)
                {
                    if (Convert.ToInt32(cdt.Rows[i]["dramount"]) > 0 && Convert.ToInt32(cdt.Rows[i]["cramount"]) == 0)
                    {
                        
                        cmdprty.Parameters.AddWithValue("@partyid", f["partyid"]);
                        cmdprty.Parameters.AddWithValue("@id", prtytrid);
                        cmdprty.Parameters.AddWithValue("divid", divisionid);
                        cmdprty.Parameters.AddWithValue("@dbamount", cdt.Rows[i]["dramount"].ToString());
                        cmdprty.Parameters.AddWithValue("@cramount", cdt.Rows[i]["cramount"].ToString());
                        cmdprty.Parameters.AddWithValue("@trdate", g.SqlDate(f["vdate"]));
                        cmdprty.Parameters.AddWithValue("@narration", "To Narration");
                        cmdprty.Parameters.AddWithValue("@voucherno", trid);
                        cmdprty.Transaction = tr;
                        cmdprty.ExecuteNonQuery();
                        newid++;
                       
                    }
                }
                String AutoQry = "update autoidgen set radtvoc=radtvoc+1 where divid=" + divisionid.ToString();
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

        public Boolean UpdateMyVoucher(FormCollection f, DataTable udt, DataTable cdt, ref string trid, DateTime finst, DateTime finend, int divisionid, String divname, int secid, int uid) {
            SqlTransaction tr = null;
            GenHelper g = new GenHelper();

            Int32 vid = g.GetNextId("voucher_master", "id");
            Int32 newid = g.GetNextId("accounttrans", "id");
            Int32 prtytrid = g.GetNextId("party_ledger", "id");

            SqlConnection con = DBClass.mycon();
            tr = con.BeginTransaction();
            try
            {
                String Q1="delete from voucher_master where voucherno='" + f["voucherno"] + "'";
                String Q2 = "delete from accounttrans where voucherno='" + f["voucherno"] + "'";
                String Q3 = "delete from party_ledger where voucherno='" + f["voucherno"] + "'";
                SqlCommand c1 = new SqlCommand(Q1, con);
                c1.Transaction = tr;
                c1.ExecuteNonQuery();
                SqlCommand c2 = new SqlCommand(Q2, con);
                c2.Transaction = tr;
                c2.ExecuteNonQuery();
                SqlCommand c3 = new SqlCommand(Q3, con);
                c3.Transaction = tr;
                c3.ExecuteNonQuery();
                String Sql = "insert into voucher_master (id,voucherno,vodate,billtypeid,rvnumber,ponumber,invoiceno,sectionid,divid,userid,enteredon,partyid,trtypeid,rvdate, podate,invdate) ";
                Sql += "values(@id,@voucherno,@vodate,@billtypeid,@rvnumber,@ponumber,@invoiceno,@sectionid,@divid,@userid,@enteredon,@partyid,@trtypeid,@rvdate, @podate,@invdate)";
                //SqlConnection mcon = DBClass.mycon();
                // String Sql = "insert into voucher_master (id,voucherno,vodate,billtypeid,partyid)";
                //Sql += " values(@id,@voucherno,@vodate,@billtypeid,@partyid)";

                SqlCommand cmd = new SqlCommand(Sql, con);
                cmd.Parameters.AddWithValue("@id",vid);
                cmd.Parameters.AddWithValue("@voucherno", f["voucherno"]);
                cmd.Parameters.AddWithValue("@vodate", g.SqlDate(f["vdate"]));
                cmd.Parameters.AddWithValue("@billtypeid", f["billtypeid"]);
                cmd.Parameters.AddWithValue("@rvnumber", f["rvno"]);
                cmd.Parameters.AddWithValue("@ponumber", f["pono"]);
                cmd.Parameters.AddWithValue("@invoiceno", f["invoice"]);
                cmd.Parameters.AddWithValue("@rvdate",  g.SqlDate(f["rvdate"]));
                cmd.Parameters.AddWithValue("@podate",  g.SqlDate(f["podate"]));
                cmd.Parameters.AddWithValue("@invdate",  g.SqlDate(f["invdate"]));
                cmd.Parameters.AddWithValue("@partyid", f["partyid"]);
                cmd.Parameters.AddWithValue("@trtypeid", this.transtype);
                cmd.Parameters.AddWithValue("@sectionid", secid);
                cmd.Parameters.AddWithValue("@userid", uid);
                cmd.Parameters.AddWithValue("@divid", divisionid);
                cmd.Parameters.AddWithValue("@enteredon", DateTime.Today);
                cmd.Transaction = tr;
                int a = cmd.ExecuteNonQuery();

                
                String Trqry = "insert into accounttrans (id,trtype,voucherno,accno,trdate,narration,dramount,cramount,partyid,divid,sectionid,userid,enteredon) ";
                Trqry += " values(@id,@trtype,@voucherno,@accno,@trdate,@narration,@dramount,@cramount,@partyid,@divid,@sectionid,@userid,@enteredon)";
                for (int i = 0; i < cdt.Rows.Count; i++)
                {
                    if (Convert.ToInt32(cdt.Rows[i]["cramount"]) > 0 || Convert.ToInt32(cdt.Rows[i]["dramount"]) > 0)
                    {
                        SqlCommand cmdtr = new SqlCommand(Trqry, con);

                        cmdtr.Parameters.AddWithValue("@id", newid);
                        cmdtr.Parameters.AddWithValue("@trtype", this.transtype);
                        //cmdtr.Parameters.AddWithValue(@transid,
                        cmdtr.Parameters.AddWithValue("@voucherno", f["voucherno"]);
                        cmdtr.Parameters.AddWithValue("@accno", cdt.Rows[i]["accountnumber"].ToString());
                        cmdtr.Parameters.AddWithValue("@trdate",  g.SqlDate(f["vdate"]));
                        cmdtr.Parameters.AddWithValue("@narration", "My Narration");
                        cmdtr.Parameters.AddWithValue("@dramount", cdt.Rows[i]["dramount"].ToString());
                        cmdtr.Parameters.AddWithValue("@cramount", cdt.Rows[i]["cramount"].ToString());
                        cmdtr.Parameters.AddWithValue("@partyid", f["partyid"]);
                        cmdtr.Parameters.AddWithValue("@divid", divisionid);
                        cmdtr.Parameters.AddWithValue("@sectionid", secid);
                        cmdtr.Parameters.AddWithValue("@userid", userid);
                        cmdtr.Parameters.AddWithValue("@enteredon", DateTime.Today);
                        //cmdtr.Parameters.AddWithValue("@depoid",
                        cmdtr.Transaction = tr;
                        cmdtr.ExecuteNonQuery();
                        newid++;
                    }
                }
                String PrtySql = "insert into party_ledger (partyid,id,divid,dbamount,cramount,trdate,narration,voucherno) ";
                PrtySql += " values(@partyid,@id,@divid,@dbamount,@cramount,@trdate,@narration,@voucherno)";
                SqlCommand cmdprty = new SqlCommand(PrtySql, con);
                
                for (int i = 0; i < cdt.Rows.Count; i++)
                {
                    if (Convert.ToInt32(cdt.Rows[i]["dramount"]) > 0 && Convert.ToInt32(cdt.Rows[i]["cramount"]) == 0)
                    {

                        cmdprty.Parameters.AddWithValue("@partyid", f["partyid"]);
                        cmdprty.Parameters.AddWithValue("@id", prtytrid);
                        cmdprty.Parameters.AddWithValue("divid", divisionid);
                        cmdprty.Parameters.AddWithValue("@dbamount", cdt.Rows[i]["dramount"].ToString());
                        cmdprty.Parameters.AddWithValue("@cramount", cdt.Rows[i]["cramount"].ToString());
                        cmdprty.Parameters.AddWithValue("@trdate", g.SqlDate(f["vdate"]));
                        cmdprty.Parameters.AddWithValue("@narration", "To Narration");
                        cmdprty.Parameters.AddWithValue("@voucherno", f["voucherno"]);
                        cmdprty.Transaction = tr;
                        cmdprty.ExecuteNonQuery();
                        newid++;

                    }
                }
                
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
    }
}