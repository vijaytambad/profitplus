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
    public class Adjvoucher_masterController : Controller
    {
		public int PerPageRec=4;
        public ActionResult Index()
        {
            return Redirect("/Adjvoucher_master/Voucher_masterAdd");
        }
        public ActionResult Adjvoucher_masterlist()
        { 	
			int pageno = Convert.ToInt16(Request["page"]);
			GenHelper.pager gen = new GenHelper.pager();
            Adjvoucher_master obj = new Adjvoucher_master();
            DataSet ds = new DataSet();
            DataTable dt = obj.GetOrderedRecords(pageno, PerPageRec, "id desc");
            Int32 cnt = obj.GetOrderedRecordsCount("id desc");
			gen.PagedList(pageno, cnt,PerPageRec,"Adjvoucher_master","Voucher_masterlist");
			ViewBag.pageLinks=gen.PageNos;
			ViewData["TableHeading"] = "List Of Adjvoucher_master";
            ViewData["addlink"]="Voucher_masterAdd";
            dt.TableName = "Adjvoucher_master";
            ds.Tables.Add(dt);
			return View("listVoucher_master",ds);
        }
		
		public ActionResult Voucher_masterAdd()
        { 
            DataSet ds =new DataSet();
            var list = new SelectList(new[] 
            {
                new { ID = "0", Name = "Select Adjustment" },
                new { ID = "1", Name = "Regular " },
                new { ID = "2", Name = "Year End" },
            },
            "ID", "Name", 0);

            ViewBag.list = list;
			DataTable dt = DBClass.GetAllRecords("select * from Voucher_master where id=0");
			dt.Rows.Add(dt.NewRow());
			dt.TableName = "Adjvoucher_master";
            ds.Tables.Add(dt);
            			//DataTable dtbilltype_master = DBClass.GetData("select billtypeid,billtypename from billtype_master");
                        //ViewBag.billtype_master = new SelectList(dtbilltype_master.AsDataView(), "billtypeid", "billtypename");
						DataTable dtsection_master = DBClass.GetData("select sectionid,sectionname from section_master");
			ViewBag.section_master = new SelectList(dtsection_master.AsDataView(), "sectionid", "sectionname");
						DataTable dtdiv_master = DBClass.GetData("select divid,divname from div_master");
			ViewBag.div_master = new SelectList(dtdiv_master.AsDataView(), "divid", "divname");
						DataTable dtdepot_master = DBClass.GetData("select depotid,depotname from depot_master");
			ViewBag.depot_master = new SelectList(dtdepot_master.AsDataView(), "depotid", "depotname");
            //DataTable party_master = DBClass.GetData("select partyid,partyname from party_master");
            //ViewBag.party_master = new SelectList(party_master.AsDataView(), "partyid", "partyname");
            String Qrybtd = "SELECT id,billtypeid,accountnumber,type,cramount,dramount FROM dbo.billtype_details bd where billtypeid=0";
            DataTable bdt = DBClass.GetData(Qrybtd);
            foreach (DataColumn col in bdt.Columns)
                col.ReadOnly = false;
            bdt.TableName = "billdetails";
            Session["billdetails"] = bdt;
            ds.Tables.Add(bdt);
            String[] tblheader = { "S.no", "trtype", "accountnumber", "dramount", "cramount", "Actions" };
            ViewBag.tblheader = tblheader;
            ViewBag.billdetails = (DataTable)Session["billdetails"];
            ViewBag.totcr = totvalues("CR", bdt);
            ViewBag.totdr = totvalues("DR", bdt);

            ViewBag.DialogTitle="Add Adjvoucher_master";
            return View("addVoucher_master",ds);
        }

        public ActionResult GetBillDetails() {
            String[] tblheader = { "S.no", "trtype", "accountnumber", "dramount", "cramount","Actions" };
            ViewBag.tblheader = tblheader;
            Billtype_details btypdet = new Billtype_details();
            DataTable dt= btypdet.GetOrderedRecords(1, 100, 1, "id");
            //dt.Columns.Add("slno");
            //for (int i = 0; i < dt.Rows.Count; i++) { dt.Rows[i]["slno"] = i+1; }
            Session["billdetails"] = dt;
            DataTable newdt = (DataTable)Session["billdetails"];
            //DataSet ds = new DataSet(); ds.Tables.Add(newdt);
            ViewBag.totcr = totvalues("CR", newdt);
            ViewBag.totdr = totvalues("DR", newdt);
            return PartialView("GetBillDetails",newdt );
        }

        public double totvalues(String trtype,DataTable mydt) {
            double tot = 0;
            if (trtype.Equals("DR"))
            {
                for (int i = 0; i < mydt.Rows.Count; i++) {
                    tot += Convert.ToDouble(mydt.Rows[i]["dramount"]);
                }
            }
            if (trtype.Equals("CR"))
            {
                for (int i = 0; i < mydt.Rows.Count; i++)
                {
                    tot += Convert.ToDouble(mydt.Rows[i]["cramount"]);
                }
            }
            return tot;
        }
       
        public ActionResult EditDetails()
        {
            var list = new SelectList(new[] 
            {
                new { ID = "0", Name = "Select Type" },
                new { ID = "DB", Name = "Debit " },
                new { ID = "CR", Name = "Credit" },
            },
           "ID", "Name", 0);
            DataTable dtaccount_master = DBClass.GetData("select accountnumber,accountname from account_master");
            ViewBag.account_master = new SelectList(dtaccount_master.AsDataView(), "accountnumber", "accountname");
            ViewBag.list = list;
            int myid= Convert.ToInt32(Request["slno"]);
            DataTable dt = (DataTable)Session["billdetails"];
            DataRow[] dr=dt.Select("id=" + myid.ToString());
            ViewBag.row = dr[0];
            return View("editdetail");
        }

        public ActionResult savebilldetail(FormCollection fc) {
            DataTable dt = (DataTable)Session["billdetails"];
            if (Convert.ToInt32(fc["id"]) != 0)
            {
                int id = Convert.ToInt32(fc["id"]);
                DataRow dr = dt.Select("id=" + id.ToString()).FirstOrDefault();
                dr["type"] = fc["trtype"];
                dr["accountnumber"] = fc["accountnumber"];
                dr["cramount"] = Convert.ToDouble(fc["cramount"]);
                dr["dramount"] = Convert.ToDouble(fc["dramount"]);
            }
            else
            {
                DataRow dr= dt.NewRow();
                int id = GetNewID(dt);
                dr["id"] = id;
                dr["type"] = fc["trtype"];
                dr["accountnumber"] = fc["accountnumber"];
                dr["cramount"] = Convert.ToDouble(fc["cramount"]);
                dr["dramount"] = Convert.ToDouble(fc["dramount"]);
                dt.Rows.Add(dr);
            }
            //Session.Remove("billdetails");
            Session["billdetails"] = dt;
            String[] tblheader = { "S.no", "trtype", "accountnumber", "dramount", "cramount", "Actions" };
            ViewBag.tblheader = tblheader;
            DataTable newdt = (DataTable)Session["billdetails"];
            ViewBag.totcr = totvalues("CR", newdt);
            ViewBag.totdr = totvalues("DR", newdt);
            return PartialView("GetBillDetails", newdt);
            //return Content("Saved");

        }

        public Int32 GetNewID(DataTable mydt) {
            var max=0;
            var col = mydt.DefaultView.Sort="id desc";
            if (mydt.Rows.Count > 0)
            {
                mydt = mydt.DefaultView.ToTable();
                max = Convert.ToInt32(mydt.Rows[0]["id"]) + 1;
            }
            else
                max = 1;
            return max;            
        }
        public ActionResult SaveVoucher(FormCollection fc) {
            if (Session["userid"] != null)
            {
                //Int32 abc = Convert.ToInt32(fc["billtypeid"].ToString());
                Int32 mid = Convert.ToInt32(Session["Userid"]);
                Users user = new Users();
                DataTable userdt = user.GetUserRecord(mid);
                DataTable cartdt = (DataTable)Session["billdetails"];
                Adjvoucher_master radt = new Adjvoucher_master();
                string trid = "";
                DateTime fst = (DateTime)Session["finstart"];
                DateTime fend = (DateTime)Session["finend"];
                int mydivid = Convert.ToInt32(Session["divid"]);
                String mydivname = Session["divname"].ToString();
                if (radt.SaveMyVoucher(fc, userdt, cartdt, ref  trid, fst, fend, mydivid, mydivname, Convert.ToInt32(Session["sectionid"]), Convert.ToInt32(Session["Userid"])))
                {
                    return Content("Transaction Saved Successfully With Voucher No : " + trid.ToString());
                }
                else
                {

                }
            }
            return Content("Saved");
        }

        public Boolean ValidateDate()
        {
            return false;
        }
		public ActionResult Voucher_masterEdit(Int32 Id)
        { 
            DataSet ds =new DataSet();
			DataTable dt = DBClass.GetAllRecords("select * from Voucher_master where id="+Id);
			dt.TableName = "Adjvoucher_master";
             ds.Tables.Add(dt);
            			DataTable dtbilltype_master = DBClass.GetData("select billtypeid,billtypename from billtype_master");
			ViewBag.billtype_master = new SelectList(dtbilltype_master.AsDataView(), "billtypeid", "billtypename", dt.Rows[0]["billtypeid"]);
						DataTable dtsection_master = DBClass.GetData("select sectionid,sectionname from section_master");
			ViewBag.section_master = new SelectList(dtsection_master.AsDataView(), "sectionid", "sectionname", dt.Rows[0]["sectionid"]);
						DataTable dtdiv_master = DBClass.GetData("select divid,divname from div_master");
			ViewBag.div_master = new SelectList(dtdiv_master.AsDataView(), "divid", "divname", dt.Rows[0]["divid"]);
						DataTable dtdepot_master = DBClass.GetData("select depotid,depotname from depot_master");
			ViewBag.depot_master = new SelectList(dtdepot_master.AsDataView(), "depotid", "depotname", dt.Rows[0]["depoid"]);
            DataTable party_master = DBClass.GetData("select partyid,partyname from party_master");
            ViewBag.party_master = new SelectList(party_master.AsDataView(), "partyid", "partyname", dt.Rows[0]["partyid"]);
            ViewBag.DialogTitle="Edit Adjvoucher_master";
            
            
            return View("editVoucher_master",ds);
        }
		
		public ActionResult Save(FormCollection fc){
            if (fc["id"] == null || fc["id"] == "")
            {
                Adjvoucher_master t = new Adjvoucher_master();
                Boolean res = t.InsertVoucher_master(fc);
                if (res) return Content("Record Inserted Successfully"); else return Content("Error Adding Record Try Again");
            }
            else 
            {
                Adjvoucher_master t = new Adjvoucher_master();
                Boolean res = t.UpdateVoucher_master(fc);
                if (res) return Content("Record Edited Successfully"); else return Content("Error Editing Record Try Again");
            }
            //return PartialView("SaveRec");
        }

        public ActionResult Voucher_masterdelete(int id)
        {
            Adjvoucher_master t = new Adjvoucher_master();
            String st = "";
            Boolean res = t.DeleteRecord(id,ref st);
            if (res) return Content("Record Deleted Successfully"); else return Content(st);
        }

        public ActionResult editAdjvoucher() {
            var list = new SelectList(new[] 
            {
                new { ID = "0", Name = "Select Type" },
                new { ID = "DB", Name = "Debit " },
                new { ID = "CR", Name = "Credit" },
            },
            "ID", "Name", 0);
            ViewBag.vtype = 1;
            ViewBag.list = list;
            DataTable dtaccount_master = DBClass.GetData("select accountnumber,accountname from account_master");
            ViewBag.account_master = new SelectList(dtaccount_master.AsDataView(), "accountnumber", "accountname");
            DataTable dt;
            dt = DBClass.GetData("Select * from billtype_details where billtypeid=0");
            dt.Columns[0].AllowDBNull=true;
            dt.Rows.Add();
            dt.Rows[0]["id"] = 0;
            ViewBag.row = dt.Rows[0];
            return PartialView("editdetail");

        }

        public ActionResult findvoucher() {
            GenHelper g = new GenHelper();
            DataTable dt= g.FindVouchers(Request["vtype"],Convert.ToInt32(Session["divid"]));
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            return PartialView("_findvoc",ds);
        }

        public JsonResult ValidateVono(String txtvocno)
        {
            Boolean jsonres = false;
            DataTable dt = DBClass.GetData("select * from voucher_master where voucherno='" + txtvocno + "'");
            if (dt.Rows.Count > 0)
            {
                if (Convert.ToInt32(Session["divid"]) == Convert.ToInt32(dt.Rows[0]["divid"])) {
                    jsonres= true;
                }
            }
            return Json(jsonres,JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAdjvoucher(FormCollection fc) {

            DataTable vdt = DBClass.GetData("select * from voucher_master where voucherno='" + fc["txtvocno"] + "'");
            vdt.TableName="vouchers";
            DataTable dtbilltype_master = DBClass.GetData("select billtypeid,billtypename from billtype_master");
                        ViewBag.billtype_master = new SelectList(dtbilltype_master.AsDataView(), "billtypeid", "billtypename",vdt.Rows[0]["billtypeid"]);
						DataTable dtsection_master = DBClass.GetData("select sectionid,sectionname from section_master");
			ViewBag.section_master = new SelectList(dtsection_master.AsDataView(), "sectionid", "sectionname");
						DataTable dtdiv_master = DBClass.GetData("select divid,divname from div_master");
			ViewBag.div_master = new SelectList(dtdiv_master.AsDataView(), "divid", "divname");
						DataTable dtdepot_master = DBClass.GetData("select depotid,depotname from depot_master");
			ViewBag.depot_master = new SelectList(dtdepot_master.AsDataView(), "depotid", "depotname");
            DataTable party_master = DBClass.GetData("select partyid,partyname from party_master");
            ViewBag.party_master = new SelectList(party_master.AsDataView(), "partyid", "partyname", vdt.Rows[0]["partyid"]);

            String Qry="SELECT dbo.billtype_details.id,dbo.billtype_details.billtypeid,dbo.billtype_details.accountnumber,dbo.billtype_details.type,dbo.billtype_details.dramount,dbo.billtype_details.cramount ";
                Qry+=" FROM dbo.billtype_details where billtypeid="+ vdt.Rows[0]["billtypeid"].ToString();
            DataTable billtype=DBClass.GetData(Qry);
            DataTable actr = DBClass.GetData("Select * from accounttrans where voucherno='" + fc["txtvocno"] + "'");
            String Qrybtd ="SELECT id,billtypeid,accountnumber,type,isnull((SELECT cramount from accounttrans where voucherno='"+fc["txtvocno"]+"' and accno= bd.accountnumber),0) cramount,";
            Qrybtd += " isnull((SELECT dramount from accounttrans where voucherno='" + fc["txtvocno"] + "' and accno= bd.accountnumber),0) dramount FROM dbo.billtype_details bd";
            DataTable bdt = DBClass.GetData(Qrybtd);
            foreach (DataColumn col in bdt.Columns)
                col.ReadOnly = false;
            bdt.TableName="billdetails";
            Session["billdetails"] = bdt;
            String[] tblheader = { "S.no", "trtype", "accountnumber", "dramount", "cramount", "Actions" };
            ViewBag.tblheader = tblheader;
            ViewBag.vocmast=vdt;
            ViewBag.billdetails=(DataTable)Session["billdetails"];
            ViewBag.totcr = totvalues("CR", bdt);
            ViewBag.totdr = totvalues("DR", bdt);
            return View("EditRadt");
        }

        public ActionResult UpdateVoucher(FormCollection fc)
        {
            if (Session["userid"] != null)
            {
                Int32 abc = Convert.ToInt32(fc["billtypeid"].ToString());
                Int32 mid = Convert.ToInt32(Session["Userid"]);
                Users user = new Users();
                DataTable userdt = user.GetUserRecord(mid);
                DataTable cartdt = (DataTable)Session["billdetails"];
                Adjvoucher_master radt = new Adjvoucher_master();
                string trid = "";
                DateTime fst = (DateTime)Session["finstart"];
                DateTime fend = (DateTime)Session["finend"];
                int mydivid = Convert.ToInt32(Session["divid"]);
                String mydivname = Session["divname"].ToString();
                if (radt.UpdateMyVoucher(fc, userdt, cartdt, ref  trid, fst, fend, mydivid, mydivname, Convert.ToInt32(Session["sectionid"]), Convert.ToInt32(Session["Userid"])))
                {
                    return Content("Transaction Saved Successfully With Voucher No : " + trid.ToString());
                }
                else
                {

                }
            }
            return Content("Saved");
        }
    }
}
