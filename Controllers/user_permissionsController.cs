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
    public class User_permissionsController : Controller
    {
		public int PerPageRec=4;
        public ActionResult Index()
        {
            return Redirect("User_permissions/User_permissionslist?page=1");
        }
        public ActionResult User_permissionslist()
        { 	
			int pageno = Convert.ToInt16(Request["page"]);
			GenHelper.pager gen = new GenHelper.pager();
            User_permissions obj = new User_permissions();
            DataSet ds = new DataSet();
            DataTable dt = obj.GetOrderedRecords(pageno, PerPageRec, "id desc");
            Int32 cnt = obj.GetOrderedRecordsCount("id desc");
			gen.PagedList(pageno, cnt,PerPageRec,"User_permissions","User_permissionslist");
			ViewBag.pageLinks=gen.PageNos;
			ViewData["TableHeading"] = "List Of User_permissions";
            ViewData["addlink"]="User_permissionsAdd";
            dt.TableName = "User_permissions";
            ds.Tables.Add(dt);
			return View("listUser_permissions",ds);
        }
		
		public ActionResult User_permissionsAdd()
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from User_permissions where id=0");
			dt.Rows.Add(dt.NewRow());
			dt.TableName = "User_permissions";
            ds.Tables.Add(dt);
            			DataTable dtuser_types = DBClass.GetData("select usertypeid,usertypename from user_types");
			ViewBag.user_types = new SelectList(dtuser_types.AsDataView(), "usertypeid", "usertypename", dt.Rows[0]["usertypeid"]);
						DataTable dtuser_menus = DBClass.GetData("select menuid,menuname from user_menus");
			ViewBag.user_menus = new SelectList(dtuser_menus.AsDataView(), "menuid", "menuname", dt.Rows[0]["menuid"]);
			
            ViewBag.DialogTitle="Add User_permissions";
            return View("addUser_permissions",ds);
        }
		
		public ActionResult User_permissionsEdit(Int32 Id)
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from User_permissions where id="+Id);
			dt.TableName = "User_permissions";
             ds.Tables.Add(dt);
            			DataTable dtuser_types = DBClass.GetData("select usertypeid,usertypename from user_types");
			ViewBag.user_types = new SelectList(dtuser_types.AsDataView(), "usertypeid", "usertypename", dt.Rows[0]["usertypeid"]);
						DataTable dtuser_menus = DBClass.GetData("select menuid,menuname from user_menus");
			ViewBag.user_menus = new SelectList(dtuser_menus.AsDataView(), "menuid", "menuname", dt.Rows[0]["menuid"]);
			
            ViewBag.DialogTitle="Edit User_permissions";
            return View("addUser_permissions",ds);
        }
		
		public ActionResult Save(FormCollection fc){
            if (fc["id"] == null || fc["id"] == "")
            {
                User_permissions t = new User_permissions();
                Boolean res = t.InsertUser_permissions(fc);
                if (res) return Content("Record Inserted Successfully"); else return Content("Error Adding Record Try Again");
            }
            else 
            {
                User_permissions t = new User_permissions();
                Boolean res = t.UpdateUser_permissions(fc);
                if (res) return Content("Record Edited Successfully"); else return Content("Error Editing Record Try Again");
            }
            //return PartialView("SaveRec");
        }

        public ActionResult User_permissionsdelete(int id)
        {
            User_permissions t = new User_permissions();
            Boolean res = t.DeleteRecord(id);
            if (res) return Content("Record Deleted Successfully"); else return Content("Error Deleting Record Try Again");
        }
    }
}
