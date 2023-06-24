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

        [Route("ApiCalling")]
        public async Task<IHttpActionResult> GetWeatherData()

        {
            string file = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/outputdata.txt");
            //deserialize JSON from file
            string Json = System.IO.File.ReadAllText(file);
            JavaScriptSerializer ser = new JavaScriptSerializer();

            // return View(personlist);

            // Task Key =  GetLocationKey();
            DateTime startDate = new DateTime(2023 - 6 - 22);
            DateTime endDate = new DateTime(2023 - 6 - 22);
            string apiKey = "apah899bLstduZGLWkICXLq8U2SbPkeq"; // Replace with your AccuWeather API key

            var log = DB.tbl_ProductAssignment.FirstOrDefault();

            if (log != null)
            {
                string location = log.LatitudeNew + ',' + log.LongitudeNew;
                Task<int> Key1 = GetLocationKey(log.LatitudeNew, log.LongitudeNew);
                int Key = 171238;
                // Task<dynamic> ReponseData = GetDailyData(Key, apiKey);
                // Task<string> key = GetLocation(apiKey, location);
                // int Key = 171238;
                // Task<dynamic> ReponseData = GetDailyData(Key, apiKey);
                // WeatherResponse.WeatherData accuWeatherData = JsonConvert.DeserializeObject<WeatherResponse.WeatherData>(Json);

                // Create a new AccuWeatherData object and populate its properties
                var accuWeatherData = ser.Deserialize<List<WeatherResponse.WeatherData>>(Json);
                List<tbl_WeatherData> lstdata = new List<tbl_WeatherData>();
                foreach (var item in accuWeatherData)
                {
                    tbl_WeatherData data = new tbl_WeatherData
                    {
                        UserID = log.UserID,
                        UserName = log.UserName,
                        ProjectID = log.ProjectID,
                        ProjectName = log.ProjectName,
                        ProductID = log.ProductID,
                        ProductName = log.ProductName,
                        LocalObservationDateTime = item.LocalObservationDateTime,
                        EpochTime = item.EpochTime,
                        WeatherText = item.WeatherText,
                        WeatherIcon = item.WeatherIcon,
                        HasPrecipitation = item.HasPrecipitation,
                        PrecipitationType = item.PrecipitationType,
                        IsDayTime = item.IsDayTime,
                        TemperatureMetricValue = item.Temperature.Metric.Value,
                        TemperatureMetricUnit = item.Temperature.Metric.Unit,
                        TemperatureMetricUnitType = item.Temperature.Metric.UnitType,
                        TemperatureImperialValue = item.Temperature.Imperial.Value,
                        TemperatureImperialUnit = item.Temperature.Imperial.Unit,
                        TemperatureImperialUnitType = item.Temperature.Imperial.UnitType,
                        MobileLink = item.MobileLink,
                        Link = item.Link,
                        Latitude = log.LatitudeNew,
                        Longitude = log.LongitudeNew,
                    };

                    lstdata.Add(data);
                }
                if (lstdata.Count > 0)
                {
                    DB.tbl_WeatherData.AddRange(lstdata);
                    DB.SaveChanges();
                }



                // Pre-set project/product parameters
                int powerPeak = 100; // Power peak in watts (replace with the actual value)
                string orientation = "N"; // Orientation (replace with the actual value: N, E, S, or W)
                double inclination = 30.0; // Inclination/tilt in degrees (replace with the actual value)
                double area = 50.0; // Area in square meters (replace with the actual value)
                double longitude = 123.456; // Longitude (replace with the actual value)
                double latitude = 78.901; // Latitude (replace with the actual value)

                // Retrieve the solar radiation value from the AccuWeather response
                //double solarRadiationValue = item.SolarRadiation; // Assuming the AccuWeather response includes the solar radiation value

                //// Calculate the power based on the solar radiation and other parameters
                //double power = powerPeak * (solarRadiationValue / 1000.0) * (area / 100.0);

                //// Adjust the power based on the inclination angle and orientation
                //double azimuthAngle = GetDegreesAzimuth(longitude, latitude); //CalculateAzimuthAngle(item.LocalObservationDateTime, longitude, latitude);
                //double angleFactor = CalculateAngleFactor(inclination, azimuthAngle, orientation);
                //power *= angleFactor;

                //double electricityProduction = power * GetDaylightDuration();

                //// Print the electricity production
                //Console.WriteLine("Electricity Production: " + electricityProduction + " kWh");
                // Calculate the electricity production based on power and time
              //  DateTime localObservationDateTime = item.LocalObservationDateTime;
               // double electricityProduction = power * GetDaylightDuration(localObservationDateTime);

                // Print the electricity production
               // Console.WriteLine("Electricity Production: " + electricityProduction + " kWh");

            }



            return Ok();
        }
        public async Task<int> GetLocationKey(string Latitude, string Longitude)
        {
            var json = "";
            var resp = new Response();
            JavaScriptSerializer _jss = new JavaScriptSerializer();
            int key = 0;
            try
            {
                string apiKey = "apah899bLstduZGLWkICXLq8U2SbPkeq"; // Replace with your AccuWeather API key

                // location by Lato Long
                string location = Latitude + ',' + Longitude;
                string apiUrl = $"https://dataservice.accuweather.com/locations/v1/cities/geoposition/search?apikey={apiKey}&q={location}&language=en&details=true";
                // GET "http://dataservice.accuweather.com/locations/v1/cities/geoposition/search?apikey=apah899bLstduZGLWkICXLq8U2SbPkeq&q=50.82289508874981%2C12.928446472126954&language=en&details=true"

                //HttpClient client = new HttpClient();

                //// Add an Accept header for JSON format.
                //client.DefaultRequestHeaders.Accept.Add(
                //new MediaTypeWithQualityHeaderValue("application/json"));

                //// List data response.
                //HttpResponseMessage response = client.GetAsync(apiUrl).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
                //if (response.IsSuccessStatusCode)
                //{
                //    // Parse the response body.
                //    var dataObjects = response.Content.ReadAsAsync<IEnumerable<DataObject>>().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                //    foreach (var d in dataObjects)
                //    {
                //        // Console.WriteLine("{0}", d.Name);
                //    }
                //}
                //else
                //{
                //    Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                //}


                var client = new HttpClient();
                HttpResponseMessage response = client.GetAsync(apiUrl).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
                response.EnsureSuccessStatusCode(); // Throw an exception if error

                var body = await response.Content.ReadAsStringAsync();
                dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(body);
                key = jsonData["Key"];

            }
            catch (Exception ex)
            {
                throw ex;
            }
            // return new {"key", }(key);
            return key;
        }
        public async Task<dynamic> GetDailyData(int key, string apiKey)
        {
            var json = "";
            var resp = new Response();
            JavaScriptSerializer _jss = new JavaScriptSerializer();
            dynamic jsonData = "";
            //int key = 0;
            try
            {

                //"http://dataservice.accuweather.com/currentconditions/v1/2602716/historical/24?apikey=apah899bLstduZGLWkICXLq8U2SbPkeq&language=en&details=false"
                // location by Lato Long
                string apiUrl = $"http://dataservice.accuweather.com/currentconditions/v1/{key}/historical/24?apikey={apiKey}&language=en&details=false";
                var client = new HttpClient();

                //var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

                var response =  client.GetAsync(apiUrl).Result;
                response.EnsureSuccessStatusCode(); // Throw an exception if error

                var body = await response.Content.ReadAsStringAsync();

                jsonData = JsonConvert.DeserializeObject<dynamic>(body);
                // key = jsonData["Key"];

            }
            catch (Exception ex)
            {

            }
            // return new {"key", }(key);
            return jsonData;
        }
        static async Task<string> GetLocation(string apiKey, string location)
        {
            string baseUrl = "http://dataservice.accuweather.com";
            string endpoint = $"/locations/v1/search?q={location}&apikey={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(baseUrl + endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        var locationData = JsonConvert.DeserializeObject<LocationData[]>(responseContent);

                        if (locationData.Length > 0)
                        {
                            // Return the location key of the first result
                            return locationData[0].Key;
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

            return null;
        }
        private double CalculateAzimuthAngle(DateTime date, double longitude, double latitude)
        {
            // Perform the necessary calculations to determine the azimuth angle based on the date, longitude, and latitude
            // You can use astronomical formulas or libraries to obtain the azimuth angle
            // For simplicity, you can use approximate formulas based on the date, longitude, and latitude

            double azimuthAngle = 0.0; // Placeholder, replace with the actual calculation
            return azimuthAngle;
        }
        public double GetDegreesAzimuth(double longitude, double latitude)
        {
            return (180 / Math.PI) * GetAzimuth(longitude, latitude);
        }
        public double GetAzimuth(double longitude, double latitude)
        {
            var longitudinalDifference = longitude - longitude;
            var latitudinalDifference = longitude - longitude;
            var azimuth = (Math.PI * .5d) - Math.Atan(latitudinalDifference / longitudinalDifference);
            if (longitudinalDifference > 0) return azimuth;
            else if (longitudinalDifference < 0) return azimuth + Math.PI;
            else if (latitudinalDifference < 0) return Math.PI;
            return 0d;
        }
        private double CalculateAngleFactor(double inclination, double azimuthAngle, string orientation)
        {
            // Convert inclination angle to radians
            double inclinationRad = Math.PI * inclination / 180.0;

            // Convert azimuth angle to radians
            double azimuthRad = Math.PI * azimuthAngle / 180.0;

            // Initialize the angle factor
            double angleFactor = 1.0;

            // Adjust the angle factor based on the orientation
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

        // Function to calculate the daylight duration based on the local observation date and time
        private double GetDaylightDuration(DateTime localObservationDateTime)
        {
            // Perform the necessary calculations to determine the daylight duration based on the local observation date and time
            // You can use sunrise and sunset time calculations or refer to astronomical data sources to obtain the daylight duration

            double daylightDuration = 12.0; // Placeholder, replace with the actual calculation
            return daylightDuration;
        }
    }

    class LocationData
    {
        public string Key { get; set; }
        public string LocalizedName { get; set; }
        // Add any other relevant properties from the AccuWeather response
    }

}
