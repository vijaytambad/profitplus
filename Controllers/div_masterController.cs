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
    public class Div_masterController : Controller
    {
		public int PerPageRec=4;
        public ActionResult Index()
        {
            return Redirect("Div_master/Div_masterlist?page=1");
        }
        public ActionResult Div_masterlist()
        { 	
			int pageno = Convert.ToInt16(Request["page"]);
			GenHelper.pager gen = new GenHelper.pager();
            Div_master obj = new Div_master();
            DataSet ds = new DataSet();
            DataTable dt = obj.GetOrderedRecords(pageno, PerPageRec, "divid desc");
            Int32 cnt = obj.GetOrderedRecordsCount("divid desc");
			gen.PagedList(pageno, cnt,PerPageRec,"Div_master","Div_masterlist");
			ViewBag.pageLinks=gen.PageNos;
			ViewData["TableHeading"] = "List Of Div_master";
            ViewData["addlink"]="Div_masterAdd";
            dt.TableName = "div_master";
            ds.Tables.Add(dt);
			return View("listDiv_master",ds);
        }
		
		public ActionResult Div_masterAdd()
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Div_master where divid=0");
			dt.Rows.Add(dt.NewRow());
			dt.TableName = "div_master";
            ds.Tables.Add(dt);
            
            ViewBag.DialogTitle="Add Div_master";
            return View("addDiv_master",ds);
        }
		
		public ActionResult Div_masterEdit(Int32 Id)
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Div_master where divid="+Id);
			dt.TableName = "div_master";
             ds.Tables.Add(dt);
            
            ViewBag.DialogTitle="Edit Div_master";
            return View("addDiv_master",ds);
        }
		
		public ActionResult Save(FormCollection fc){
            if (fc["divid"] == null || fc["divid"] == "")
            {
                Div_master t = new Div_master();
                Boolean res = t.InsertDiv_master(fc);
                if (res) return Content("Record Inserted Successfully"); else return Content("Error Adding Record Try Again");
            }
            else 
            {
                Div_master t = new Div_master();
                Boolean res = t.UpdateDiv_master(fc);
                if (res) return Content("Record Edited Successfully"); else return Content("Error Editing Record Try Again");
            }
            //return PartialView("SaveRec");
        }

        public ActionResult Div_masterdelete(int id)
        {
            Div_master t = new Div_master();
            Boolean res = t.DeleteRecord(id);
            if (res) return Content("Record Deleted Successfully"); else return Content("Error Deleting Record Try Again");
        }
        public JsonResult IsAvailable(String divname)
        {
            Div_master d = new Div_master();
            return Json(d.IsAvailable(divname), JsonRequestBehavior.AllowGet);

        }
    }
}
