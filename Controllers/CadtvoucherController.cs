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
    public class CadtvoucherController : Controller
    {
        //
        // GET: /Cadtvoucher/

        public ActionResult Index()
        {
            return Redirect("Cadtvoucher/Voucher_masterAdd");
        }
        public enum vouchertype : int
        {
            ADV = 0,
            EXP = 1,

        }
        public ActionResult Voucher_masterAdd()
        {
            int sectionid = Convert.ToInt32(Session["sectionid"]);
            DataSet ds = new DataSet();
            DataTable dt = DBClass.GetAllRecords("select * from Voucher_master where id=0");
            dt.Rows.Add(dt.NewRow());
            dt.TableName = "Cadtvoucher";
            ds.Tables.Add(dt);
            DataTable dtbilltype_master = DBClass.GetData("select billtypeid,billtypename from billtype_master where sectionid=" + sectionid);
            ViewBag.billtype_master = new SelectList(dtbilltype_master.AsDataView(), "billtypeid", "billtypename", dt.Rows[0]["billtypeid"]);
            DataTable dtsection_master = DBClass.GetData("select sectionid,sectionname from section_master");
            ViewBag.section_master = new SelectList(dtsection_master.AsDataView(), "sectionid", "sectionname", dt.Rows[0]["sectionid"]);
            DataTable dtdiv_master = DBClass.GetData("select divid,divname from div_master");
            ViewBag.div_master = new SelectList(dtdiv_master.AsDataView(), "divid", "divname", dt.Rows[0]["divid"]);
            DataTable dtparty_master = DBClass.GetData("select partyid,partyname from party_master");
            ViewBag.party_master = new SelectList(dtparty_master.AsDataView(), "partyid", "partyname", dt.Rows[0]["partyid"]);
            ViewBag.DialogTitle = "Add RadtVoucher_master";
        /**    var enumData = from vouchertype e in Enum.GetValues(typeof(vouchertype))
                           select new
                           {
                               ID = (int)e,
                               Name = e.ToString()
                           };
            ViewBag.EnumList = new SelectList(enumData, "ID", "Name");  **/
            return View("addCadtVoucher", ds);
        }
        public ActionResult GetBillDetails()
        {
            int billtypeid = Convert.ToInt16(Request["billid"]);
            DataSet ds = new DataSet();
            String[] tblheader = {"Sl.no","Type","Acct.Head","Db","Cr","Action" };
            ViewBag.tblheader = tblheader;
            Billtype_details det = new Billtype_details();
            DataTable dt = det.GetOrderedRecordsbilltype(billtypeid, "id");
           // dt.TableName = "mytab";
         //   dt.Columns.Add("slno", typeof(System.Int32));
            ds.Tables.Add(dt);
            Session["datatable"] = dt;
            ViewBag.TotatDb = totaldbcr("DB",dt);
            ViewBag.TotalCr = totaldbcr("CR",dt);
            return PartialView("GetBillDetails", dt);
        }
        public double totaldbcr(String type, DataTable mydt)
        {
            double total=0;
            if (type == "DB")
            {
                for (int i = 0; i < mydt.Rows.Count; i++)
                {
                    total+= Convert.ToDouble(mydt.Rows[i]["dramount"]);
                }
            }
            if(type=="CR")
            {
                for (int i = 0; i < mydt.Rows.Count; i++)
                {
                    total+= Convert.ToDouble(mydt.Rows[i]["cramount"]);
                }
            
            }

            return total;
        }

        public ActionResult EditDetails()
        {
            int myid = Convert.ToInt16(Request["slno"]);
            DataTable mytab = (DataTable)Session["datatable"];
            DataRow[] rows = mytab.Select("id=" + myid.ToString());
            ViewBag.data=rows[0];
            return View("EditAccounting");
        }
        public ActionResult savebilldetail(FormCollection fc)
        {
          //  DataSet ds = new DataSet();
           DataTable dt = (DataTable)Session["datatable"];
           int id=Convert.ToInt32(fc["id"]);
           DataRow rows = dt.Select("id=" + id.ToString()).FirstOrDefault();
           rows["dramount"]=Convert.ToDouble(fc["dramount"]);
           rows["cramount"]=Convert.ToDouble(fc["cramount"]);
           Session["datatable"]=dt;
           String[] tblheader = { "Sl.no", "Type", "Acct.Head", "Db", "Cr", "Action" };
           ViewBag.tblheader = tblheader;
           DataTable newdt = (DataTable)Session["datatable"];
           ViewBag.TotatDb = totaldbcr("DB", dt);
           ViewBag.TotalCr = totaldbcr("CR", dt);
           return View("GetBillDetails", newdt);

        }

        public ActionResult Voucher_masterEdit()
        {
            DataSet ds = new DataSet();
            DataTable dt = DBClass.GetAllRecords("select dramount,cramount from accounttrans where id=0");
            dt.Rows.Add(dt.NewRow());
            dt.TableName = "Cadtvoucher";
            ds.Tables.Add(dt);
            ViewBag.DialogTitle = "Cadtvoucher";
            return View("EditCadtVoucher", ds);
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
                Users user = new Users();
                DataTable userdt = user.GetUserRecord(userid);
                DataTable Cardt = (DataTable)Session["datatable"];
                Cadtvoucher cadt = new Cadtvoucher();
                DateTime fst = (DateTime)Session["finstart"];
                DateTime fend = (DateTime)Session["finend"];
                int divid = Convert.ToInt32(Session["divid"]);
                int sectionid=Convert.ToInt32(Session["sectionid"]);
                String divname = Session["divname"].ToString();
                String trid = "";
                DataRow[] rows = Cardt.Select("accountnumber=" + payaccno);
                double Payamt = Convert.ToDouble(rows[0]["cramount"]);

                if (cadt.savevoucherdata(fc, userdt, Cardt, ref trid, fst, fend, divid, divname, sectionid, userid,Payamt))
                {
                    return Content("Transaction saved sucessfully with Voucher.No="+trid);
                }
                else
                { 
                    return Content("Error in Saving");
                
                }
            
            }

            return Content("Saved");
        
        }
    }
}
