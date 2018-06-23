using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Globalization;


namespace ProfitPlus.Helpers
{

    public class pdf
    {
        DataSet ds;
        public DataTable reptab;
        public PdfPTable tbl;
        public float[] colwid;
        public String[] repheaders;
        public String[] tblheaders;
        public int mright = 36, mleft = 72, mtop = 108, mbottom = 72;
        public String fname;
        public String UserTableName;
        Rectangle r = new Rectangle(595, 828);
        Stream fs;
        public int tablestart = 0;
        public String ReportName;
        public pdf (DataSet dataset,float[] colw, String[] rheads,String[] tblhead,String filename, String rname="",String Utblname="") {
            ds = dataset; colwid = colw; repheaders = rheads; fname = filename; tblheaders = tblhead; ReportName = rname;
            tablestart = 828 - mtop - mbottom;UserTableName=Utblname;
            reptab=ds.Tables[0];
        }
        
        public void GenerateReport(){
           
            Font tblfnt = new Font(Font.FontFamily.HELVETICA, 8, 0);
            Font tblfootfnt = new Font(Font.FontFamily.HELVETICA, 7, 1); 
            Font tblheadfnt = new Font(Font.FontFamily.HELVETICA, 10, 1); 
            
            tbl = new PdfPTable(reptab.Columns.Count);
            PdfPTable dochead = new PdfPTable(reptab.Columns.Count);
            tbl.SetWidths(colwid);
            tbl.TotalWidth = (595-mleft-mright); tbl.LockedWidth = true;
            tbl.HorizontalAlignment = 1;
            tbl.SpacingBefore = 10f;
            tbl.SpacingAfter = 10f;
            tbl.HeaderRows = 1; 
            
            /*if (repheaders.Length > 0)
            {
                for (int i = 0; i < repheaders.Length; i++)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(repheaders[i], tblheadfnt));
                    cell.Colspan = reptab.Columns.Count;
                    cell.Border = 0;
                    cell.HorizontalAlignment = 1;
                    dochead.AddCell(cell);
                }
            }*/
            for (int rh = 0; rh < reptab.Columns.Count; rh++)
            {
                PdfPCell c = new PdfPCell(new Phrase(tblheaders[rh].ToString().ToUpper(),tblheadfnt));
                tbl.AddCell(c); 
                
            }
            if (reptab.Rows.Count > 0)
            {
                for (int row = 0; row < reptab.Rows.Count; row++)
                {
                    for (int col = 0; col < reptab.Columns.Count; col++)
                    {
                        PdfPCell cell;
                        //String dtype = reptab.Rows[row][reptab.Columns[col]].GetType().Name;
                        if (reptab.Rows[row][reptab.Columns[col]].GetType().Name.Equals("Double"))
                        {
                            double colval = Convert.ToDouble(reptab.Rows[row][reptab.Columns[col]]);
                            String txtval = indcurr(colval);
                            cell = new PdfPCell(new Phrase(txtval, tblfnt));

                            cell.HorizontalAlignment = 2;
                        }
                        else if (reptab.Rows[row][reptab.Columns[col]].GetType().Name.Equals("Int32"))
                        {
                            double colval = Convert.ToDouble(reptab.Rows[row][reptab.Columns[col]]);
                            String txtval = indcurr(colval);
                            cell = new PdfPCell(new Phrase(txtval, tblfnt));
                            cell.HorizontalAlignment = 2;
                        }
                        else
                        {
                            cell = new PdfPCell(new Phrase(reptab.Rows[row][reptab.Columns[col]].ToString(), tblfnt));
                            cell.HorizontalAlignment = 0;
                        }
                        tbl.AddCell(cell);

                    }

                }

            }
            else {

                PdfPCell mycell = new PdfPCell(new Phrase("No Rows To Show",tblfnt));
                mycell.Colspan = colwid.Length;
                tbl.AddCell(mycell);

               
            }
            if (ReportName.Equals("TrialBalance")) {

                DataTable trbal = ds.Tables[1];
                PdfPCell tr1 = new PdfPCell(new Phrase("Totals : ", tblfootfnt)); tr1.Colspan = 3; tr1.HorizontalAlignment = 2;
                tbl.AddCell(tr1);
                double colval = Convert.ToDouble(trbal.Rows[0]["TotalCredits"]);
                String txtval = indcurr(colval);
                PdfPCell tr2=new PdfPCell(new Phrase(txtval.ToString(),tblfootfnt));
                tr2.HorizontalAlignment = 2;
                tbl.AddCell(tr2);
                colval = Convert.ToDouble(trbal.Rows[0]["TotalDebits"]);
                txtval = indcurr(colval);
                PdfPCell tr3=new PdfPCell(new Phrase(txtval,tblfootfnt));
                tr3.HorizontalAlignment = 2;
                tbl.AddCell(tr3);
                tbl.AddCell(new Phrase(""));
            }
            if (ReportName.Equals("ReceiptPayment"))
            {

                DataTable trbal = ds.Tables[1];
                PdfPCell tr1 = new PdfPCell(new Phrase("Totals : ", tblfootfnt)); tr1.Colspan = 2; tr1.HorizontalAlignment = 2;
                tbl.AddCell(tr1);
                double colval = Convert.ToDouble(trbal.Rows[0]["receipts"]);
                String txtval = indcurr(colval);
                PdfPCell tr2 = new PdfPCell(new Phrase(txtval, tblfootfnt));
                tr2.HorizontalAlignment = 2;
                tbl.AddCell(tr2);
                colval = Convert.ToDouble(trbal.Rows[0]["payments"]);
                txtval = indcurr(colval);
                PdfPCell tr3 = new PdfPCell(new Phrase(txtval, tblfootfnt));
                tr3.HorizontalAlignment = 2;
                tbl.AddCell(tr3);
                tbl.AddCell(new Phrase(""));
            }

            if (ReportName.Equals("placc"))
            {

                DataTable trbal = ds.Tables[1];
                PdfPCell tr1 = new PdfPCell(new Phrase("Totals : ", tblfootfnt)); tr1.Colspan = 2; tr1.HorizontalAlignment = 2;
                tbl.AddCell(tr1);
                double colval = Convert.ToDouble(trbal.Rows[0]["receipts"]);
                String txtval = indcurr(colval);
                PdfPCell tr2 = new PdfPCell(new Phrase(txtval, tblfootfnt));
                tr2.HorizontalAlignment = 2;
                tbl.AddCell(tr2);
                colval = Convert.ToDouble(trbal.Rows[0]["payments"]);
                txtval = indcurr(colval);
                PdfPCell tr3 = new PdfPCell(new Phrase("Totals : ", tblfootfnt)); tr3.Colspan = 2; tr3.HorizontalAlignment = 2;
                tbl.AddCell(tr3);
                PdfPCell tr4 = new PdfPCell(new Phrase(txtval, tblfootfnt));
                tr4.HorizontalAlignment = 2;
                tbl.AddCell(tr4);
                //tbl.AddCell(new Phrase(""));
            }

            //String rec= reptab.Compute("Sum(Receipts)", "").ToString();
            Document doc = new Document(r, mleft, mright, mtop, mbottom);
            fs = new FileStream(fname, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();
            PdfPageing ev = new PdfPageing(repheaders,tablestart+mtop+10,this);
            writer.PageEvent = ev;
            //doc.Add(dochead);
            doc.Add(tbl);
            doc.Close();
        }

        public String indcurr(double myval)
        {
            CultureInfo indian = new CultureInfo("hi-IN");
            String text = String.Format(indian, "{0:c}", myval);
            return text;
        }
        private void openpdf(String myfilename) {
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=test.pdf");
            HttpContext.Current.Response.ContentType = "application/pdf";
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.End();
        
        }
    }

