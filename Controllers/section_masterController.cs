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
    public class Section_masterController : Controller
    {
		public int PerPageRec=4;
        public ActionResult Index()
        {
            return Redirect("Section_master/Section_masterlist?page=1");
        }
        public ActionResult Section_masterlist()
        { 	
			int pageno = Convert.ToInt16(Request["page"]);
			GenHelper.pager gen = new GenHelper.pager();
            Section_master obj = new Section_master();
            DataSet ds = new DataSet();
            DataTable dt = obj.GetOrderedRecords(pageno, PerPageRec, "sectionid desc");
            Int32 cnt = obj.GetOrderedRecordsCount("sectionid desc");
			gen.PagedList(pageno, cnt,PerPageRec,"Section_master","Section_masterlist");
			ViewBag.pageLinks=gen.PageNos;
			ViewData["TableHeading"] = "List Of Section_master";
            ViewData["addlink"]="Section_masterAdd";
            dt.TableName = "Section_master";
            ds.Tables.Add(dt);
			return View("listSection_master",ds);
        }
		
		public ActionResult Section_masterAdd()
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Section_master where sectionid=0");
			dt.Rows.Add(dt.NewRow());
			dt.TableName = "Section_master";
            ds.Tables.Add(dt);
            
            ViewBag.DialogTitle="Add Section_master";
            return View("addSection_master",ds);
        }
		
		public ActionResult Section_masterEdit(Int32 Id)
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Section_master where sectionid="+Id);
			dt.TableName = "Section_master";
             ds.Tables.Add(dt);
            
            ViewBag.DialogTitle="Edit Section_master";
            return View("addSection_master",ds);
        }
		
		public ActionResult Save(FormCollection fc){
            if (fc["sectionid"] == null || fc["sectionid"] == "")
            {
                Section_master t = new Section_master();
                Boolean res = t.InsertSection_master(fc);
                if (res) return Content("Record Inserted Successfully"); else return Content("Error Adding Record Try Again");
            }
            else 
            {
                Section_master t = new Section_master();
                Boolean res = t.UpdateSection_master(fc);
                if (res) return Content("Record Edited Successfully"); else return Content("Error Editing Record Try Again");
            }
            //return PartialView("SaveRec");
        }

        public ActionResult Section_masterdelete(int id)
        {
            Section_master t = new Section_master();
            String st = "";
            Boolean res = t.DeleteRecord(id,ref st);
            if (res) return Content("Record Deleted Successfully"); else return Content(st+"\n"+"Error Deleting Record Try Again");
        }
        public JsonResult IsAvailable(String sectionname)
        {
            Section_master d = new Section_master();
            return Json(d.IsAvailable(sectionname), JsonRequestBehavior.AllowGet);

        }
    }
}
