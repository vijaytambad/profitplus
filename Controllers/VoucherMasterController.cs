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
    public class VoucherMasterController : Controller
    {
        //
        // GET: /VoucherMaster/
        enum type
        {
            DB,
            CR

        };
        public ActionResult Index()
        {
            return Redirect("VoucherMaster/vouchermaster");
        }
        public ActionResult vouchermaster()
        {
            DataSet ds = new DataSet();
            DataTable dt = DBClass.GetAllRecords("select * from Billtype_details where id=0");
            dt.Rows.Add(dt.NewRow());
            dt.TableName = "Billtype_details";
            ds.Tables.Add(dt);
            DataTable dtbilltype_master = DBClass.GetData("select billtypeid,billtypename from billtype_master");
            ViewBag.billtype_master = new SelectList(dtbilltype_master.AsDataView(), "billtypeid", "billtypename", dt.Rows[0]["billtypeid"]);
            DataTable dtaccount_master = DBClass.GetData("select accountid,accountname from account_master");
            ViewBag.account_master = new SelectList(dtaccount_master.AsDataView(), "accountid", "accountname", dt.Rows[0]["accountnumber"]);
            DataTable party_master = DBClass.GetData("select partyid,partyname from party_master");
            ViewBag.party_master = new SelectList(party_master.AsDataView(), "partyid", "partyname", dt.Rows[0]["accountnumber"]);
           // ViewBag.DialogTitle = "Add Billtype_details";
            DataTable dt10 = DBClass.GetAllRecords("select id,trtype,accno,dramount,cramount from accounttrans");
            dt10.TableName = "Accttrans";
            ds.Tables.Add(dt10);
            ViewData["addlink"] = "voucherAdd";
            return View("VoucherMaster", ds);
        }
        public ActionResult voucherform()
        {
            DataSet ds = new DataSet();
            DataTable dt = DBClass.GetAllRecords("select * from accounttrans where transid=0");
            dt.Rows.Add(dt.NewRow());
            dt.TableName = "AccountTrans";
            ds.Tables.Add(dt);            
            ViewBag.DialogTitle = "Add Voucher Entry";
            return View("openvoucher", ds);
        }
        public ActionResult voucherAdd()
        {
            DataSet ds = new DataSet();
            int divid = Convert.ToInt16(Session["divid"]);
            int sectionid = Convert.ToInt16(Session["sectionid"]);
            DataTable dt = DBClass.GetAllRecords("select * from accounttrans  where transid=0");
            dt.Rows.Add(dt.NewRow());
            dt.TableName = "Addvoucherdata";
            ds.Tables.Add(dt);
            ViewBag.DialogTitle = "Add Voucher Entry";
            return View("addvoucher", ds);
        }
        public ActionResult listdata()
        {
            DataSet ds = new DataSet();
            DataTable dt10 = DBClass.GetAllRecords("select id,trtype,accno,dramount,cramount from accounttrans");
            dt10.TableName = "Accttrans";
            ds.Tables.Add(dt10);
            ViewData["addlink"] = "voucherAdd";
            ViewBag.DialogTitle = "Add Voucher";
           // return View("VoucherMaster", ds);          
            return View("listvoucherdata",ds);
        }
        public ActionResult Save(FormCollection fc)
        {
            if (fc["id"] == null || fc["id"] == "")
            {
                int divid = Convert.ToInt16(Session["divid"]);
                int sectionid = Convert.ToInt16(Session["sectionid"]);
                int userid = Convert.ToInt16(Session["userid"]);
                VoucherMaster t = new VoucherMaster();
                Boolean res = t.Insertvoucherdata(fc,divid,sectionid,userid);
                if (res) return Content("Record Inserted Successfully"); else return Content("Error Adding Record Try Again");
            }
            else
            {
                Party_master t = new Party_master();
                Boolean res = t.UpdateParty_master(fc);
                if (res) return Content("Record Edited Successfully"); else return Content("Error Editing Record Try Again");
            }
            //return PartialView("SaveRec");
        }
        public ActionResult voucherEdit(int id)
        {

            DataSet ds = new DataSet();
            DataTable dt = DBClass.GetAllRecords("select * from accounttrans where id=" + id);
            dt.TableName = "Accttrans";
            ds.Tables.Add(dt);         
            ViewBag.DialogTitle = "Edit Acctrans";
            return View("addvoucher", ds);        
        
        }
        public ActionResult voucherDelete(int id)
        {

            VoucherMaster t = new VoucherMaster();
            Boolean res = t.DeleteRecord(id);
            if (res) return Content("Record Deleted Successfully"); else return Content("Error Deleting Record Try Again");


        }
    }
}
