using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using photovoltaic_API.Controllers;
using Photovoltic_API.Models;
using System.Security.Cryptography;
using Microsoft.Ajax.Utilities;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Data.Entity;
using System.Web.UI;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
using System.Runtime.ConstrainedExecution;
using System.Data.Entity.Core.Objects;

namespace Photovoltic_API.Controllers
{
    [System.Web.Http.RoutePrefix("api/FetchWeather")]
    public class FetchingWeatherController : ApiController
    {
        DB_WeatherEntities DB = new DB_WeatherEntities();
        [Route("GetMapProductsByProID")]
        [HttpGet]
        public object GetPrByProjectID(int UserID, int ProjectID, bool Status)
        {
            var json = "";
            var resp = new Response();
            JavaScriptSerializer _jss = new JavaScriptSerializer();
            try
            {
                var query = (from a in DB.tbl_ProductAssignment
                             join pj in DB.tbl_Projects on a.ProjectID equals pj.ProjectID
                             join pt in DB.tbl_Products on a.ProductID equals pt.ProductID
                             where a.UserID == UserID && a.ProjectID == ProjectID &&
                            pj.IsActive == Status
                             select new
                             {
                                 a.ID,
                                 a.ProjectName,
                                 a.ProjectID,
                                 a.ProductName,
                                 a.ProductID,
                                 pj.Description,
                                 pt.Wattage,
                                 pt.WarrantyYears,
                                 pt.Price,
                                 ptDes = pt.Description,
                                 a.ImagePath,
                                 pj.IsActive,
                                 Latitude = a.LatitudeNew,
                                 Longitude = a.LongitudeNew

                             }).ToList();



                if (query.Count > 0)
                {
                    resp.Code = 200;
                    resp.Status = "success";
                    resp.Message = "Details successfully..";
                    resp.data = query;

                }
                else
                {
                    resp.Code = 401;
                    resp.Status = "Not Found";
                    resp.Message = "Record Not Found..";


                }

            }
            catch (Exception ex)
            {
                resp.Code = 404;
                resp.Status = "Bad Resquest";
                resp.Message = ex.Message;
            }
            json = _jss.Serialize(resp);
            return json;
        }

        [Route("DailyForcast")]
        public async Task<IHttpActionResult> GetWeatherData()

        {
            List<tbl_HistoryWeather> lsthistory = new List<tbl_HistoryWeather>();
            JavaScriptSerializer ser = new JavaScriptSerializer();
            try
            {
                string apiKey = "apah899bLstduZGLWkICXLq8U2SbPkeq";
                var query = (from a in DB.tbl_ProductAssignment
                             join pj in DB.tbl_Projects on a.ProjectID equals pj.ProjectID
                             join pt in DB.tbl_Products on a.ProductID equals pt.ProductID
                             where pj.IsActive == true
                             select new
                             {
                                 a.ID,
                                 a.ProjectName,
                                 a.ProjectID,
                                 a.ProductName,
                                 a.ProductID,
                                 pj.Description,
                                 pt.Wattage,
                                 pt.WarrantyYears,
                                 pt.Price,
                                 ptDes = pt.Description,
                                 a.ImagePath,
                                 pj.IsActive,
                                 Latitude = a.Latitude,
                                 Longitude = a.Longitude

                             }).ToList();

                foreach (var _item in query)
                {
                    double targetLatitude = Convert.ToDouble(_item.Latitude);   // The target latitude value
                    double targetLongitude = Convert.ToDouble(_item.Longitude); // The target longitude value
                    DateTime dt = DateTime.Now.Date;
                    var tblProjects = DB.tbl_HistoryWeather.Where(x => x.Latitude == targetLatitude && x.Longitude == targetLongitude && EntityFunctions.TruncateTime(x.CreatedDate) == dt).FirstOrDefault();

                    if (tblProjects == null)
                    {
                        string location = Convert.ToString(_item.Latitude) + ',' + Convert.ToString(_item.Longitude);
                        // int key = 1012635;
                        //  Get Location Key
                        Task<int> key = GetLocation(apiKey, location);

                        // Get Daily Forcast Weather Data
                        var accujson = GetDailyData(key.Result, apiKey);
                        var accuWeatherData = ser.Deserialize<DailyWeatherdata.Root>(accujson.Result);
                        DateTime? dat = null;
                        tbl_HistoryWeather data = new tbl_HistoryWeather
                        {

                            Sunrise = (accuWeatherData != null && accuWeatherData.DailyForecasts != null && accuWeatherData.DailyForecasts[0].Sun != null) ? accuWeatherData.DailyForecasts[0].Sun.Rise : dat,
                            Sunset = (accuWeatherData != null && accuWeatherData.DailyForecasts != null && accuWeatherData.DailyForecasts[0].Sun != null) ? accuWeatherData.DailyForecasts[0].Sun.Set : dat,
                            SolarIrradiance_Value = (accuWeatherData != null && accuWeatherData.DailyForecasts != null && accuWeatherData.DailyForecasts[0].Day != null && accuWeatherData.DailyForecasts[0].Day.SolarIrradiance != null) ? accuWeatherData.DailyForecasts[0].Day.SolarIrradiance.Value : 0.0,
                            SolarIrradiance_Unit = (accuWeatherData != null && accuWeatherData.DailyForecasts != null && accuWeatherData.DailyForecasts[0].Day != null && accuWeatherData.DailyForecasts[0].Day.SolarIrradiance != null) ? accuWeatherData.DailyForecasts[0].Day.SolarIrradiance.Unit : "",
                            SolarIrradiance_UnitType = (accuWeatherData != null && accuWeatherData.DailyForecasts != null && accuWeatherData.DailyForecasts[0].Day != null && accuWeatherData.DailyForecasts[0].Day.SolarIrradiance != null) ? accuWeatherData.DailyForecasts[0].Day.SolarIrradiance.UnitType : string.Empty,
                            Latitude = Convert.ToDouble(_item.Latitude),
                            Longitude = Convert.ToDouble(_item.Longitude),
                            CreatedDate = DateTime.Now
                        };
                        if (data != null)
                            lsthistory.Add(data);

                    }
                }
                if (lsthistory.Count > 0)
                {
                    DB.tbl_HistoryWeather.AddRange(lsthistory);
                    DB.SaveChanges();

                }

            }
            catch (Exception ex)
            {
                return Ok(new { status = 401, isSuccess = false, message = ex.Message, data = "" });
            }
             
            return Ok(new { status = 200, isSuccess = true, message = "data save", data = lsthistory });
            // return Ok();
        }

