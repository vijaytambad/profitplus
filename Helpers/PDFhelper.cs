using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

namespace ProfitPlus.Helpers
{
    
    public class PDFhelper
    {
        DataTable reptab;
        PdfPTable tbl;
        float[] colwid;
        String[] repheaders;
        String[] pgheads;
        int mright = 36, mleft = 36, mtop = 144, mbottom = 72;
        String fname;
        Rectangle r = new Rectangle(595, 842);
        Stream fs;
        MemoryStream ms;

        public PDFhelper(DataTable mytable,float[] colw, String[] rheads,String[] phead,String filename) {
            reptab = mytable; colwid = colw; repheaders = rheads; fname = filename;
        }
        
        public void GenerateReport(){
            Document doc = new Document(r,mleft,mright,mtop,mbottom);
            
            fs= new FileStream( fname, FileMode.Create);
            Font tblfnt = new Font(Font.FontFamily.HELVETICA, 8, 0); 
            Font tblheadfnt = new Font(Font.FontFamily.HELVETICA, 12, 1); 
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            tbl = new PdfPTable(reptab.Columns.Count);
            tbl.SetWidths(colwid);
           
            tbl.TotalWidth = (595-mleft-mright); tbl.LockedWidth = true;
            tbl.SetWidths(colwid);
            
            tbl.HorizontalAlignment = 0;
            tbl.SpacingBefore = 10f;
            tbl.SpacingAfter = 10f;
            
            for (int i = 0; i < repheaders.Length; i++) {
                PdfPCell cell = new PdfPCell(new Phrase(repheaders[i],tblheadfnt));
                cell.Colspan = reptab.Columns.Count;
                cell.Border = 0;
                cell.HorizontalAlignment = 1;
                tbl.AddCell(cell);
            }
            
            for (int rh = 0; rh < reptab.Columns.Count; rh++)
            {

                tbl.AddCell(reptab.Columns[rh].ColumnName.ToUpper());
                
            }

            for (int row = 0; row < reptab.Rows.Count; row++)
            {
                for (int col = 0; col < reptab.Columns.Count; col++)
                {
                    PdfPCell cell;
                    String dtype = reptab.Rows[row][reptab.Columns[col]].GetType().Name;
                    if (reptab.Rows[row][reptab.Columns[col]].GetType().Name.Equals("Double")) {
                        cell = new PdfPCell(new Phrase(Convert.ToDouble(reptab.Rows[row][reptab.Columns[col]]).ToString("n2").ToString(),tblfnt));
                        
                        cell.HorizontalAlignment = 2;
                    }
                    else if (reptab.Rows[row][reptab.Columns[col]].GetType().Name.Equals("Int32")) {
                        cell = new PdfPCell(new Phrase(reptab.Rows[row][reptab.Columns[col]].ToString(), tblfnt));
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
            doc.Open();
            MyPager ev = new MyPager();
            writer.PageEvent = ev;
            doc.Add(tbl);
            doc.Close();
            //openpdf(fname);
        }


        public void GenerateReportnew()
        {
            Document doc = new Document(r, mleft, mright, mtop, mbottom);

            //fs = new FileStream(fname, FileMode.Create);
            fs=HttpContext.Current.Response.OutputStream;

            PdfWriter writer = PdfWriter.GetInstance(doc,fs );
            tbl = new PdfPTable(reptab.Columns.Count);
            tbl.SetWidths(colwid);

            tbl.TotalWidth = (595 - mleft - mright); tbl.LockedWidth = true;
            tbl.SetWidths(colwid);
            tbl.HorizontalAlignment = 0;
            tbl.SpacingBefore = 10f;
            tbl.SpacingAfter = 10f;

            /*for (int i = 0; i < repheaders.Length; i++)
            {
                PdfPCell cell = new PdfPCell(new Phrase(repheaders[i]));
                cell.Colspan = reptab.Columns.Count;
                cell.Border = 0;
                cell.HorizontalAlignment = 1;
                tbl.AddCell(cell);
            }*/

            for (int rh = 0; rh < reptab.Columns.Count; rh++)
            {
                tbl.AddCell(reptab.Columns[rh].ColumnName.ToUpper());

            }

            for (int row = 0; row < reptab.Rows.Count; row++)
            {
                for (int col = 0; col < reptab.Columns.Count; col++)
                {
                    tbl.AddCell(reptab.Rows[row][reptab.Columns[col]].ToString());

                }

            }
            doc.Open();
            MyPager ev = new MyPager();
            writer.PageEvent = ev;
            doc.Add(tbl);
            doc.Close();
            //openpdf(fname);
        }

        private void openpdf(String myfilename) {
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=test.pdf");
            HttpContext.Current.Response.ContentType = "application/pdf";
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.End();
        
        }
        
    }

    public partial class MyPager : PdfPageEventHelper {
        static int page = 1; String[] repheaders;
        /*public MyPager(String[] tblhead) { 
            repheaders=tblhead;
        }*/
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            
            Paragraph myfooter = new Paragraph("Thank Yor Page "+page.ToString(), FontFactory.GetFont(FontFactory.TIMES, 10, iTextSharp.text.Font.NORMAL));
            Paragraph myheader = new Paragraph("Welcome ");
            myheader.Alignment = Element.ALIGN_CENTER;
            Font fnt = new Font(Font.FontFamily.HELVETICA, 16,1);
            myheader.Font = fnt;
            PdfPTable headerTbl = new PdfPTable(1);
            headerTbl.TotalWidth = 523;
            headerTbl.HorizontalAlignment = Element.ALIGN_CENTER;
            PdfPCell cell = new PdfPCell(myheader);
            cell.Border = 0;
            cell.PaddingLeft = 10;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            headerTbl.AddCell(cell);
            headerTbl.WriteSelectedRows(0,-1, 15, 790, writer.DirectContent);
            PdfPTable headerfooter = new PdfPTable(1);
            PdfPCell cell1 = new PdfPCell(myfooter);
            cell1.Border = 0;
            cell1.PaddingLeft = 10;
            headerfooter.TotalWidth = 300;
            headerfooter.AddCell(cell1);
            headerfooter.WriteSelectedRows(0, -1, 15, 30, writer.DirectContent);
            page++;
        }
        
    }

    public class HeaderFooter:PdfPageEventHelper{
        
        public override void  OnEndPage(PdfWriter writer, Document document)
        {
 	         base.OnEndPage(writer, document);
             
             PdfContentByte cb = writer.DirectContent;
             ColumnText ct = new ColumnText(cb);
             Phrase myText = new Phrase("TEST paragraph\nNewline");
             ct.SetSimpleColumn(myText, 34, 50, 34, 110, 15, Element.ALIGN_CENTER);
             ct.Go();
        }
    }
}