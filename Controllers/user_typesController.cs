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
    public class User_typesController : Controller
    {
        GenHelper g = new GenHelper();
       // List<String> Errors;
		public int PerPageRec=4;
        public ActionResult Index()
        {
            return Redirect("User_types/User_typeslist?page=1");
        }
        public ActionResult User_typeslist()
        {
            if (!g.HasPermission()) return Redirect("/Login");
            
			int pageno = Convert.ToInt16(Request["page"]);
			GenHelper.pager gen = new GenHelper.pager();
            User_types obj = new User_types();
            DataSet ds = new DataSet();
            DataTable dt = obj.GetOrderedRecords(pageno, PerPageRec, "usertypeid desc");
            Int32 cnt = obj.GetOrderedRecordsCount("usertypeid desc");
			gen.PagedList(pageno, cnt,PerPageRec,"User_types","User_typeslist");
			ViewBag.pageLinks=gen.PageNos;
			ViewData["TableHeading"] = "List Of User_types";
            ViewData["addlink"]="User_typesAdd";
            dt.TableName = "User_types";
            ds.Tables.Add(dt);
			return View("listUser_types",ds);
        }
		
		public ActionResult User_typesAdd()
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from User_types where usertypeid=0");
			dt.Rows.Add(dt.NewRow());
			dt.TableName = "User_types";
            ds.Tables.Add(dt);
            ViewBag.DialogTitle="Add User_types";
            return View("addUser_types",ds);
        }
		
		public ActionResult User_typesEdit(Int32 Id)
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from User_types where usertypeid="+Id);
			dt.TableName = "User_types";
             ds.Tables.Add(dt);
            
            ViewBag.DialogTitle="Edit User_types";
            return View("addUser_types",ds);
        }
		
		public ActionResult Save(FormCollection fc){
            if (fc["usertypeid"] == null || fc["usertypeid"] == "")
            {
                User_types t = new User_types();
                Boolean res = t.InsertUser_types(fc);
                if (res) return Content("Record Inserted Successfully"); else return Content("Error Adding Record Try Again");
            }
            else 
            {
                User_types t = new User_types();
                Boolean res = t.UpdateUser_types(fc);
                if (res) return Content("Record Edited Successfully"); else return Content("Error Editing Record Try Again");
            }
            //return PartialView("SaveRec");
        }

        public ActionResult User_typesdelete(int id)
        {

            if (!CanDelete())
                return Content("Contains Related Recordes in another table, You Cannot Delete This Record");
            User_types t = new User_types();
            Boolean res = t.DeleteRecord(id);
            if (res) return Content("Record Deleted Successfully"); else return Content("Error Deleting Record Try Again");

        }

        public Boolean CanDelete(){

            return false;
        }
        public ActionResult SetPermission(int id) {
          //  String MyMneu = "";
            String Qry = "Select A.menuid,A.menuname,A.menuurl, checked=";
            Qry += "CASE WHEN EXISTS (select * from user_permissions B where B.menuid = A.menuid and B.usertypeid="+id.ToString()+")";
            Qry+=" THEN 1 ELSE 0 END from user_menus A";
            String Qry1 = "select usertypename from user_types where usertypeid="+id;
            DataTable dt1 = DBClass.GetAllRecords(Qry1);
            String username=(dt1.Rows[0]["usertypename"]).ToString();
            DataTable dt = DBClass.GetAllRecords (Qry);
            dt.TableName = "Permissions";
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            ViewBag.UserTypeId =id;
            ViewBag.PageTitle = "Set User Permissions";
            return View("SetPermission", ds);
        }

        public ActionResult savepermissions() {
            int utype = Convert.ToInt32(Request.QueryString.Get("uid"));
            String Mess="";
            String ids =Request.QueryString.Get("chkids");
            User_types u = new User_types();
            if (u.SavePer(ids, utype)) Mess = "Permissions Saved "; else Mess="Try again";
            return Content(Mess);
        }

      /**  public void GenerateMenu(DataTable mydt, ref String MyMenu)
        {
            for (int i = 0; i < mydt.Rows.Count; i++) {
                MyMenu += "<li><a href='" + mydt.Rows[i]["menuurl"]+ "'>" + mydt.Rows[i]["menuname"] + "</a></li>";
            }
        }  **/
        public JsonResult IsAvailable(String usertypename)
        {
            User_types d = new User_types();
            return Json(d.IsAvailable(usertypename), JsonRequestBehavior.AllowGet);

        }

    }
}
