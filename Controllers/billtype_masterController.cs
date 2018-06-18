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
    public class Billtype_masterController : Controller
    {
		public int PerPageRec=4;
        public ActionResult Index()
        {
            return Redirect("Billtype_master/Billtype_masterlist?page=1");
        }
        public ActionResult Billtype_masterlist()
        { 	
			int pageno = Convert.ToInt16(Request["page"]);
			GenHelper.pager gen = new GenHelper.pager();
            Billtype_master obj = new Billtype_master();
            DataSet ds = new DataSet();
            DataTable dt = obj.GetOrderedRecords(pageno, PerPageRec, "billtypeid desc");
            Int32 cnt = obj.GetOrderedRecordsCount("billtypeid desc");
			gen.PagedList(pageno, cnt,PerPageRec,"Billtype_master","Billtype_masterlist");
			ViewBag.pageLinks=gen.PageNos;
			ViewData["TableHeading"] = "List Of Billtype_master";
            ViewData["addlink"]="Billtype_masterAdd";
            dt.TableName = "Billtype_master";
            ds.Tables.Add(dt);
			return View("listBilltype_master",ds);
        }
		
		public ActionResult Billtype_masterAdd()
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Billtype_master where billtypeid=0");
			dt.Rows.Add(dt.NewRow());
			dt.TableName = "Billtype_master";
            ds.Tables.Add(dt);
            			DataTable dtsection_master = DBClass.GetData("select sectionid,sectionname from section_master");
			ViewBag.section_master = new SelectList(dtsection_master.AsDataView(), "sectionid", "sectionname", dt.Rows[0]["sectionid"]);
						DataTable dtdiv_master = DBClass.GetData("select divid,divname from div_master");
			ViewBag.div_master = new SelectList(dtdiv_master.AsDataView(), "divid", "divname", dt.Rows[0]["divid"]);
			
            ViewBag.DialogTitle="Add Billtype_master";
            return View("addBilltype_master",ds);
        }
		
		public ActionResult Billtype_masterEdit(Int32 Id)
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Billtype_master where billtypeid="+Id);
			dt.TableName = "Billtype_master";
             ds.Tables.Add(dt);
            			DataTable dtsection_master = DBClass.GetData("select sectionid,sectionname from section_master");
			ViewBag.section_master = new SelectList(dtsection_master.AsDataView(), "sectionid", "sectionname", dt.Rows[0]["sectionid"]);
						DataTable dtdiv_master = DBClass.GetData("select divid,divname from div_master");
			ViewBag.div_master = new SelectList(dtdiv_master.AsDataView(), "divid", "divname", dt.Rows[0]["divid"]);
			
            ViewBag.DialogTitle="Edit Billtype_master";
            return View("addBilltype_master",ds);
        }
		
		public ActionResult Save(FormCollection fc){
            if (fc["billtypeid"] == null || fc["billtypeid"] == "")
            {
                Billtype_master t = new Billtype_master();
                Boolean res = t.InsertBilltype_master(fc);
                if (res) return Content("Record Inserted Successfully"); else return Content("Error Adding Record Try Again");
            }
            else 
            {
                Billtype_master t = new Billtype_master();
                Boolean res = t.UpdateBilltype_master(fc);
                if (res) return Content("Record Edited Successfully"); else return Content("Error Editing Record Try Again");
            }
            //return PartialView("SaveRec");
        }

        public ActionResult Billtype_masterdelete(int id)
        {
            Billtype_master t = new Billtype_master();
            Boolean res = t.DeleteRecord(id);
            if (res) return Content("Record Deleted Successfully"); else return Content("Error Deleting Record Try Again");
        }
        public JsonResult IsAvailable(String billtypename)
        {
            Billtype_master d = new Billtype_master();
            return Json(d.IsAvailable(billtypename), JsonRequestBehavior.AllowGet);

        }
    }
}
