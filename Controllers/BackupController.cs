using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProfitPlus.Helpers;
using System.IO;
using System.Net.Mime;

namespace ProfitPlus.Controllers
{
    public class BackupController : Controller
    {
        //
        // GET: /Backup/
        String serverpath = "";
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult backup()
        {
            serverpath = Server.MapPath("~/backups");
            string[] backuplist = Directory.GetFiles(@serverpath, "*.bak");
           
            /*String lst="<select name='flist' size='10' id='flist'>";
            
            for (Int32 i = 0; i < backuplist.Length; i++) { 
                lst+="<option>"+backuplist[i]+"</option>";
            }
            lst += "</select>";*/
            ViewBag.backlist = backuplist;
            ViewBag.msg = "";
            //ViewBag.flist = lst;
            return View("backupform");

        }

        public ActionResult backupdb()
        {
            serverpath = Server.MapPath("~/backups");

            GenHelper g = new GenHelper();
            string DatabaseName = "profitplus";
            string BackupName = DatabaseName + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString() + ".bak";
            try
            {
                SqlConnection con = DBClass.mycon();
                String Qry = "BACKUP DATABASE " + DatabaseName + " TO DISK = '" + serverpath + "\\" + BackupName + "' WITH FORMAT, MEDIANAME = 'Z_SQLServerBackups', NAME = '" + BackupName + "';";
                SqlCommand cmd = new SqlCommand(Qry, con);
                cmd.ExecuteNonQuery();
                con.Close();
                string[] backuplist = Directory.GetFiles(@serverpath, "*.bak");
                ViewBag.backlist = backuplist;
                ViewBag.msg = "Backup Successfully Created";
                return View("backupform");
            }
            catch (Exception ex)
            {
                g.logerror(ex);
                String Errmsg = "Error Creating Backup" + "<br/>" + ex.Message;
                return Content(Errmsg);
            }

        }

        public ActionResult restore()
        {
            serverpath = Server.MapPath("~/backups");
            GenHelper g = new GenHelper();
            String selectedfile;
            selectedfile = Request.QueryString["selfile"];
            string DatabaseName = "profitplus";
            string BackupName = selectedfile;
            try
            {
                SqlConnection sqlConnection = DBClass.mycon();
                string sqlQuery = "RESTORE DATABASE " + DatabaseName + " FROM DISK ='" + BackupName + "'";
                SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
                sqlCommand.CommandType = CommandType.Text;
                int iRows = sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
                String message = "The " + DatabaseName + " database restored with the name " + BackupName + " successfully...";
                return Content(message);
                
            }
            catch (SqlException ex)
            {
                g.logerror(ex);
                 String errmessage = "Cannot Restore " + DatabaseName + " with the name " + BackupName + "... Try Again";
                 return Content(errmessage);
            }
            
            
        }

        public FileResult Download()
        {
            String selectedfile = Request.QueryString["selfile"];
            byte[] fileBytes = System.IO.File.ReadAllBytes(selectedfile);
            string fileName = Path.GetFileName(selectedfile);
            return File(fileBytes, "application/bak", fileName);
        }

        public ActionResult delfile() {
            String fname = Request.QueryString["selfile"]; 
            System.IO.File.Delete(fname);
            return Redirect("/backup/backup");
        }
    }
}
