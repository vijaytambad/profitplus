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
    public class Account_groupController : Controller
    {
		public int PerPageRec=4;
        public ActionResult Index()
        {
            return Redirect("Account_group/Account_grouplist?page=1");
        }
        public ActionResult Account_grouplist()
        { 	
			int pageno = Convert.ToInt16(Request["page"]);
			GenHelper.pager gen = new GenHelper.pager();
            Account_group obj = new Account_group();
            DataSet ds = new DataSet();
            DataTable dt = obj.GetOrderedRecords(pageno, PerPageRec, "groupid desc");
            Int32 cnt = obj.GetOrderedRecordsCount("groupid desc");
			gen.PagedList(pageno, cnt,PerPageRec,"Account_group","Account_grouplist");
			ViewBag.pageLinks=gen.PageNos;
			ViewData["TableHeading"] = "List Of Account_group";
            ViewData["addlink"]="Account_groupAdd";
            dt.TableName = "Account_group";
            ds.Tables.Add(dt);
			return View("listAccount_group",ds);
        }
		
		public ActionResult Account_groupAdd()
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Account_group where groupid=0");
			dt.Rows.Add(dt.NewRow());
			dt.TableName = "Account_group";
            ds.Tables.Add(dt);
            			DataTable dtaccount_parent = DBClass.GetData("select parentid,parentname from account_parent");
			ViewBag.account_parent = new SelectList(dtaccount_parent.AsDataView(), "parentid", "parentname", dt.Rows[0]["parentid"]);			
            ViewBag.DialogTitle="Add Account_group";
            return View("addAccount_group",ds);
        }
		
		public ActionResult Account_groupEdit(Int32 Id)
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Account_group where groupid="+Id);
			dt.TableName = "Account_group";
             ds.Tables.Add(dt);
            			DataTable dtaccount_parent = DBClass.GetData("select parentid,parentname from account_parent");
			ViewBag.account_parent = new SelectList(dtaccount_parent.AsDataView(), "parentid", "parentname", dt.Rows[0]["parentid"]);
			
            ViewBag.DialogTitle="Edit Account_group";
            return View("addAccount_group",ds);
        }
		
		public ActionResult Save(FormCollection fc){
            if (fc["groupid"] == null || fc["groupid"] == "")
            {
                Account_group t = new Account_group();
                Boolean res = t.InsertAccount_group(fc);
                if (res) return Content("Record Inserted Successfully"); else return Content("Error Adding Record Try Again");
            }
            else 
            {
                Account_group t = new Account_group();
                Boolean res = t.UpdateAccount_group(fc);
                if (res) return Content("Record Edited Successfully"); else return Content("Error Editing Record Try Again");
            }
            //return PartialView("SaveRec");
        }

        public ActionResult Account_groupdelete(int id)
        {
            Account_group t = new Account_group();
            Boolean res = t.DeleteRecord(id);
            if (res) return Content("Record Deleted Successfully"); else return Content("Error Deleting Record Try Again");
        }
        public JsonResult IsAvailable(String groupname)
        {
            Account_group d = new Account_group();
            return Json(d.IsAvailable(groupname), JsonRequestBehavior.AllowGet);

        }
    }
}
