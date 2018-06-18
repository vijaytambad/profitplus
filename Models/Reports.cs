using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProfitPlus.Helpers;

namespace ProfitPlus.Models
{
    public class Reports
    {
        public DataTable GetRpData(String sdate, String edate) {
            GenHelper g = new GenHelper();
            String Qry="SELECT [at].accountnumber, am.accountname,Sum([at].dramount) AS Receipts,Sum([at].cramount) AS Payments FROM dbo.account_master AS am";
            Qry += " INNER JOIN dbo.accounttrans AS [at] ON am.accountnumber = [at].accountnumber WHERE [at].divid = 1 and trdate>='"+g.SqlDate(sdate) +"' and trdate<='"+g.SqlDate(edate)+"' GROUP BY [at].accountnumber,am.accountname";
            return DBClass.GetData(Qry);
        }


        public DataTable PrepareTraialBal(String tbl,String startdate,String enddate) { 
            String Createtbl="CREATE TABLE [dbo].["+tbl+"] (";
            Createtbl += "[id] int NULL ,[accountnumber] varchar(20) NULL ,[accountname] varchar(80) NULL ,[parentid] int,[groupid] int,[opbal] float(53) NULL ,[receipts] float(53) NULL ,[payments] float(53) NULL ,[clbal] float(53) NULL )";
            DBClass.NonQuery(Createtbl);
            String AcQry = "insert into " + tbl + "(accountnumber,accountname,parentid,groupid)  select accountnumber,accountname,parentid,groupid from account_master";
            DBClass.NonQuery(AcQry);
            String OpQry = "update "+tbl+"   set opbal=(select ISNULL(sum(cramount)-SUM(dramount),0) from accounttrans as t2 where mytrialbal.accountnumber=t2.accountnumber and trdate<'"+startdate+"' and divid=1)";
            DBClass.NonQuery(OpQry);
            String RPQry = "update "+tbl+"   set receipts=(select ISNULL(sum(cramount),0) from accounttrans as t2 where mytrialbal.accountnumber=t2.accountnumber and trdate>='"+startdate+"' and trdate<='"+enddate+"' and divid=1)";
            RPQry += ",payments=(select ISNULL(sum(dramount),0) from accounttrans as t3 where mytrialbal.accountnumber=t3.accountnumber and trdate>='"+startdate+"' and trdate<='"+enddate+"' and divid=1)";
            DBClass.NonQuery(RPQry);
            String ClQry = "update "+tbl+" set clbal=isnull(opbal+receipts-payments,0)";
            DBClass.NonQuery(ClQry);
            //String TrQry = "SELECT accountnumber,accountname,SUM(CASE WHEN clbal < 0 THEN clbal ELSE 0 END) AS Debit,";
            //TrQry += "SUM(CASE WHEN clbal > 0 THEN clbal ELSE 0 END) AS credit FROM mytrialbal where clbal!=0  GROUP BY accountnumber,accountname"; 
            String TrQry="select accountnumber,accountname,opbal,receipts,payments,clbal from "+tbl +" where clbal!=0";
            return DBClass.GetData(TrQry);

        }

        public DataTable GetPLAccount()
        {
            String SumQry = "select (select sum(clbal) from mytrialbal t1 where parentid=4) Revenue,(select sum(clbal) from mytrialbal t2 where parentid=5) Expenses";
            DataTable SumTbl = DBClass.GetData(SumQry);
            Double pl = Convert.ToDouble(SumTbl.Rows[0]["Revenue"]) - Math.Abs(Convert.ToDouble(SumTbl.Rows[0]["expenses"]));
            if (pl < 0)
            {
                String InsQry = "insert into mytrialbal(accountnumber,accountname,parentid,clbal) values(499999,'Loss For The Year',4," + Math.Abs(pl).ToString() + ")";
                DBClass.NonQuery(InsQry);
            }
            else {
                String InsQry = "insert into mytrialbal(accountnumber,accountname,parentid,clbal) values(599999,'Profit For The Year',5," + Math.Abs(pl).ToString() + ")";
                DBClass.NonQuery(InsQry);
            }
            
            String PlQry="delete from receipts;";
                    PlQry+="insert into receipts(sno,accountnumber,accountname,amount) ";
                    PlQry+="(select ROW_NUMBER()  OVER (ORDER BY  accountnumber) As sno,accountnumber,accountname,sum(clbal)";
                    PlQry+=" from mytrialbal where clbal!=0 and parentid=4 group by accountnumber,accountname);";
                    PlQry+="delete from payments;";
                    PlQry+="insert into payments(sno,accountnumber,accountname,amount) ";
                    PlQry+="(select ROW_NUMBER()  OVER (ORDER BY  accountnumber) As sno,accountnumber,accountname,abs(sum(clbal))";
                    PlQry += " from mytrialbal where clbal!=0 and parentid=5 group by accountnumber,accountname)";
                    String DispQry;
            DBClass.NonQuery(PlQry);
            if (pl > 0)
            {
                 DispQry = "SELECT r.accountnumber,r.accountname,r.amount,p.accountnumber,p.accountname,p.amount FROM receipts AS r right JOIN dbo.payments AS p ON r.sno = p.sno";
            }
            else
            {
                 DispQry = "SELECT r.accountnumber,r.accountname,r.amount,p.accountnumber,p.accountname,p.amount FROM receipts AS r left JOIN dbo.payments AS p ON r.sno = p.sno";
            }
            return DBClass.GetData(DispQry);
        }
    }

}