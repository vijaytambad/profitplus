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
    public class Billtype_detailsController : Controller
    {
		public int PerPageRec=4;
        public ActionResult Index()
        {
            return Redirect("Billtype_details/Billtype_detailslist?page=1");
        }
        public ActionResult Billtype_detailslist(int id)
        { 	
			int pageno = Convert.ToInt16(Request["page"]);
			GenHelper.pager gen = new GenHelper.pager();
            Billtype_details obj = new Billtype_details();
            DataSet ds = new DataSet();
            DataTable dt = obj.GetOrderedRecords(pageno, PerPageRec,id, "id desc");
            Int32 cnt = obj.GetOrderedRecordsCount("id desc");
			gen.PagedList(pageno, cnt,PerPageRec,"Billtype_details","Billtype_detailslist");
			ViewBag.pageLinks=gen.PageNos;
			ViewData["TableHeading"] = "List Of Billtype_details";
            ViewData["addlink"]="Billtype_detailsAdd";
            dt.TableName = "Billtype_details";
            ds.Tables.Add(dt);
			return View("listBilltype_details",ds);
        }
		
		public ActionResult Billtype_detailsAdd()
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Billtype_details where id=0");
			dt.Rows.Add(dt.NewRow());
			dt.TableName = "Billtype_details";
            ds.Tables.Add(dt);
            DataTable dtbilltype_master = DBClass.GetData("select billtypeid,billtypename from billtype_master");
			ViewBag.billtype_master = new SelectList(dtbilltype_master.AsDataView(), "billtypeid", "billtypename", dt.Rows[0]["billtypeid"]);
            DataTable dtaccount_master = DBClass.GetData("select accountnumber,accountname from account_master");
            ViewBag.account_master = new SelectList(dtaccount_master.AsDataView(), "accountnumber", "accountname", dt.Rows[0]["accountnumber"]);
            ViewBag.DialogTitle="Add Billtype_details";
            return View("addBilltype_details",ds);
        }
		
		public ActionResult Billtype_detailsEdit(Int32 Id)
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Billtype_details where id="+Id);
			dt.TableName = "Billtype_details";
            ds.Tables.Add(dt);
            DataTable dtbilltype_master = DBClass.GetData("select billtypeid,billtypename from billtype_master");
			ViewBag.billtype_master = new SelectList(dtbilltype_master.AsDataView(), "billtypeid", "billtypename", dt.Rows[0]["billtypeid"]);
            DataTable dtaccount_master = DBClass.GetData("select accountnumber,accountname from account_master");
            ViewBag.account_master = new SelectList(dtaccount_master.AsDataView(), "accountnumber", "accountname", dt.Rows[0]["accountnumber"]);			
            ViewBag.DialogTitle="Edit Billtype_details";
            return View("addBilltype_details",ds);
        }
		
		public ActionResult Save(FormCollection fc){
            if (fc["id"] == null || fc["id"] == "")
            {
                Billtype_details t = new Billtype_details();
                Boolean res = t.InsertBilltype_details(fc);
                if (res) return Content("Record Inserted Successfully"); else return Content("Error Adding Record Try Again");
            }
            else 
            {
                Billtype_details t = new Billtype_details();
                Boolean res = t.UpdateBilltype_details(fc);
                if (res) return Content("Record Edited Successfully"); else return Content("Error Editing Record Try Again");
            }
            //return PartialView("SaveRec");
        }

        public ActionResult Billtype_detailsdelete(int id)
        {
            Billtype_details t = new Billtype_details();
            Boolean res = t.DeleteRecord(id);
            if (res) return Content("Record Deleted Successfully"); else return Content("Error Deleting Record Try Again");
        }
    }
}
