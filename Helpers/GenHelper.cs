using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data ;
using System.IO;

namespace ProfitPlus.Helpers
{
    public class GenHelper
    {
        public class paytype
        {
          
            public String payid { get; set; }
            public String paydesc { get; set; }

            public paytype(String Id, String Desc)
            {
                payid = Id; paydesc = Desc;
            }
        }
        public class pager
        {
            public int Pageno = 1;
            public int StartNo = 1;
            public int Endno;
            public int TotalRec;
            public int RecPerPage;
            public int TotalPages;
            public String PageNos;

            public void PagedList(int pgno, int totalrows, int perpage, String cntrl, String action)
            {
                //pager mypager = new pager();
                Pageno = pgno;
                RecPerPage = perpage;
                TotalRec = totalrows;
                double mypages = (double)totalrows / (double)perpage;
                TotalPages = (int)Math.Ceiling(mypages);
                StartNo = (pgno-1) * perpage + 1;
                Endno = StartNo + perpage - 1;
                int startpageno = 1;
                int endpageno = TotalPages;
                if (TotalPages >= 10)
                {
                    startpageno = pgno - 5;
                    endpageno = pgno + 5;
                    if (startpageno < 0)
                    {
                        startpageno = 1;
                        endpageno = startpageno + 10 - 1;
                    }
                    if (endpageno > TotalPages)
                    {
                        endpageno = TotalPages;
                        startpageno = TotalPages - 10;
                    }
                }

                if (TotalPages > 1)
                {
                    PageNos += "<ul class='pagination'>";
                    for (int i = startpageno; i <= endpageno; i++)
                    {
                        PageNos += "<li><a href='/" + cntrl + "/" + action + "?page=" + (i) + "'>" + (i) + "</a><li>  ";

                    }
                    PageNos += "</ul>";
                }
                return;
            }
        }

        public Int32 GetNextId(String Tbl,String KeyField) { 
            String Qry ="select max("+KeyField+") from "+Tbl;
            DataTable dt= DBClass.GetAllRecords(Qry);
            if (dt.Rows[0].ItemArray[0].ToString() == "") return 1;
            if (Convert.ToInt32(dt.Rows[0].ItemArray[0]) > 0)
                return Convert.ToInt32(dt.Rows[0].ItemArray[0]) + 1;
            else
                return 1;
        }

        public void logerror(Exception e)
        {
            String Errfile=HttpContext.Current.Server.MapPath("~/Errorlogs")+"\\errors.txt";
            String Err="------------------------------------------------------------------------------";
            Err =string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            Err+=Environment.NewLine;
            Err+=e.Message ;
            Err+=Environment.NewLine ;
            Err+="------------------------------------------------------------------------------";
            File.AppendAllText(Errfile,Err);
        }

