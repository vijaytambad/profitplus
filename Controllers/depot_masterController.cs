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
    public class Depot_masterController : Controller
    {
		public int PerPageRec=4;
        public ActionResult Index()
        {
            return Redirect("Depot_master/Depot_masterlist?page=1");
        }
        public ActionResult Depot_masterlist()
        { 	
			int pageno = Convert.ToInt16(Request["page"]);
			GenHelper.pager gen = new GenHelper.pager();
            Depot_master obj = new Depot_master();
            DataSet ds = new DataSet();
            DataTable dt = obj.GetOrderedRecords(pageno, PerPageRec, "depotid desc");
            Int32 cnt = obj.GetOrderedRecordsCount("depotid desc");
			gen.PagedList(pageno, cnt,PerPageRec,"Depot_master","Depot_masterlist");
			ViewBag.pageLinks=gen.PageNos;
			ViewData["TableHeading"] = "List Of Depot_master";
            ViewData["addlink"]="Depot_masterAdd";
            dt.TableName = "depot_master";
            ds.Tables.Add(dt);
			return View("listDepot_master",ds);
        }
		
		public ActionResult Depot_masterAdd()
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Depot_master where depotid=0");
			dt.Rows.Add(dt.NewRow());
			dt.TableName = "depot_master";
            ds.Tables.Add(dt);
            			DataTable dtdiv_master = DBClass.GetData("select divid,divname from div_master");
			ViewBag.div_master = new SelectList(dtdiv_master.AsDataView(), "divid", "divname", dt.Rows[0]["divid"]);
			
            ViewBag.DialogTitle="Add Depot_master";
            return View("addDepot_master",ds);
        }
		
		public ActionResult Depot_masterEdit(Int32 Id)
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Depot_master where depotid="+Id);
			dt.TableName = "depot_master";
             ds.Tables.Add(dt);
            			DataTable dtdiv_master = DBClass.GetData("select divid,divname from div_master");
			ViewBag.div_master = new SelectList(dtdiv_master.AsDataView(), "divid", "divname", dt.Rows[0]["divid"]);
			
            ViewBag.DialogTitle="Edit Depot_master";
            return View("addDepot_master",ds);
        }
		
		public ActionResult Save(FormCollection fc){
            if (fc["depotid"] == null || fc["depotid"] == "")
            {
                Depot_master t = new Depot_master();
                Boolean res = t.InsertDepot_master(fc);
                if (res) return Content("Record Inserted Successfully"); else return Content("Error Adding Record Try Again");
            }
            else 
            {
                Depot_master t = new Depot_master();
                Boolean res = t.UpdateDepot_master(fc);
                if (res) return Content("Record Edited Successfully"); else return Content("Error Editing Record Try Again");
            }
            //return PartialView("SaveRec");
        }

        public ActionResult Depot_masterdelete(int id)
        {
            Depot_master t = new Depot_master();
            Boolean res = t.DeleteRecord(id);
            if (res) return Content("Record Deleted Successfully"); else return Content("Error Deleting Record Try Again");
        }
        public JsonResult IsAvailable(String depotname)
        {
            Depot_master d = new Depot_master();
            return Json(d.IsAvailable(depotname), JsonRequestBehavior.AllowGet);

        }
    }
}
