using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProfitPlus.Models;
using System.Web.Mvc;
using ProfitPlus.Helpers;

namespace ProfitPlus.Controllers
{
    public class CadtBpoController : Controller
    {
        //
        // GET: /CadtBpo/

        public ActionResult Index()
        {
            ViewBag.tableheading = "List of Pending BPO";
            CadtBpo cb = new CadtBpo();
            DataTable dt = cb.GetPartyTrans(1);
            DataSet ds = new DataSet();
            ViewBag.bpotype = cb.bpotype;
            
            ds.Tables.Add(dt);
            return View("search", ds);
        }

        [HttpPost]
        public ActionResult SaveBpo()
        {
            GenHelper g = new GenHelper();
            Users user = new Users();
            DataTable userdt = user.GetUserRecord(Convert.ToInt32( Session["Userid"]));
            DateTime fst=(DateTime)Session["finstart"];
            DateTime fend=(DateTime)Session["finend"];
            Int32 divid = Convert.ToInt32(Session["divid"]);
            String divname = Session["divname"].ToString();
            Int32 sectionid = Convert.ToInt32(Session["sectionid"]);
            CadtBpo cb = new CadtBpo();
            String test = Request.QueryString["chkval"];
            String bpdate = Request.QueryString["bpodate"];
            String bpodate = g.SqlDate(bpdate);
            if(cb.SaveBpo(test,bpodate,fst,fend,divid,divname,sectionid,Convert.ToInt32(userdt.Rows[0]["id"])))
                return Content("Transaction Successfull");
            else
                return Content("Transaction Un Succeccful.  Try Again Later");
        }
        
    }
}