        public Boolean HasPermission() {
            String uname=(String) HttpContext.Current.Session["UserName"] ;
            if (uname== null)
                return false;
            else
                return true;
        }
        public String GetNewVoucherNo(int vtype,DateTime finstart,DateTime finend,int divid,String divname)
        {
            int vno = 0;
            String prefix = "";
            String fld = "";
            String Fstart=Getyear(finstart);
            String Fend = Getyear(finend);
            switch (vtype)
            {
                case 1:
                    {
                        fld = "Cadtvoc"; prefix = "Cadtvoc/" + Fstart + "-" + Fend + "/"; break;
                    
                    }
                case 2:
                    {
                        fld = "Padtvoc"; prefix = "Padtvoc/" + Fstart + "-" + Fend + "/"; break;
                    }
                case 3:
                    {
                        fld = "Sadtvoc"; prefix = "Sadtvoc/" + Fstart + "-" + Fend + "/"; break;
                    }
                case 4:
                    {
                        fld = "Radtvoc"; prefix = "Radtvoc/" + Fstart + "-" + Fend + "/"; break;
                    }
                case 5:
                    {
                        fld = "Adjvoc"; prefix = "Adjvoc/" + Fstart + "-" + Fend + "/"; break;
                    
                    }
                case 6:
                    {
                        fld = "Cadtbpo"; prefix = "Cadtbpo/" + Fstart + "-" + Fend + "/"; break;
                    }
                case 7:
                    {
                        fld = "Padtbpo"; prefix = "Padtbpo/" + Fstart + "-" + Fend + "/"; break;
                    }
                case 8:
                    {
                        fld = "Radtbpo"; prefix = "Radtbpo/" + Fstart + "-" + Fend + "/"; break;
                    }
                case 9:
                    {
                        fld = "Sadtbpo"; prefix = "Sadtbpo/" + Fstart + "-" + Fend + "/"; break;

                    }
                case 10:
                    {
                        fld = "cash"; prefix = "cash/" + Fstart + "-" + Fend + "/"; break;

                    }
            }
            String Sql = "select " + fld + " from autoidgen where divid=" + divid;
            vno = Convert.ToInt32(DBClass.GetData(Sql).Rows[0][fld]) + 1;
            return prefix + divname.Substring(0, 3) + "/"+FixedInt(vno);
        
        }
        public String FixedInt(int num)
        {
            String textnum = "";
            if (num < 10) textnum = "00000" + num.ToString();
            if (num > 9 && num < 100) textnum = "0000" + num.ToString();
            if (num > 99 && num < 1000) textnum = "000" + num.ToString();
            if (num > 999 && num < 10000) textnum = "00" + num.ToString();
            if (num > 10000 && num < 100000) textnum = "0" + num.ToString();
            if (num > 99999 && num < 1000000) textnum = num.ToString();
            return textnum;
        
        }
        public String Getyear(DateTime mydate)
        {
            String dt = mydate.ToString("G");
            DateTime yd = DateTime.Parse(dt);
            String yy = yd.Year.ToString();
            return yy;
        }

        public String SqlDate(String dt)
        {
            DateTime mydt = DateTime.Parse(dt);
            return mydt.ToString("yyyy-MM-dd");

        }

        public String DispDate(String dt){
            DateTime mydt = DateTime.Parse(dt);
            return mydt.ToString("dd-MM-yyyy");
        }


        public DataTable FindVouchers(String voctype,Int32 divid){
            
            String qry="select dbo.voucher_master.voucherno,dbo.voucher_master.vodate,dbo.voucher_master.rvnumber,dbo.voucher_master.ponumber,dbo.voucher_master.invoiceno,dbo.party_master.partyname FROM dbo.voucher_master INNER JOIN dbo.party_master ON dbo.voucher_master.partyid = dbo.party_master.partyid ";
            qry += " where voucher_master.sectionid='" + voctype + "' and dbo.voucher_master.divid=" + divid.ToString();
            return DBClass.GetData(qry);
        }

        public DataTable FindBpo(String voctype, Int32 divid)
        {

            String qry = "SELECT dbo.bpo_master.bponumber,dbo.bpo_master.bpodate,dbo.bpo_master.typeofbpo FROM dbo.bpo_master ";
            qry += " where  dbo.bpo_master.divid=" + divid.ToString();
            return DBClass.GetData(qry);
        }

        public Double GetPartyBalance(int partyid) {
            DataTable dt= DBClass.GetData("select sum(cramount)-sum(dbamount) as balance from party_ledger where partyid="+partyid.ToString());
            return(Convert.ToDouble( dt.Rows[0]["balance"]));
        
        }

        public DataSet GetPartyLedger(int partyid)
        {
            DataSet ds = new DataSet();
            DataTable mdt = DBClass.GetData("select * from party_master where id=" + partyid.ToString());
            DataTable tdt = DBClass.GetData("SELECT id,dbamount,cramount,trdate,narration FROM party_ledger  where partyid=" + partyid.ToString());
            ds.Tables.Add(mdt); ds.Tables.Add(tdt);
            return ds;
        }

        public List<paytype> Acclist()
        {
            List<paytype> mlist = new List<paytype>();
            paytype p1 = new paytype("1", "Cash");
            paytype p2 = new paytype("2", "Cheque");
            paytype p3 = new paytype("3", "Net Banking");
            mlist.Add(p1); mlist.Add(p2); mlist.Add(p3);
            return mlist;
        }
    }
}