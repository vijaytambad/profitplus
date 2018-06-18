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
    public class PadtvoucherController : Controller
    {
        //
        // GET: /Padtvoucher/

        public ActionResult Index()
        {
            return Redirect("Padtvoucher/Voucher_masterAdd");
        }
        public ActionResult Voucher_masterAdd()
        {
            int sectionid = Convert.ToInt32(Session["sectionid"]);
            DataSet ds = new DataSet();
            DataTable dt = DBClass.GetAllRecords("select * from Voucher_master where id=0");
            dt.Rows.Add(dt.NewRow());
            dt.TableName = "Padtvoucher";
            ds.Tables.Add(dt);
            DataTable dtbilltype_master = DBClass.GetData("select billtypeid,billtypename from billtype_master where sectionid=" + sectionid);
            ViewBag.billtype_master = new SelectList(dtbilltype_master.AsDataView(), "billtypeid", "billtypename", dt.Rows[0]["billtypeid"]);
            DataTable dtparty_master = DBClass.GetData("select partyid,partyname from party_master");
            ViewBag.party_master = new SelectList(dtparty_master.AsDataView(), "partyid", "partyname", dt.Rows[0]["partyid"]);
            ViewBag.DialogTitle = "Add Padtvoucher";
            return View("addPadtVoucher",ds);
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
        public JsonResult ValidateDate(DateTime vdate)
        {
            Cadtvoucher d = new Cadtvoucher();
            return Json(d.IsValid(vdate), JsonRequestBehavior.AllowGet);

        }
        public ActionResult savevoucher(FormCollection fc)
        {
            if (Session["userid"] != null)
            {
                Int32 billtypeid = Convert.ToInt32(fc["billtypeid"]);
                Billtype_master b = new Billtype_master();
                String payaccno = b.Netpayable(billtypeid);
                Int32 userid = Convert.ToInt32(Session["userid"]);
                Users us = new Users();
                DataTable userdt = us.GetUserRecord(userid);
                DataTable Cardt = (DataTable)Session["datatable"];
                Int32 divid = Convert.ToInt32(Session["divid"]);
                Int32 sectionid = Convert.ToInt32(Session["sectionid"]);
                String divname = Session["divname"].ToString();
                String trid = "";
                Int32 totaldb = Convert.ToInt32(fc["totdb"]);
                Padtvoucher padt = new Padtvoucher();
                DateTime fst=(DateTime)Session["finstart"];
                DateTime fend=(DateTime)Session["finend"];
                DataRow[] rows = Cardt.Select("accountnumber=" + payaccno);
                double Payamt = Convert.ToDouble(rows[0]["cramount"]);

                if(padt.savevoucherdata(fc, userdt, Cardt, ref trid,fst, fend, divid, divname, userid, Payamt,sectionid))
                {
                    return Content("Transaction Saved Sucessfully with Trid=" + trid);
                    
                }
                else
                {}
                
            
            
            }


            return Content("");
        }
    }
}
