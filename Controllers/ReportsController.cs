using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProfitPlus.Helpers;
using ProfitPlus.Models;

namespace ProfitPlus.Controllers
{
    public class ReportsController : Controller
    {
        //
        // GET: /Reports/

        public ActionResult Index()
        {
            DataTable dt = DBClass.GetData("select * from reports");
            return View("Index",dt);
        }

        public ActionResult rpstatement() {

            return View("rpstatement");
        }

        public ActionResult printrp(FormCollection fc) {
            DataSet ds = new DataSet();
            String repname = "ReceiptPayment";
            
            Reports rs = new Reports(); GenHelper g = new GenHelper();
            float[] cols = new float[] { 40f, 220f, 60f, 60f};
            String path = Server.MapPath("/pdfs/receiptpayments.pdf");
            String[] rephead = new String[] { "N.EW.K.R.T.C Hubli", "Receipts & Payments From " + fc["stdate"] + " To " + fc["edate"] };
            String[] tblhead = new string[] {"Account Number","Account Name","Receipts","Payments" };
            DataTable dt = rs.GetRpData(fc["stdate"], fc["edate"]);
            ds.Tables.Add(dt);
            DataTable dt1=DBClass.GetData("select sum(cramount) receipts,sum(dramount) payments from accounttrans where trdate>='"+g.SqlDate(fc["stdate"])+"' and trdate<='"+g.SqlDate(fc["edate"])+"' and divid=1");
            ds.Tables.Add(dt1);
            pdf ph = new pdf(ds, cols, rephead, tblhead, path,repname);
            ph.GenerateReport();
            return File(path, "application/pdf");
            
        }
        public ActionResult TrialBalForm(FormCollection fc) {

            return View("trialbal");
        }
        public ActionResult TrialBal(FormCollection fc) {
            GenHelper g = new GenHelper();
            DataSet ds = new DataSet();
            String repname = "TrialBalance";
            String Tblname = "mytrialbal";
            DBClass.NonQuery("drop table "+Tblname);
            Reports rp=new Reports();
            String Mon = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(fc["mon"]));
            
            String Stdate = fc["year"]+"-"+ fc["mon"] + "-" +"01";
            DateTime sd = DateTime.Parse(Stdate);
            DateTime ed = sd.AddMonths(1); ed = ed.AddDays(-1);
            String edate = g.SqlDate(ed.ToString());
            DataTable trbal = rp.PrepareTraialBal(Tblname,Stdate,edate);
            String SumQry="select sum(receipts) as TotalCredits,sum(payments) as TotalDebits from "+Tblname;
            DataTable sumdt = DBClass.GetData(SumQry);
            ds.Tables.Add(trbal); ds.Tables.Add(sumdt);
            float[] cols = new float[] { 60f, 180f, 65f,65f, 65f,65f };
            String path = Server.MapPath("/pdfs/trialbalance.pdf");
            String[] rephead = new String[] { "N.EW.K.R.T.C Hubli", "Trial Balance For the Month of "+Mon+" "+fc["year"] };
            String[] tblhead = new string[] { "Account Number", "Account Name","Opening Balance", "Receipts", "Payments","Closing Balance" };
            pdf ph = new pdf(ds, cols, rephead, tblhead, path,repname,Tblname);
            ph.GenerateReport();
            //String DropQry = "drop table " + Tblname;
            //DBClass.NonQuery(DropQry);
            return File(path, "application/pdf");
        }

        public ActionResult pl() {
            Reports r = new Reports();
            GenHelper g = new GenHelper();
            DataSet ds = new DataSet();
            String repname = "placc";
            String Tblname = "mytrialbal";
            DataTable tb = r.PrepareTraialBal(Tblname,"2018-06-01","2018-06-30");
            DataTable pltab= r.GetPLAccount();
            String SumQry = "select (select sum(amount) from receipts ) as receipts,(select sum(amount) from payments) as payments";
            DataTable sumdt = DBClass.GetData(SumQry);
            ds.Tables.Add(pltab); ds.Tables.Add(sumdt);
            float[] cols = new float[] { 65f, 120f, 65f, 65f, 120f, 65f };
            String path = Server.MapPath("/pdfs/pandlaccount.pdf");
            String[] rephead = new String[] { "N.EW.K.R.T.C Hubli", "Trial Balance For the Year  " };
            String[] tblhead = new string[] { "Account Number", "Account Name", "Amount", "Account Number", "Account Name", "Amount" };

            pdf ph = new pdf(ds, cols, rephead, tblhead, path, repname, Tblname);
            ph.GenerateReport();
            return File(path, "application/pdf");
        }
    }
}
