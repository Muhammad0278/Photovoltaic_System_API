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
          //  string file = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/dailydata.txt");
            //deserialize JSON from file
           // string Json = System.IO.File.ReadAllText(file);
            JavaScriptSerializer ser = new JavaScriptSerializer();
            // DailyWeatherResponse.DailyWeatherdata accuWeatherData = JsonConvert.DeserializeObject<WeatherResponse.WeatherData>(Json);
           // DailyWeatherdata q = JsonConvert.DeserializeObject<DailyWeatherdata>(Json);
        //    var accuWeatherData = ser.Deserialize<List<WeatherResponse.WeatherData>>(Json); 
           // var accuWeatherData = ser.Deserialize<DailyWeatherdata.Root>(Json);
           
            // dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(Json);
            //string  key = jsonData["DailyForecasts"];
            // return View(personlist);

            // Task Key =  GetLocationKey();



            string apiKey = "apah899bLstduZGLWkICXLq8U2SbPkeq";
            var query = (from a in DB.tbl_ProductAssignment
                         join pj in DB.tbl_Projects on a.ProjectID equals pj.ProjectID
                         join pt in DB.tbl_Products on a.ProductID equals pt.ProductID
                         where pj.IsActive == true 
                         select new  {  a.ID,  a.ProjectName, a.ProjectID,  a.ProductName,  a.ProductID,  pj.Description,
                             pt.Wattage,    pt.WarrantyYears,  pt.Price,  ptDes = pt.Description,
                             a.ImagePath,  pj.IsActive,  Latitude = a.Latitude,  Longitude = a.Longitude

                         }).ToList();
            List<tbl_HistoryWeather> lsthistory= new List<tbl_HistoryWeather>();
            foreach (var _item in query)
            {
               double targetLatitude = Convert.ToDouble(_item.Latitude);   // The target latitude value
               double targetLongitude = Convert.ToDouble(_item.Longitude); // The target longitude value
                DateTime dt = DateTime.Now.Date;
                var tblProjects = DB.tbl_HistoryWeather.Where(x =>  x.Latitude == targetLatitude && x.Longitude == targetLongitude && EntityFunctions.TruncateTime(x.CreatedDate) == dt).FirstOrDefault();

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
            //var log = DB.tbl_ProductAssignment.FirstOrDefault();

            //if (log != null)
            //{
             //    int powerPeak = Convert.ToInt32(log.Powerpeak); 
            //    string orientation = log.orientation;
            //    double inclination = Convert.ToDouble(log.inclination);
            //    double area = Convert.ToDouble(log.area); 
            //    double longitude = Convert.ToDouble(log.LongitudeNew); 
            //    double latitude = Convert.ToDouble(log.LatitudeNew);

            //    double solarIrradiance = Convert.ToDouble(data.SolarIrradiance_Value);
            //    double solarIrradianceKW = solarIrradiance / 1000;
            //    double tiltAngleFactor = Math.Cos(Math.PI * inclination / 180);
            //    double effectiveSolarIrradiance = solarIrradianceKW * tiltAngleFactor;
            //    double totalIncidentPower = effectiveSolarIrradiance * area;
            //    double electricityProduction = totalIncidentPower * powerPeak;

            //    // Print the electricity production
            //    Console.WriteLine("Electricity Production: " + electricityProduction + " kWh");

            //    //double power = powerPeak * (Convert.ToDouble(data.SolarIrradiance_Value) / 1000.0) * (area / 100.0);
            //    //double azimuthAngle = GetAzimuth(longitude, latitude); //CalculateAzimuthAngle(item.LocalObservationDateTime, longitude, latitude);
            //    //double angleFactor = CalculateAngleFactor(inclination, azimuthAngle, orientation);
            //    //power *= angleFactor;
            //    //double electricityProduction1 = power * GetDaylightDuration(Convert.ToDateTime(data.Sunrise), Convert.ToDateTime(data.Sunset));
            //    //Console.WriteLine("Electricity Production: " + electricityProduction + " kWh");

            //}



            return Ok();
        }
    
        public async Task<string> GetDailyData(int key, string apiKey)
        {
            var json = "";
            var resp = new Response();
            JavaScriptSerializer _jss = new JavaScriptSerializer();
            JavaScriptSerializer ser = new JavaScriptSerializer();
            dynamic jsonData = "";
            var accuWeatherData1 = new DailyWeatherdata.Root();
            //int key = 0;
            try
            {

                //GET "http://dataservice.accuweather.com/forecasts/v1/daily/1day/2602716?apikey=apah899bLstduZGLWkICXLq8U2SbPkeq&language=en&details=true"
                // location by Lato Long
                string apiUrl = $"http://dataservice.accuweather.com/forecasts/v1/daily/1day/{key}?apikey={apiKey}&language=en&details=true";
                var client = new HttpClient();

                //var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

                var response =  client.GetAsync(apiUrl).Result;
                // response.EnsureSuccessStatusCode(); // Throw an exception if error
                if (response.IsSuccessStatusCode)
                {
                     json = await response.Content.ReadAsStringAsync();
                 //  var  accuWeatherData = ser.Deserialize<DailyWeatherdata.Root>(body);
                    // jsonData = JsonConvert.DeserializeObject<dynamic>(body);
                }
                // key = jsonData["Key"];

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
                            return Convert.ToInt32 (locationData[0].Key);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }

            return 0;
        }
       
        public double GetDegreesAzimuth(double longitude, double latitude)
        {
            return (180 / Math.PI) * GetAzimuth(longitude, latitude);
        }
        public double GetAzimuth(double Longitude, double Latitude)
        {
            double startLat = ToRadians(Latitude);
            double endLat = ToRadians(Latitude);
            double deltaLon = ToRadians(Longitude - Longitude);

            double y = Math.Sin(deltaLon) * Math.Cos(endLat);
            double x = Math.Cos(startLat) * Math.Sin(endLat) - Math.Sin(startLat) * Math.Cos(endLat) * Math.Cos(deltaLon);

            double azimuthRad = Math.Atan2(y, x);
            double azimuthDeg = ToDegrees(azimuthRad);

            // Adjust the azimuth angle to be within the range of 0 to 360 degrees
            if (azimuthDeg < 0)
                azimuthDeg += 360;

            return azimuthDeg;
            //var longitudinalDifference = longitude - longitude;
            //var latitudinalDifference = longitude - longitude;
            //var azimuth = (Math.PI * .5d) - Math.Atan(latitudinalDifference / longitudinalDifference);
            //if (longitudinalDifference > 0) return azimuth;
            //else if (longitudinalDifference < 0) return azimuth + Math.PI;
            //else if (latitudinalDifference < 0) return Math.PI;
            //return 0d;
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