        [Route("MonthlyReportGeneration")]
        public async Task<IHttpActionResult> GetMonthlyWeatherData()

        {
            DateTime dtNow = DateTime.Now.Date;

            // Call Daily data and save response
           // GetWeatherData();
            JavaScriptSerializer ser = new JavaScriptSerializer();

            string apiKey = "apah899bLstduZGLWkICXLq8U2SbPkeq";
            var query = (from ass in DB.tbl_ProductAssignment
                         join proj in DB.tbl_Projects on ass.ProjectID equals proj.ProjectID
                         join prod in DB.tbl_Products on ass.ProductID equals prod.ProductID
                         where proj.IsActive == true
                         select new
                         {
                             ass,
                             proj,
                             prod

                         }).ToList();
            List<tbl_HistoryWeather> lsthistory = new List<tbl_HistoryWeather>();
            foreach (var _item in query)
            {
                DateTime latetDate = Convert.ToDateTime(_item.ass.CreatedDate).AddMonths(1).Date;
                if (latetDate == dtNow)
                {
                   
                    // Electricity Calculation
                    Response calReponse =  CalculateElectricity(_item.ass, _item.prod);
                    if (calReponse.Code == 200)
                    {
                        double targetLatitude = Convert.ToDouble(_item.ass.Latitude);
                        double targetLongitude = Convert.ToDouble(_item.ass.Longitude);
                        //Generate Report
                        ReportingController ld = new ReportingController();
                        Response report = ld.GetReport(_item.ass);
                        if(report.Code ==200 && report.Detail != "")
                        {
                            _item.ass.isActive = false;
                            _item.ass.IsReportGenerate = true;
                            _item.ass.ReportPath = report.Detail;
                            _item.proj.IsActive = false;
                                DB.SaveChanges();
                           
                        }
                    }

                }
                
            }
           
            return Ok();
        }
        public Response CalculateElectricity(tbl_ProductAssignment PASS, tbl_Products Prod)
        {
            var json = "";
            var resp = new Response();
            JavaScriptSerializer _jss = new JavaScriptSerializer();
            try
            {
                
                if (Prod != null)
                {
                    int powerPeak = Convert.ToInt32(Prod.Powerpeak);
                    string orientation = Prod.orientation;
                    double inclination = Convert.ToDouble(Prod.inclination);
                    double area = Convert.ToDouble(Prod.area);
                    double lat = Convert.ToDouble(PASS.Latitude);
                    double lon = Convert.ToDouble(PASS.Longitude);
                    var tblWeather = DB.tbl_HistoryWeather.Where(x => x.Latitude == lat && x.Longitude == lon).ToList();
                    foreach (var _item in tblWeather)
                    { 
                        double power = powerPeak * (Convert.ToDouble(_item.SolarIrradiance_Value) / 1000.0) * (area / 100.0);
                        double Angleazimuth = GetCalculateAzimuth(lon, lat);
                        double angleFactor = CalculateAngleFactor(inclination, Angleazimuth, orientation);
                        power *= angleFactor;
                        double electricityProduction1 = power * GetDaylightDuration(Convert.ToDateTime(_item.Sunrise), Convert.ToDateTime(_item.Sunset));

                        tbl_WeatherData wd = new tbl_WeatherData();
                        wd.ProjectID = PASS.ProjectID;
                        wd.ProjectName = PASS.ProjectName;
                        wd.ProductID = PASS.ProductID;
                        wd.ProductName = PASS.ProductName;
                        wd.Latitude = PASS.Latitude;
                        wd.Longitude = PASS.Longitude;
                        wd.Sunrise = Convert.ToDateTime(_item.Sunrise);
                        wd.Sunset = Convert.ToDateTime(_item.Sunset);
                        wd.SolarIrradiance_Value = Convert.ToDouble(_item.SolarIrradiance_Value);
                        wd.SolarIrradiance_Unit = _item.SolarIrradiance_Unit;
                        wd.SolarIrradiance_UnitType = _item.SolarIrradiance_UnitType;
                        wd.CalElectricity = Convert.ToString(Math.Round(electricityProduction1, 0));
                        wd.CreatedDate = _item.CreatedDate;
                        wd.UserID = PASS.UserID;
                        wd.UserName = PASS.UserName;
                        wd.ProductAssignmentID = PASS.ID;
                        DB.tbl_WeatherData.Add(wd);
                        DB.SaveChanges();
                        resp.Code = 200;
                        resp.Status = "success";
                        resp.Message = "successfully..";

                    }
                }

            }
            catch (Exception ex)
            {
                resp.Code = 404;
                resp.Status = "Bad Resquest";
                resp.Message = ex.Message;
            }
            //json = _jss.Serialize(resp);
            return resp;
        }
        public async Task<string> GetDailyData(int key, string apiKey)
        {
            var json = "";
            var resp = new Response();
            var accuWeatherData1 = new DailyWeatherdata.Root();
            //int key = 0;
            try
            {
                string apiUrl = $"http://dataservice.accuweather.com/forecasts/v1/daily/1day/{key}?apikey={apiKey}&language=en&details=true";
                var client = new HttpClient();
                var response = client.GetAsync(apiUrl).Result;
                // response.EnsureSuccessStatusCode(); // Throw an exception if error
                if (response.IsSuccessStatusCode)
                {
                    json = await response.Content.ReadAsStringAsync();
                
                }

            }
            catch (Exception ex)
            {

            }
            // return new {"key", }(key);
            return json;
        }
        static async Task<int> GetLocation(string apiKey, string location)
        {
            string baseUrl = "http://dataservice.accuweather.com";
            string endpoint = $"/locations/v1/search?q={location}&apikey={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    //  HttpResponseMessage response = await client.GetAsync(baseUrl + endpoint);
                    HttpResponseMessage response = client.GetAsync(baseUrl + endpoint).Result;  //
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        var locationData = JsonConvert.DeserializeObject<LocationData[]>(responseContent);

                        if (locationData.Length > 0)
                        {
                            // Return the location key of the first result
                            return Convert.ToInt32(locationData[0].Key);
                        }
                    }
                   
                }
                catch (Exception ex)
                {
                   // return ex.Message;
                }
            }

            return 0;
        }

        public double GetDegreesAzimuth(double longitude, double latitude)
        {
            return (180 / Math.PI) * GetCalculateAzimuth(longitude, latitude);
        }
        public double GetCalculateAzimuth(double longitude, double latitude)
        {
           
            double lonRad = ToRadians(longitude);
            double latRad = ToRadians(latitude);
            double numerator = Math.Sin(lonRad);
            double denominator = Math.Tan(latRad) * Math.Cos(lonRad) - Math.Sin(latRad) * Math.Cos(lonRad);
            double azimuthRad = Math.Atan2(numerator, denominator);
            double azimuthDeg = ToDegrees(azimuthRad);
            if (azimuthDeg < 0)
            {
                azimuthDeg += 360;
            }

            return azimuthDeg;
        }
       
        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        private static double ToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }
        private double CalculateAngleFactor(double inclination, double azimuthAngle, string orientation)
        {
            double inclinationRad = Math.PI * inclination / 180.0;
            double azimuthRad = Math.PI * azimuthAngle / 180.0;
            double angleFactor = 1.0;
            switch (orientation)
            {
                case "N":
                    angleFactor *= Math.Cos(inclinationRad);
                    break;
                case "E":
                    angleFactor *= Math.Cos(inclinationRad - azimuthRad);
                    break;
                case "S":
                    angleFactor *= Math.Cos(inclinationRad + Math.PI);
                    break;
                case "W":
                    angleFactor *= Math.Cos(inclinationRad + azimuthRad);
                    break;
                default:
                    // Handle invalid orientation value
                    Console.WriteLine("Invalid orientation. Angle factor calculation failed.");
                    return 0.0;
            }

            return angleFactor;
        }
       
        private double GetDaylightDuration(DateTime Sunset, DateTime Sunrise)
        {

            TimeSpan daylightDuration = Sunset - Sunrise;
            double dayDuration = daylightDuration.TotalHours;
            return dayDuration;
        }
    }

    class LocationData
    {
        public string Key { get; set; }
        public string LocalizedName { get; set; }
    }


}
