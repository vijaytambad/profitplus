using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProfitPlus.Models;
using System.Data;
using ProfitPlus.Helpers;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ProfitPlus.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult VerifyLogin(FormCollection fc)
        {
            Login lg = new Login();
            if (lg.Logedin(fc))
            {
                Session["Menu"]= GenerateMenu();
                
                DataTable dt = DBClass.GetAllRecords("select * from finyear");
                Session["finstart"] = dt.Rows[0]["finyearbegin"];
                Session["finend"] = dt.Rows[0]["finyearend"];
                return View("Dashboard");
            }
            else
            {
                return View("index");
            }
           
        }

        public ActionResult LogOut()
        {
            Session.Abandon();
            return Redirect("~/Login/Index");
        }

        private String GenerateMenu()
        {
            String mystr="";
            int utype = Convert.ToInt16(Session["UserTypeId"]);
            String Qry="SELECT dbo.user_menus.menuid,dbo.user_menus.menuname,dbo.user_menus.menuurl ";
            Qry += " FROM dbo.user_menus";
            Qry+=" INNER JOIN dbo.user_permissions ON dbo.user_menus.menuid = dbo.user_permissions.menuid";
            Qry+=" INNER JOIN dbo.user_types ON dbo.user_permissions.usertypeid = dbo.user_types.usertypeid";
            Qry+=" WHERE dbo.user_types.usertypeid ="+utype.ToString();
            DataTable dt=DBClass.GetAllRecords(Qry) ;
            if(dt.Rows.Count>0)
            {
                for(int i=0;i<dt.Rows.Count;i++)
                {
                    mystr += "<li><a id='"+i.ToString()+"' href='" + dt.Rows[i]["menuurl"].ToString() + "'>" + dt.Rows[i]["menuname"].ToString() + "</a></li>";
                }
            }
            return mystr;
        }

        public FileStreamResult pdf()
        {
            String resstr = PartialView("index", null).ToString();
              //String strView = RenderPartialViewToString("index", null);
            // String strView = RenderPartialViewToString("addCadtVoucher", null);
            MemoryStream workStream = new MemoryStream();
            Document document = new Document();
            PdfWriter.GetInstance(document, workStream).CloseStream = false;

            document.Open();
            //document.Add(new Paragraph("Hello World"));
             document.Add(new Paragraph(resstr));
            document.Add(new Paragraph(DateTime.Now.ToString()));
            document.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;
            
            return new FileStreamResult(workStream, "application/pdf");
        }
        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            if (model != null)
                ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
