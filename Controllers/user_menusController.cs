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
    public class User_menusController : Controller
    {
		public int PerPageRec=3;
        public ActionResult Index()
        {
            return Redirect("User_menus/User_menuslist?page=1");
        }
        public ActionResult User_menuslist()
        { 	
			int pageno = Convert.ToInt16(Request["page"]);
			GenHelper.pager gen = new GenHelper.pager();
            User_menus obj = new User_menus();
            DataSet ds = new DataSet();
            DataTable dt = obj.GetOrderedRecords(pageno, PerPageRec, "menuid desc");
            Int32 cnt = obj.GetOrderedRecordsCount("menuid desc");
			gen.PagedList(pageno, cnt,PerPageRec,"User_menus","User_menuslist");
			ViewBag.pageLinks=gen.PageNos;
			ViewData["TableHeading"] = "List Of User_menus";
            ViewData["addlink"]="User_menusAdd";
            dt.TableName = "User_menus";
            ds.Tables.Add(dt);
			return View("listUser_menus",ds);
        }
		
		public ActionResult User_menusAdd()
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from User_menus where menuid=0");
			dt.Rows.Add(dt.NewRow());
			dt.TableName = "User_menus";
            ds.Tables.Add(dt);
            
            ViewBag.DialogTitle="Add User_menus";
            return View("addUser_menus",ds);
        }
		
		public ActionResult User_menusEdit(Int32 Id)
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from User_menus where menuid="+Id);
			dt.TableName = "User_menus";
             ds.Tables.Add(dt);
            
            ViewBag.DialogTitle="Edit User_menus";
            return View("addUser_menus",ds);
        }
		
		public ActionResult Save(FormCollection fc){
            if (fc["menuid"] == null || fc["menuid"] == "")
            {
                User_menus t = new User_menus();
                Boolean res = t.InsertUser_menus(fc);
                if (res) return Content("Record Inserted Successfully"); else return Content("Error Adding Record Try Again");
            }
            else 
            {
                User_menus t = new User_menus();
                Boolean res = t.UpdateUser_menus(fc);
                if (res) return Content("Record Edited Successfully"); else return Content("Error Editing Record Try Again");
            }
            //return PartialView("SaveRec");
        }

        public ActionResult User_menusdelete(int id)
        {
            User_menus t = new User_menus();
            Boolean res = t.DeleteRecord(id);
            if (res) return Content("Record Deleted Successfully"); else return Content("Error Deleting Record Try Again");
        }
        public JsonResult IsAvailable(String menuname)
        {
            User_menus d = new User_menus();
            return Json(d.IsAvailable(menuname), JsonRequestBehavior.AllowGet);

        }
    }
}
