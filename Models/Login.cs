using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ProfitPlus.Helpers;

namespace ProfitPlus.Models
{
    public class Login
    {

        public Int32 ID { get; set; }
        public String Uname { get; set; }
        public String Pwd { get; set; }

        public Boolean Logedin(FormCollection f)
        {
            String Qry = "SELECT dbo.div_master.divid,dbo.section_master.sectionid,dbo.users.id,dbo.users.username,dbo.users.password,dbo.users.usertypeid,dbo.user_types.usertypename,dbo.section_master.sectionname,dbo.div_master.divname";
            Qry+=" FROM dbo.users INNER JOIN dbo.user_types ON dbo.user_types.usertypeid = dbo.users.usertypeid";
            Qry+=" INNER JOIN dbo.div_master ON dbo.users.divid = dbo.div_master.divid";
            Qry+= " INNER JOIN dbo.section_master ON dbo.section_master.sectionid = dbo.users.sectionid";
            Qry+=" where username='"+f["username"]+"' and password='"+f["pwd"]+"'";
            DataTable  dt = DBClass.GetAllRecords(Qry);
            if (dt.Rows.Count > 0)
            {
                HttpContext.Current.Session["Userid"] = dt.Rows[0]["id"];
                HttpContext.Current.Session["UserName"] = dt.Rows[0]["username"];
                HttpContext.Current.Session["UserTypeId"] = dt.Rows[0]["usertypeid"];
                HttpContext.Current.Session["UserTypeName"] = dt.Rows[0]["usertypename"];
                HttpContext.Current.Session["SectionName"] = dt.Rows[0]["sectionname"];
                HttpContext.Current.Session["divname"] = dt.Rows[0]["divname"];
                HttpContext.Current.Session["divid"] = dt.Rows[0]["divid"];
                HttpContext.Current.Session["sectionid"] = dt.Rows[0]["sectionid"];
                return true;
            }
            else
            {
                return false;
            }
        }

        private DataTable GetMenu(Int32 uid)
        {
            String Qry = "SELECT dbo.permissions.user_type,dbo.usertypes.user_type_name,dbo.permissions.user_option,dbo.menuoptions.menuname,";
            Qry += "dbo.menuoptions.menuid,dbo.menuoptions.menuurl,dbo.menuoptions.parentid ";
            Qry += "FROM dbo.permissions INNER JOIN dbo.usertypes ON dbo.usertypes.user_type_id = dbo.permissions.user_type ";
            Qry += "INNER JOIN dbo.menuoptions ON dbo.permissions.user_option = dbo.menuoptions.menuid WHERE ";
            Qry += "dbo.usertypes.user_type_id =" + uid.ToString();
            DataTable dt = DBClass.GetAllRecords(Qry);
            return dt;
        }
        public String indcurr(double myval) {
            CultureInfo indian = new CultureInfo("hi-IN");
            String text = String.Format(indian, "{0:c}", myval);
            return text;
        }
        
    }
}