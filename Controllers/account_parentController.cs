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
    public class Account_parentController : Controller
    {
		public int PerPageRec=4;
        public ActionResult Index()
        {
            return Redirect("Account_parent/Account_parentlist?page=1");
        }
        public ActionResult Account_parentlist()
        { 	
			int pageno = Convert.ToInt16(Request["page"]);
			GenHelper.pager gen = new GenHelper.pager();
            Account_parent obj = new Account_parent();
            DataSet ds = new DataSet();
            DataTable dt = obj.GetOrderedRecords(pageno, PerPageRec, "parentid desc");
            Int32 cnt = obj.GetOrderedRecordsCount("parentid desc");
			gen.PagedList(pageno, cnt,PerPageRec,"Account_parent","Account_parentlist");
			ViewBag.pageLinks=gen.PageNos;
			ViewData["TableHeading"] = "List Of Account_parent";
            ViewData["addlink"]="Account_parentAdd";
            dt.TableName = "Account_parent";
            ds.Tables.Add(dt);
			return View("listAccount_parent",ds);
        }
		
		public ActionResult Account_parentAdd()
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Account_parent where parentid=0");
			dt.Rows.Add(dt.NewRow());
			dt.TableName = "Account_parent";
            ds.Tables.Add(dt);
            
            ViewBag.DialogTitle="Add Account_parent";
            return View("addAccount_parent",ds);
        }
		
		public ActionResult Account_parentEdit(Int32 Id)
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Account_parent where parentid="+Id);
			dt.TableName = "Account_parent";
             ds.Tables.Add(dt);
            
            ViewBag.DialogTitle="Edit Account_parent";
            return View("addAccount_parent",ds);
        }
		
		public ActionResult Save(FormCollection fc){
            if (fc["parentid"] == null || fc["parentid"] == "")
            {
                Account_parent t = new Account_parent();
                Boolean res = t.InsertAccount_parent(fc);
                if (res) return Content("Record Inserted Successfully"); else return Content("Error Adding Record Try Again");
            }
            else 
            {
                Account_parent t = new Account_parent();
                Boolean res = t.UpdateAccount_parent(fc);
                if (res) return Content("Record Edited Successfully"); else return Content("Error Editing Record Try Again");
            }
            //return PartialView("SaveRec");
        }

        public ActionResult Account_parentdelete(int id)
        {
            Account_parent t = new Account_parent();
            Boolean res = t.DeleteRecord(id);
            if (res) return Content("Record Deleted Successfully"); else return Content("Error Deleting Record Try Again");
        }
        public JsonResult IsAvailable(String parentname)
        {
            Account_parent d = new Account_parent();
            return Json(d.IsAvailable(parentname), JsonRequestBehavior.AllowGet);

        }
    }
}
