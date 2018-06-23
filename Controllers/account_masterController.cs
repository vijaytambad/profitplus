using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using ProfitPlus.Models;
using ProfitPlus.Helpers;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;

namespace ProfitPlus.Controllers
{
    public class Account_masterController : Controller
    {
		public int PerPageRec=50;
        public ActionResult Index()
        {
            return Redirect("Account_master/Account_masterlist?page=1");
        }
        public ActionResult Account_masterlist()
        { 	
			int pageno = Convert.ToInt16(Request["page"]);
			GenHelper.pager gen = new GenHelper.pager();
            Account_master obj = new Account_master();
            DataSet ds = new DataSet();
            DataTable dt = obj.GetOrderedRecords(pageno, PerPageRec, "accountid desc");
            Int32 cnt = obj.GetOrderedRecordsCount("accountid desc");
			gen.PagedList(pageno, cnt,PerPageRec,"Account_master","Account_masterlist");
			ViewBag.pageLinks=gen.PageNos;
			ViewData["TableHeading"] = "List Of Account_master";
            ViewData["addlink"]="Account_masterAdd";
            dt.TableName = "Account_master";
            ds.Tables.Add(dt);
			return View("listAccount_master",ds);
        }
		
		public ActionResult Account_masterAdd()
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Account_master where accountid=0");
			dt.Rows.Add(dt.NewRow());
			dt.TableName = "Account_master";
            ds.Tables.Add(dt);
            			DataTable dtaccount_parent = DBClass.GetData("select parentid,parentname from account_parent");
			ViewBag.account_parent = new SelectList(dtaccount_parent.AsDataView(), "parentid", "parentname", dt.Rows[0]["parentid"]);
						DataTable dtaccount_group = DBClass.GetData("select groupid,groupname from account_group");
			ViewBag.account_group = new SelectList(dtaccount_group.AsDataView(), "groupid", "groupname", dt.Rows[0]["groupid"]);
						DataTable dtsection_master = DBClass.GetData("select sectionid,sectionname from section_master");
			ViewBag.section_master = new SelectList(dtsection_master.AsDataView(), "sectionid", "sectionname", dt.Rows[0]["sectionid"]);
			
            ViewBag.DialogTitle="Add Account_master";
            return View("addAccount_master",ds);
        }
		
		public ActionResult Account_masterEdit(Int32 Id)
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Account_master where accountid="+Id);
			dt.TableName = "Account_master";
             ds.Tables.Add(dt);
            			DataTable dtaccount_parent = DBClass.GetData("select parentid,parentname from account_parent");
			ViewBag.account_parent = new SelectList(dtaccount_parent.AsDataView(), "parentid", "parentname", dt.Rows[0]["parentid"]);
						DataTable dtaccount_group = DBClass.GetData("select groupid,groupname from account_group");
			ViewBag.account_group = new SelectList(dtaccount_group.AsDataView(), "groupid", "groupname", dt.Rows[0]["groupid"]);
						DataTable dtsection_master = DBClass.GetData("select sectionid,sectionname from section_master");
			ViewBag.section_master = new SelectList(dtsection_master.AsDataView(), "sectionid", "sectionname", dt.Rows[0]["sectionid"]);
			
            ViewBag.DialogTitle="Edit Account_master";
            return View("addAccount_master",ds);
        }
		
		public ActionResult Save(FormCollection fc){
            if (fc["accountid"] == null || fc["accountid"] == "")
            {
                Account_master t = new Account_master();
                Boolean res = t.InsertAccount_master(fc);
                if (res) return Content("Record Inserted Successfully"); else return Content("Error Adding Record Try Again");
            }
            else 
            {
                Account_master t = new Account_master();
                Boolean res = t.UpdateAccount_master(fc);
                if (res) return Content("Record Edited Successfully"); else return Content("Error Editing Record Try Again");
            }
            //return PartialView("SaveRec");
        }

        public ActionResult Account_masterdelete(int id)
        {
            Account_master t = new Account_master();
            Boolean res = t.DeleteRecord(id);
            if (res) return Content("Record Deleted Successfully"); else return Content("Error Deleting Record Try Again");
        }
        public ActionResult Account_groupdelete(int id)
        {
            Account_group t = new Account_group();
            Boolean res = t.DeleteRecord(id);
            if (res) return Content("Record Deleted Successfully"); else return Content("Error Deleting Record Try Again");
        }
        public JsonResult IsAvailable(String accountnumber)
        {
            Account_master d = new Account_master();
            return Json(d.IsAvailable(accountnumber), JsonRequestBehavior.AllowGet);
            
        }

        public ActionResult pdf() {
            DataSet ds = new DataSet();
            int[] sumcolindex = { 3, 4, 5, 6 };
            float[] cols=new float[]{35f,40f,220f,50f,50f,50f,50f,50f};
            String path = Server.MapPath("/pdfs/test.pdf");
            String[] rephead = new String[] { "N.EW.K.R.T.C Hubli", "Account List " };
            String[] tblhead = new String[] { "Account Id", "Account Number", "Account Name","Parent Id","Group Id","Section Id","Allow","Active" };
            DataTable dt = DBClass.GetData("select * from account_master");
            ds.Tables.Add(dt);
            pdf ph = new pdf(ds, cols,rephead,tblhead, path);
            ph.GenerateReport();
            return File(path, "application/pdf");
            
        }

        
       
    }
}
