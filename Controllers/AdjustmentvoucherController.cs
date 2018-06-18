using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using ProfitPlus.Models;
using ProfitPlus.Helpers;

namespace ProfitPlus.Controllers
{
    public class AdjustmentvoucherController : Controller
    {
        //
        // GET: /Adjustmentvoucher/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Voucher_masterAdd()
        {
            int sectionid = Convert.ToInt32(Session["sectionid"]);
            DataSet ds = new DataSet();
            DataTable dt = DBClass.GetAllRecords("select * from Voucher_master where id=0");
            dt.Rows.Add(dt.NewRow());
            dt.TableName = "Adjustmentvoucher";
            ds.Tables.Add(dt);
           
            ViewBag.DialogTitle = "Add Adjustment voucher";
            return View("addAdjustment", ds);
        }
    }
}
