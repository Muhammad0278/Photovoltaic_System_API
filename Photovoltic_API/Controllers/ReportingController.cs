using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;
using System.Web.Http;
using photovoltaic_API.Controllers;
using System.Web.Script.Serialization;
using RouteAttribute = System.Web.Http.RouteAttribute;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using Photovoltic_API.Models;
using iTextSharp.text.pdf.parser;
//using System.Windows.Forms.DataVisualization.Charting;



namespace Photovoltic_API.Controllers
{
    [System.Web.Http.RoutePrefix("api/ReportingModule")]
    public class ReportingController : ApiController
    {
        static Font HeaderTitle = FontFactory.GetFont("Helvetica", 12, Font.BOLD, BaseColor.BLACK);
        static Font HeaderSubTitle = FontFactory.GetFont("Helvetica", 10, Font.BOLD, BaseColor.BLACK);
        static Font NormalFont = FontFactory.GetFont("Helvetica", 10, Font.NORMAL, BaseColor.BLACK);
        // GET: Reporting
        DB_WeatherEntities DB = new DB_WeatherEntities();
        Helper_Reporting HP = new Helper_Reporting();
        [Route("GenerateReport")]
        [HttpGet]
        public Response GetReport(tbl_ProductAssignment tlbAss)
        {
            var res = new Response();
            var Path= GenerateReport(tlbAss);
            if (Path != "")
            {
                res.Code = 200;
                res.Detail = Path;
               
            }
            else
            {
                res.Code = 401;
                res.Detail = "";
            }
           
            return res;
        }
        public string GenerateReport(tbl_ProductAssignment objtable)
        {
            var json = "";
            var res = new Response();

            JavaScriptSerializer _jss = new JavaScriptSerializer();
            try
            {
                var query = (from ass in DB.tbl_ProductAssignment
                             join proj in DB.tbl_Projects on ass.ProjectID equals proj.ProjectID
                             join prod in DB.tbl_Products on ass.ProductID equals prod.ProductID
                             where proj.IsActive == true && ass.ID== Convert.ToInt32(objtable.ID)
                             select new  {  ass,   proj,  prod    }).FirstOrDefault();

                var newguid = Guid.NewGuid();
                var tblWeather = DB.tbl_WeatherData.Where(x => x.ProductAssignmentID== objtable.ID && x.ProjectID== objtable.ProjectID && x.ProductID== objtable.ProductID).ToList();
                #region  Directory Information
                string folder = HttpContext.Current.Server.MapPath("~/TempReport");
                string Chartfolder = HttpContext.Current.Server.MapPath("~/TempPdf");
                System.IO.DirectoryInfo Chartdir = new System.IO.DirectoryInfo(Chartfolder);
                string path = "/TempReport/Project_Report_" + newguid + ".pdf";
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(folder);
                folder = HttpContext.Current.Server.MapPath("~/" + path);
                if (!dir.Exists)
                    dir.Create();

                var exfiles = dir.GetFiles();
                foreach (var item in exfiles)
                {
                    try
                    {
                        item.Delete();
                    }
                    catch (Exception ex)
                    {
                        // this means the file is in use...no biggie...we will get it next time.
                        continue;
                    }

                }

                if (!Chartdir.Exists)
                    Chartdir.Create();

                var exfilesChar = Chartdir.GetFiles();
                foreach (var item in exfilesChar)
                {
                    try
                    {
                        item.Delete();
                    }
                    catch (Exception ex)
                    {
                        // this means the file is in use...no biggie...we will get it next time.
                        continue;
                    }

                }
                #endregion


                using (MemoryStream memoryStream = new MemoryStream())
                {
                    Document document = new Document(PageSize.A4, 10, 10, 20, 20);
                    iTextSharp.text.pdf.PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    //PAPResultPrintHeaderFooter printheaderFooter = new PAPResultPrintHeaderFooter();

                    //PrintHeaderFooter printheaderFooter = new PrintHeaderFooter();
                    //writer.PageEvent = printheaderFooter;

                    document.Open();
                    var docSize = document.PageSize;
                    //  document.SetMargins(20, 20, 100, 20);

                    var MainTable = new PdfPTable(1);
                    MainTable.WidthPercentage = 100;
                    MainTable.KeepTogether = true;
                    MainTable.SplitLate = false;
                    MainTable.DefaultCell.Padding = 0;

                    var MainFianlBody = HP.CreateTable(1, Rectangle.NO_BORDER, 100f, 0f, 0f);

                    //  MainFianlBody.HeaderRows = 1;
                    var MainHeader = HP.CreateTable(new float[] { 1.5f, 0.5f }, Rectangle.NO_BORDER, 95f, 0f, 0f);
                    string TestCode = string.Empty;
                    string InstrumentName = string.Empty;

                    var SubHeader = HP.CreateTable(1, Rectangle.NO_BORDER, 90f, 0f, 0f);

                    SubHeader.AddCell(HP.CreateCell("Company Name: ABC LTD ", HeaderSubTitle, 0, Element.ALIGN_LEFT));
                    SubHeader.AddCell(HP.CreateCell("Address: MKT 204 Hill ", HeaderSubTitle, 0, Element.ALIGN_LEFT));
                    SubHeader.AddCell(HP.CreateCell("City:Chemnitz", HeaderSubTitle, 0, Element.ALIGN_LEFT));
                    SubHeader.AddCell(HP.CreateCell("Mob: 1234567", HeaderSubTitle, 0, Element.ALIGN_LEFT));
                    string period = "Period:  Instrument: " + InstrumentName;
                    SubHeader.AddCell(HP.CreateCell("Email: abc@gmail.com\t\t", HeaderSubTitle, 0, Element.ALIGN_LEFT));
                    SubHeader.AddCell(HP.CreateCell("\n", HeaderSubTitle, 0, Element.ALIGN_LEFT));
                    MainHeader.AddCell(HP.CreateCell("", HeaderSubTitle, 0, Element.ALIGN_LEFT));
                    MainHeader.AddCell(HP.CreateCellPadding(SubHeader, 0f, 0f, 0f, 0f, 0f));
                    MainFianlBody.AddCell(HP.CreateCellPadding(MainHeader, 0f, 0f, 0f, 1f, 0f));

                    var tblbody = HP.CreateTable(new float[] { 1f }, Rectangle.NO_BORDER, 85f, 5f, 10f);

                    //MainFianlBody.AddCell(CreatePaddingCellBorderMedication("Principle Diagnostics LLC 2550 Brodhead Rd, Suite 105 Bethlehem, PA 18020", HeaderTitle, 0, Element.ALIGN_LEFT, 2f, 1f, 0f, 0f, 0f, 0f, 2f, 0f, new BaseColor(224, 224, 224), 0));
                    //  tblbody.AddCell(HP.CreatePaddingCellBorder(dtStart.ToShortDateString() + " - " + dtEnd.ToShortDateString(), Patientinform, 0, Element.ALIGN_LEFT, 0f, 5f, 0f, 0f, 0f, 0f, 2f, 0f, new BaseColor(225, 225, 225), 0));
                    var CHunksVlaue1 = HP._ParaChunks("Project Name: "+ query.proj.ProjectName, "", HeaderSubTitle, NormalFont);
                    tblbody.AddCell(HP.CreateCell(CHunksVlaue1, NormalFont, 0, 0));

                    var PCHunksVlaue1 = HP._ParaChunks("Product Name: ", query.prod.ProductName, HeaderSubTitle, NormalFont);
                    tblbody.AddCell(HP.CreateCell(PCHunksVlaue1, NormalFont, 0, 0));

                    var Wattage = HP._ParaChunks("Wattage: ", Convert.ToString(query.prod.Wattage), HeaderSubTitle, NormalFont);
                    tblbody.AddCell(HP.CreateCell(Wattage, NormalFont, 0, 0));

                    var Warranty = HP._ParaChunks("Warranty Years: ", Convert.ToString(query.prod.WarrantyYears), HeaderSubTitle, NormalFont);
                    tblbody.AddCell(HP.CreateCell(Warranty, NormalFont, 0, 0));

                    var Price = HP._ParaChunks("Price: ", Convert.ToString(query.prod.Price), HeaderSubTitle, NormalFont);
                    tblbody.AddCell(HP.CreateCell(Price, NormalFont, 0, 0));

                    var Power = HP._ParaChunks("Power Peak: ", Convert.ToString(query.prod.Powerpeak), HeaderSubTitle, NormalFont);
                    tblbody.AddCell(HP.CreateCell(Power, NormalFont, 0, 0));

                    MainFianlBody.AddCell(HP.CreateCellPadding(tblbody, 0f, 0f, 0f, 0f, 0f));
                    // MainFianlBody.AddCell("This is the main");


                    var ChartPath = GetChart1(tblWeather);
                    string add = HttpContext.Current.Server.MapPath("~/" + ChartPath);
                    iTextSharp.text.Image Graphlogo = iTextSharp.text.Image.GetInstance(add, true);

                    var GraphlogoCell = new PdfPCell();
                    GraphlogoCell.PaddingTop = 0f;
                    GraphlogoCell.BorderWidthBottom = 0;
                    GraphlogoCell.BorderWidthLeft = 0;
                    GraphlogoCell.BorderWidthRight = 0;
                    GraphlogoCell.BorderWidthTop = 0;
                    GraphlogoCell.AddElement(Graphlogo);
                    MainFianlBody.AddCell(GraphlogoCell);


                    var tblresultinfo = HP.CreateTable(new float[] { 1f, 1f, 1f, 1f, 1f }, Rectangle.NO_BORDER, 90f, 5f, 5f);
                    tblresultinfo.AddCell(HP.CreatePaddingCellBordercolor("Date/Time", NormalFont, 0, Element.ALIGN_LEFT, 1f, 4f, 1f, 1f, 1f, 1f, 2f, 0f, new BaseColor(244, 244, 244), new BaseColor(0, 0, 0)));
                    tblresultinfo.AddCell(HP.CreatePaddingCellBordercolor("Solar Irradiance ", NormalFont, 0, Element.ALIGN_LEFT, 1f, 4f, 1f, 1f, 1f, 1f, 2f, 0f, new BaseColor(244, 244, 244), new BaseColor(0, 0, 0)));
                    tblresultinfo.AddCell(HP.CreatePaddingCellBordercolor("Sunrise", NormalFont, 0, Element.ALIGN_LEFT, 1f, 4f, 1f, 1f, 1f, 1f, 2f, 0f, new BaseColor(244, 244, 244), new BaseColor(0, 0, 0)));
                    tblresultinfo.AddCell(HP.CreatePaddingCellBordercolor("Sunrise", NormalFont, 0, Element.ALIGN_LEFT, 1f, 4f, 1f, 1f, 1f, 1f, 2f, 0f, new BaseColor(244, 244, 244), new BaseColor(0, 0, 0)));

                    tblresultinfo.AddCell(HP.CreatePaddingCellBordercolor("Electricity", NormalFont, 0, Element.ALIGN_LEFT, 1f, 4f, 1f, 1f, 1f, 1f, 2f, 0f, new BaseColor(244, 244, 244), new BaseColor(0, 0, 0)));
                    tblresultinfo.HeaderRows = 1;
                    foreach (var item in tblWeather)
                    {
                        tblresultinfo.AddCell(HP.CreatePaddingCellBordercolor( Convert.ToDateTime(item.CreatedDate).ToString("dd MM yyyy"), NormalFont, 0, Element.ALIGN_LEFT, 1f, 4f, 1f, 0f, 1f, 1f, 2f, 0f, new BaseColor(255, 255, 255), new BaseColor(0, 0, 0)));
                        tblresultinfo.AddCell(HP.CreatePaddingCellBordercolor(Convert.ToString(item.SolarIrradiance_Value), NormalFont, 0, Element.ALIGN_LEFT, 1f, 4f, 0f, 0f, 1f, 1f, 2f, 0f, new BaseColor(255, 255, 255), new BaseColor(0, 0, 0)));
                        tblresultinfo.AddCell(HP.CreatePaddingCellBordercolor(Convert.ToDateTime(item.Sunrise).ToString("dd MM yyyy"), NormalFont, 0, Element.ALIGN_LEFT, 1f, 4f, 0f, 0f, 1f, 1f, 2f, 0f, new BaseColor(255, 255, 255), new BaseColor(0, 0, 0)));
                        tblresultinfo.AddCell(HP.CreatePaddingCellBordercolor(Convert.ToDateTime(item.Sunset).ToString("dd MM yyyy"), NormalFont, 0, Element.ALIGN_LEFT, 1f, 4f, 0f, 0f, 1f, 1f, 2f, 0f, new BaseColor(255, 255, 255), new BaseColor(0, 0, 0)));
                        tblresultinfo.AddCell(HP.CreatePaddingCellBordercolor(item.CalElectricity, NormalFont, 0, Element.ALIGN_LEFT, 1f, 4f, 0f, 0f, 1f, 1f, 2f, 0f, new BaseColor(255, 255, 255), new BaseColor(0, 0, 0)));
                    }
                   

                    MainFianlBody.AddCell(HP.CreateCellPadding(tblresultinfo, 0f, 0f, 0f, 0f, 0f));
                    document.Add(MainFianlBody);
                    document.Close();
                    writer.Close();
                    byte[] content = memoryStream.ToArray();

                    // Write out PDF from memory stream.
                    using (FileStream fs = File.Create(folder))
                    {
                        fs.Write(content, 0, (int)content.Length);
                    }


                   
                    json = folder;
                }

            }
            catch (Exception ex)
            {
                res.Code = 404;
                res.Message = "Bad Resquest";
                res.Detail = ex.Message;
            }
           // json = _jss.Serialize(res);
            return json;

        }
        public static string GetChart1(List<tbl_WeatherData> _obj)
        {

            string path = string.Empty;
            try

            {


                Chart chart1 = new Chart();
                chart1.Width = new System.Web.UI.WebControls.Unit(1100, System.Web.UI.WebControls.UnitType.Pixel);
                chart1.Height = new System.Web.UI.WebControls.Unit(400, System.Web.UI.WebControls.UnitType.Pixel);

                chart1.Series.Clear();
                chart1.ChartAreas.Clear();

                // Create a new chart area
                var chartArea = new ChartArea();

                chart1.ChartAreas.Add(chartArea);
                chart1.ChartAreas["ChartArea1"].AxisY.Title = "Electricity";
                chart1.ChartAreas["ChartArea1"].AxisX.Title = "Date";
                chart1.ChartAreas["ChartArea1"].AxisY.TitleFont = new System.Drawing.Font("Verdana", 12, System.Drawing.FontStyle.Bold);
                chart1.ChartAreas["ChartArea1"].AxisX.TitleFont = new System.Drawing.Font("Verdana", 12, System.Drawing.FontStyle.Bold);
                // Create a new series
                var series = new Series();
                series.ChartType = SeriesChartType.Bar;
                series.ChartType = SeriesChartType.Column;
                // Add data points to the series
                foreach (var data in _obj)
                {
                    series.Points.AddXY(Convert.ToDateTime(data.CreatedDate).ToString("dd MM yyyy"), data.CalElectricity);
                }

                // Add the series to the chart
                chart1.Series.Add(series);



                var newguid = Guid.NewGuid();
                string filename = HttpContext.Current.Server.MapPath("~/TempPDF") + "/" + newguid + "Chart.png";
                path = "TempPDF/" + newguid + "Chart.png";
                chart1.SaveImage(filename, ChartImageFormat.Png);

            }
            catch (Exception ex)
            {

            }
            return path;
        }


    }
}