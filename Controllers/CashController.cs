using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using ProfitPlus.Helpers;
using ProfitPlus.Models;
using System.Web.Mvc;

namespace ProfitPlus.Controllers
{
    public class CashController : Controller
    {
        //
        // GET: /Cash/

        public ActionResult Index()
        {
            ViewBag.vtype = 10;
            return View("cash");
        }

        public ActionResult findbpo()
        {
            GenHelper g = new GenHelper();
            DataTable dt = g.FindBpo(Request["vtype"], Convert.ToInt32(Session["divid"]));
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            return PartialView("_findvoc", ds);
        }

        public ActionResult GetCashPay(FormCollection fc) {
            Cash ch = new Cash();
            String BpoNum = fc["txtvocno"];
            DataTable dt = ch.GetBpos(BpoNum,Convert.ToInt32(Session["divid"]));
            foreach (DataColumn col in dt.Columns)
                col.ReadOnly = false;
            Session["bpopayment"] = dt;
            return View("bpopay",dt);

        }
        public ActionResult EditBpo(Int32 id)
        {
            GenHelper g = new GenHelper();
            Cash cs = new Cash();

            DataTable bac = cs.GetBankAccounts(Convert.ToInt32(Session["divid"]));
            ViewBag.bacclist = new SelectList(bac.AsDataView(), "accountnumber", "bankname");
            SelectList lst = new SelectList(g.Acclist(),"payid","paydesc");
            ViewBag.aclist = lst;
            DataTable dt = cs.FindBpo(id);

            return View("EditBpo", dt);
        }

        public ActionResult savecashdetail(FormCollection fc)
        {
            DataTable dt = (DataTable)Session["bpopayment"];
            int id = Convert.ToInt32(fc["id"]);
            DataRow dr = dt.Select("id=" + id.ToString()).FirstOrDefault();
            dr["paidamt"] = fc["payamount"];
            dr["chequedate"] = fc["chequedate"];
            dr["paytype"] = fc["paytype"];
            dr["chequeno"] = fc["chequeno"];
            dr["accountno"] = fc["bankacc"];
            dr["dramount"] = Convert.ToInt32(dr["dramount"]) - Convert.ToInt32(fc["payamount"]);
            //return Content("Hello");
            return View("bpopay", dt);
        }

        public ActionResult savecash(FormCollection fc)
        {
            string trid = "";
            DateTime fst = (DateTime)Session["finstart"];
            DateTime fend = (DateTime)Session["finend"];
            int mydivid = Convert.ToInt32(Session["divid"]);
            String mydivname = Session["divname"].ToString();
            Users user = new Users();
            DataTable userdt = user.GetUserRecord(Convert.ToInt32(Session["Userid"]));
            DataTable cartdt = (DataTable)Session["bpopayment"];
            Cash csh = new Cash();
            csh.SaveCash(fc,userdt,cartdt,fst,fend,Convert.ToInt32(Session["sectionid"]),mydivid,mydivname,Convert.ToInt32(Session["divid"]),ref trid);
            return Content(trid);
        }
    }
}
