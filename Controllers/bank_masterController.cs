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
    public class Bank_masterController : Controller
    {
		public int PerPageRec=4;
        public ActionResult Index()
        {
            return Redirect("Bank_master/Bank_masterlist?page=1");
        }
        public ActionResult Bank_masterlist()
        { 	
			int pageno = Convert.ToInt16(Request["page"]);
			GenHelper.pager gen = new GenHelper.pager();
            Bank_master obj = new Bank_master();
            DataSet ds = new DataSet();
            DataTable dt = obj.GetOrderedRecords(pageno, PerPageRec, "bankid desc");
            Int32 cnt = obj.GetOrderedRecordsCount("bankid desc");
			gen.PagedList(pageno, cnt,PerPageRec,"Bank_master","Bank_masterlist");
			ViewBag.pageLinks=gen.PageNos;
			ViewData["TableHeading"] = "List Of Bank_master";
            ViewData["addlink"]="Bank_masterAdd";
            dt.TableName = "Bank_master";
            ds.Tables.Add(dt);
			return View("listBank_master",ds);
        }
		
		public ActionResult Bank_masterAdd()
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Bank_master where bankid=0");
			dt.Rows.Add(dt.NewRow());
			dt.TableName = "Bank_master";
            ds.Tables.Add(dt);
            			DataTable dtaccount_master = DBClass.GetData("select accountid,accountname from account_master");
			ViewBag.account_master = new SelectList(dtaccount_master.AsDataView(), "accountid", "accountname", dt.Rows[0]["accountnumber"]);
						DataTable dtdiv_master = DBClass.GetData("select divid,divname from div_master");
			ViewBag.div_master = new SelectList(dtdiv_master.AsDataView(), "divid", "divname", dt.Rows[0]["divid"]);
			
            ViewBag.DialogTitle="Add Bank_master";
            return View("addBank_master",ds);
        }
		
		public ActionResult Bank_masterEdit(Int32 Id)
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Bank_master where bankid="+Id);
			dt.TableName = "Bank_master";
             ds.Tables.Add(dt);
            			DataTable dtaccount_master = DBClass.GetData("select accountid,accountname from account_master");
			ViewBag.account_master = new SelectList(dtaccount_master.AsDataView(), "accountid", "accountname", dt.Rows[0]["accountnumber"]);
						DataTable dtdiv_master = DBClass.GetData("select divid,divname from div_master");
			ViewBag.div_master = new SelectList(dtdiv_master.AsDataView(), "divid", "divname", dt.Rows[0]["divid"]);
			
            ViewBag.DialogTitle="Edit Bank_master";
            return View("addBank_master",ds);
        }
		
		public ActionResult Save(FormCollection fc){
            if (fc["bankid"] == null || fc["bankid"] == "")
            {
                Bank_master t = new Bank_master();
                Boolean res = t.InsertBank_master(fc);
                if (res) return Content("Record Inserted Successfully"); else return Content("Error Adding Record Try Again");
            }
            else 
            {
                Bank_master t = new Bank_master();
                Boolean res = t.UpdateBank_master(fc);
                if (res) return Content("Record Edited Successfully"); else return Content("Error Editing Record Try Again");
            }
            //return PartialView("SaveRec");
        }

        public ActionResult Bank_masterdelete(int id)
        {
            Bank_master t = new Bank_master();
            Boolean res = t.DeleteRecord(id);
            if (res) return Content("Record Deleted Successfully"); else return Content("Error Deleting Record Try Again");
        }
    }
}
