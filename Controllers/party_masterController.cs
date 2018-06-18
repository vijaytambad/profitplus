using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ProfitPlus.Models;
using ProfitPlus.Helpers;

namespace ProfitPlus.Controllers
{
    public class Party_masterController : Controller
    {
        public int PerPageRec = 6;
        public ActionResult Index()
        {
            return Redirect("Party_master/Party_masterlist?page=1");
        }
       
        public ActionResult Party_masterlist()
        {
            int pageno = Convert.ToInt16(Request["page"]);
            GenHelper.pager gen = new GenHelper.pager();
            Party_master obj = new Party_master();
            DataSet ds = new DataSet();
            DataTable dt = obj.GetOrderedRecords(pageno, PerPageRec, "partyid desc");
            Int32 cnt = obj.GetOrderedRecordsCount("partyid desc");
            gen.PagedList(pageno, cnt, PerPageRec, "party_master", "Party_masterlist");
            ViewBag.pageLinks = gen.PageNos;
            ViewData["TableHeading"] = "List Of Party_Master";
            ViewData["addlink"] = "Party_masterAdd";         
            dt.TableName = "Party_master";
            ds.Tables.Add(dt);
			return View("listParty_master",ds);
        }
		
		public ActionResult Party_masterAdd()
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Party_master where partyid=0");
			dt.Rows.Add(dt.NewRow());
			dt.TableName = "Party_master";
            ds.Tables.Add(dt);
            			DataTable dtdiv_master = DBClass.GetData("select divid,divname from div_master");
			ViewBag.div_master = new SelectList(dtdiv_master.AsDataView(), "divid", "divname", dt.Rows[0]["divid"]);
						DataTable dtsection_master = DBClass.GetData("select sectionid,sectionname from section_master");
			ViewBag.section_master = new SelectList(dtsection_master.AsDataView(), "sectionid", "sectionname", dt.Rows[0]["sectionid"]);
			
            ViewBag.DialogTitle="Add Party_master";
            return View("addParty_master",ds);
        }
		
		public ActionResult Party_masterEdit(Int32 Id)
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Party_master where partyid="+Id);
			dt.TableName = "Party_master";
             ds.Tables.Add(dt);
            			DataTable dtdiv_master = DBClass.GetData("select divid,divname from div_master");
			ViewBag.div_master = new SelectList(dtdiv_master.AsDataView(), "divid", "divname", dt.Rows[0]["divid"]);
						DataTable dtsection_master = DBClass.GetData("select sectionid,sectionname from section_master");
			ViewBag.section_master = new SelectList(dtsection_master.AsDataView(), "sectionid", "sectionname", dt.Rows[0]["sectionid"]);
			
            ViewBag.DialogTitle="Edit Party_master";
            return View("addParty_master",ds);
        }
		
		public ActionResult Save(FormCollection fc){
            if (fc["partyid"] == null || fc["partyid"] == "")
            {
                Party_master t = new Party_master();
                Boolean res = t.InsertParty_master(fc);
                if (res) return Content("Record Inserted Successfully"); else return Content("Error Adding Record Try Again");
            }
            else 
            {
                Party_master t = new Party_master();
                Boolean res = t.UpdateParty_master(fc);
                if (res) return Content("Record Edited Successfully"); else return Content("Error Editing Record Try Again");
            }
            //return PartialView("SaveRec");
        }

        public ActionResult Party_masterdelete(int id)
        {
            Party_master t = new Party_master();
            Boolean res = t.DeleteRecord(id);
            if (res) return Content("Record Deleted Successfully"); else return Content("Error Deleting Record Try Again");
        }
        public JsonResult IsAvailable(String partyname)
        {
            int divid = Convert.ToInt16(Session["divid"]);
            Party_master d = new Party_master();
            return Json(d.IsAvailable(partyname, divid), JsonRequestBehavior.AllowGet);

        }
    }
}
