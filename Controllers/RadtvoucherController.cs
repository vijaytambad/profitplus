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
    public class RadtvoucherController : Controller
    {
        //
        // GET: /Radtvoucher/

        public ActionResult Index()
        {
             return Redirect("Radtvoucher/Voucher_masterAdd");
        
        }
        public ActionResult Voucher_masterAdd()
        {
            int sectionid = Convert.ToInt32(Session["sectionid"]);
            Int32 divid=Convert.ToInt32(Session["divid"]);
            DataSet ds = new DataSet();
            DataTable dt = DBClass.GetAllRecords("select * from Voucher_master where id=0");
            dt.Rows.Add(dt.NewRow());
            dt.TableName = "Radtvoucher";
            ds.Tables.Add(dt);
            DataTable dtbilltype_master = DBClass.GetData("select billtypeid,billtypename from billtype_master where sectionid=" + sectionid);
            ViewBag.billtype_master = new SelectList(dtbilltype_master.AsDataView(), "billtypeid", "billtypename", dt.Rows[0]["billtypeid"]);
            DataTable dtparty_master = DBClass.GetData("select partyid,partyname from party_master");
            ViewBag.party_master = new SelectList(dtparty_master.AsDataView(), "partyid", "partyname", dt.Rows[0]["partyid"]);
            DataTable dtdepot_master = DBClass.GetData("select depotid,depotname from depot_master where divid="+divid);
            ViewBag.depot_master = new SelectList(dtdepot_master.AsDataView(), "depotid", "depotname", dt.Rows[0]["depotid"]);
            ViewBag.DialogTitle = "Add Radtvoucher";
            return View("addRadtVoucher", ds);
        }
        public ActionResult GetBillDetails()
        {
            int billtypeid = Convert.ToInt16(Request["billid"]);
            DataSet ds = new DataSet();
            String[] tblheader = { "Sl.no", "Type", "Acct.Head", "Db", "Cr", "Action" };
            ViewBag.tblheader = tblheader;
            Billtype_details det = new Billtype_details();
            DataTable dt = det.GetOrderedRecordsbilltype(billtypeid, "id");
            ds.Tables.Add(dt);
            Session["datatable"] = dt;
            ViewBag.TotatDb = totaldbcr("DB", dt);
            ViewBag.TotalCr = totaldbcr("CR", dt);
            return PartialView("GetBillDetails", dt);
        }
        public double totaldbcr(String type, DataTable mydt)
        {
            double total = 0;
            if (type == "DB")
            {
                for (int i = 0; i < mydt.Rows.Count; i++)
                {
                    total += Convert.ToDouble(mydt.Rows[i]["dramount"]);
                }
            }
            if (type == "CR")
            {
                for (int i = 0; i < mydt.Rows.Count; i++)
                {
                    total += Convert.ToDouble(mydt.Rows[i]["cramount"]);
                }

            }

            return total;
        }
        public ActionResult savebilldetail(FormCollection fc)
        {
            DataTable dt = (DataTable)Session["datatable"];
            int id = Convert.ToInt32(fc["id"]);
            DataRow rows = dt.Select("id=" + id.ToString()).FirstOrDefault();
            rows["dramount"] = Convert.ToDouble(fc["dramount"]);
            rows["cramount"] = Convert.ToDouble(fc["cramount"]);
            Session["datatable"] = dt;
            String[] tblheader = { "Sl.no", "Type", "Acct.Head", "Db", "Cr", "Action" };
            ViewBag.tblheader = tblheader;
            DataTable newdt = (DataTable)Session["datatable"];
            ViewBag.TotatDb = totaldbcr("DB", dt);
            ViewBag.TotalCr = totaldbcr("CR", dt);
            return View("GetBillDetails", newdt);

        }
        public ActionResult EditDetails()
        {
            int myid = Convert.ToInt16(Request["slno"]);
            DataTable mytab = (DataTable)Session["datatable"];
            DataRow[] rows = mytab.Select("id=" + myid.ToString());
            ViewBag.data = rows[0];
            return View("EditAccounting");
        }
        public ActionResult savevoucher(FormCollection fc)
        {
            if (Session["userid"] != null)
            {
                Int32 userid = Convert.ToInt32(Session["userid"]);
                Users u=new Users();
                DataTable userdt = u.GetUserRecord(userid);
                DataTable Cardt = (DataTable)Session["datatable"];
                DateTime fst = (DateTime)Session["finstart"];
                DateTime fend = (DateTime)Session["finend"];
                int sectionid = Convert.ToInt32(Session["sectionid"]);
                int divid = Convert.ToInt32(Session["divid"]);
                String divname=Session["divname"].ToString();
                String trid = "";
                Radtvocher Radt = new Radtvocher();
                if (Radt.savevoucherdata(fc,userdt,Cardt,fst,fend,sectionid,divid,divname,userid,ref trid))
                {
                    return Content("Transaction saved sucessfully with Voucher.No=" + trid);
                }
                else
                {
                    return Content("Error in Saving");

                }
                
            
            
            }

            return Content("");
        }
    }
}
