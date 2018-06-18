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
    public class UsersController : Controller
    {
		public int PerPageRec=4;
        public ActionResult Index()
        {
            return Redirect("Users/Userslist?page=1");
        }
        public ActionResult Userslist()
        { 	
			int pageno = Convert.ToInt16(Request["page"]);
			GenHelper.pager gen = new GenHelper.pager();
            Users obj = new Users();
            DataSet ds = new DataSet();
            DataTable dt = obj.GetOrderedRecords(pageno, PerPageRec, "id desc");
            Int32 cnt = obj.GetOrderedRecordsCount("id desc");
			gen.PagedList(pageno, cnt,PerPageRec,"Users","Userslist");
			ViewBag.pageLinks=gen.PageNos;
			ViewData["TableHeading"] = "List Of Users";
            ViewData["addlink"]="UsersAdd";
            dt.TableName = "Users";
            ds.Tables.Add(dt);
			return View("listUsers",ds);
        }
		
		public ActionResult UsersAdd()
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from users where id=0");
			dt.Rows.Add(dt.NewRow());
			dt.TableName = "Users";
            ds.Tables.Add(dt);
            			DataTable dtsection_master = DBClass.GetData("select sectionid,sectionname from section_master");
			ViewBag.section_master = new SelectList(dtsection_master.AsDataView(), "sectionid", "sectionname", dt.Rows[0]["sectionid"]);
						DataTable dtdiv_master = DBClass.GetData("select divid,divname from div_master");
			ViewBag.div_master = new SelectList(dtdiv_master.AsDataView(), "divid", "divname", dt.Rows[0]["divid"]);
						DataTable dtuser_types = DBClass.GetData("select usertypeid,usertypename from user_types");
			ViewBag.user_types = new SelectList(dtuser_types.AsDataView(), "usertypeid", "usertypename", dt.Rows[0]["usertypeid"]);
			
            ViewBag.DialogTitle="Add Users";
            return View("addUsers",ds);
        }
		
		public ActionResult UsersEdit(Int32 Id)
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Users where id="+Id);
			dt.TableName = "Users";
             ds.Tables.Add(dt);
            			DataTable dtsection_master = DBClass.GetData("select sectionid,sectionname from section_master");
			ViewBag.section_master = new SelectList(dtsection_master.AsDataView(), "sectionid", "sectionname", dt.Rows[0]["sectionid"]);
						DataTable dtdiv_master = DBClass.GetData("select divid,divname from div_master");
			ViewBag.div_master = new SelectList(dtdiv_master.AsDataView(), "divid", "divname", dt.Rows[0]["divid"]);
						DataTable dtuser_types = DBClass.GetData("select usertypeid,usertypename from user_types");
			ViewBag.user_types = new SelectList(dtuser_types.AsDataView(), "usertypeid", "usertypename", dt.Rows[0]["usertypeid"]);
			
            ViewBag.DialogTitle="Edit Users";
            return View("addUsers",ds);
        }
		
		public ActionResult Save(FormCollection fc){
            if (fc["id"] == null || fc["id"] == "")
            {
                Users t = new Users();
                Boolean res = t.InsertUsers(fc);
                if (res) return Content("Record Inserted Successfully"); else return Content("Error Adding Record Try Again");
            }
            else 
            {
                Users t = new Users();
                Boolean res = t.UpdateUsers(fc);
                if (res) return Content("Record Edited Successfully"); else return Content("Error Editing Record Try Again");
            }
            //return PartialView("SaveRec");
        }

        public ActionResult Usersdelete(int id)
        {
            Users t = new Users();
            Boolean res = t.DeleteRecord(id);
            if (res) return Content("Record Deleted Successfully"); else return Content("Error Deleting Record Try Again");
        }

    }
}
