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

namespace Photovoltic_API.Controllers
{
    [System.Web.Http.RoutePrefix("api/FerchWeather")]
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
            Task Key =  GetLocationKey();
            DateTime startDate = new DateTime(2023 - 6 - 22);
            DateTime endDate = new DateTime(2023 - 6 - 22);
            string apiKey = "apah899bLstduZGLWkICXLq8U2SbPkeq"; // Replace with your AccuWeather API key

            //string apiUrl = $"http://dataservice.accuweather.com/forecasts/v1/daily/5day/{"50.82289508874981"},{"12.928446472126954"}?apikey={apiKey}";
            // Location GET "http://dataservice.accuweather.com/locations/v1/2602716?apikey=apah899bLstduZGLWkICXLq8U2SbPkeq&language=en&details=true"
            //curl -X GET "http://dataservice.accuweather.com/currentconditions/v1/2602716/historical/24?apikey=apah899bLstduZGLWkICXLq8U2SbPkeq&language=en&details=true"

            // location by Lato Long
            string apiUrl = $"http://dataservice.accuweather.com/locations/v1/cities/geoposition/search?apikey={apiKey}&q=50.82289508874981,12.928446472126954&language=en&details=true";
           // GET "http://dataservice.accuweather.com/locations/v1/cities/geoposition/search?apikey=apah899bLstduZGLWkICXLq8U2SbPkeq&q=50.82289508874981%2C12.928446472126954&language=en&details=true"
            var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode(); // Throw an exception if error

            var body = await response.Content.ReadAsStringAsync();
            dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(body);
            string cnic = jsonData["Key"];
            //using (HttpClient client = new HttpClient())
            //{
            //    try
            //    {
            //        HttpResponseMessage response = await client.GetAsync(apiUrl);
            //        response.EnsureSuccessStatusCode(); // Throw an exception if the request wasn't successful



            //        string responseBody = await response.Content.ReadAsStringAsync();
            //        var weatherData = JsonConvert.DeserializeObject<AccuWeatherResponse>(responseBody);

            //        // Filter the weather data for the specified date range
            //        var filteredData = new List<DailyWeatherData>();
            //        foreach (var data in weatherData.DailyForecasts)
            //        {
            //            if (data.Date >= startDate && data.Date <= endDate)
            //            {
            //                filteredData.Add(data);
            //            }
            //        }

            //        // Return the filtered weather data
            //        return Ok(filteredData);
            //    }
            //    catch (HttpRequestException ex)
            //    {
            //        return BadRequest($"Error: {ex.Message}");
            //    }
            //}
            return Ok();
        }
        public async Task<int> GetLocationKey()
                    {
            var json = "";
            var resp = new Response();
            JavaScriptSerializer _jss = new JavaScriptSerializer();
            int key = 0;
            try
            {
                string apiKey = "apah899bLstduZGLWkICXLq8U2SbPkeq"; // Replace with your AccuWeather API key

                // location by Lato Long
                string apiUrl = $"http://dataservice.accuweather.com/locations/v1/cities/geoposition/search?apikey={apiKey}&q=50.82289508874981,12.928446472126954&language=en&details=true";
                // GET "http://dataservice.accuweather.com/locations/v1/cities/geoposition/search?apikey=apah899bLstduZGLWkICXLq8U2SbPkeq&q=50.82289508874981%2C12.928446472126954&language=en&details=true"
                var client = new HttpClient();

                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode(); // Throw an exception if error

                var body = await response.Content.ReadAsStringAsync();
                dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(body);
                key = jsonData["Key"];
               
            }
            catch(Exception ex)
            {

            }
            // return new {"key", }(key);
            return key;
        }
    }
}