    public partial class PdfPageing : PdfPageEventHelper
    {
        static int page = 1; DataTable dt;
        int tabletop=0;
        pdf mypdf;
        String[] RepHeads;
        public PdfPageing(String[] rhead,int tbltop,pdf pd)
        {
            RepHeads = rhead; tabletop = tbltop; mypdf = pd;
        }
        
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            PdfPTable headerTbl = new PdfPTable(1);
            Paragraph myfooter = new Paragraph("Page "+ page.ToString()+"...", FontFactory.GetFont(FontFactory.TIMES, 10, iTextSharp.text.Font.NORMAL));
            Font tblheadfnt = new Font(Font.FontFamily.HELVETICA, 14, 1);
            headerTbl.TotalWidth =mypdf.tbl.TotalWidth;
            headerTbl.HorizontalAlignment = Element.ALIGN_CENTER;
            if (RepHeads.Length > 0)
            {
                for (int i = 0; i < RepHeads.Length; i++)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(RepHeads[i], tblheadfnt));
                    cell.Border = 0;
                    cell.HorizontalAlignment = 1;
                    headerTbl.AddCell(cell);
                }
            }
            headerTbl.WriteSelectedRows(0, -1, mypdf.mleft, tabletop, writer.DirectContent);
            PdfPTable headerfooter = new PdfPTable(1);
            PdfPCell cell1 = new PdfPCell(myfooter);
            cell1.Border = 0;
            cell1.PaddingLeft = 10;
            cell1.HorizontalAlignment = 2;
            headerfooter.TotalWidth = mypdf.tbl.TotalWidth+mypdf.mleft+mypdf.mright;
            headerfooter.AddCell(cell1);
            headerfooter.WriteSelectedRows(0, -1, mypdf.mleft, 72, writer.DirectContent);
            page++;
        }

    }

}