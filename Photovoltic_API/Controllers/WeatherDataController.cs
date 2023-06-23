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
    [System.Web.Http.RoutePrefix("api/WeatherAPI")]
    public class DailyWeatherData
    {
        public DateTime Date { get; set; }
        public double Temperature { get; set; }
        public double Precipitation { get; set; }
        // Add more properties as needed
    }
    public class AccuWeatherResponse
    {
        public List<DailyWeatherData> DailyForecasts { get; set; }
    }
    public class WeatherDataController : ApiController
    {
        DB_WeatherEntities DB = new DB_WeatherEntities();
  

        [Route("CallAPI")]
        [HttpGet]
       // public async Task<IHttpActionResult> GetWeatherData(double latitude, double longitude, DateTime startDate, DateTime endDate)
        public async Task<IHttpActionResult> GetWeatherData(double latitude)
           
        {
            DateTime startDate = new DateTime(2023-6-22);
            DateTime endDate = new DateTime(2023 - 6 - 22);
            string apiKey = "apah899bLstduZGLWkICXLq8U2SbPkeq"; // Replace with your AccuWeather API key

            string apiUrl = $"http://dataservice.accuweather.com/forecasts/v1/daily/5day/{"50.82289508874981"},{"12.928446472126954"}?apikey={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode(); // Throw an exception if the request wasn't successful

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var weatherData = JsonConvert.DeserializeObject<AccuWeatherResponse>(responseBody);

                    // Filter the weather data for the specified date range
                    var filteredData = new List<DailyWeatherData>();
                    foreach (var data in weatherData.DailyForecasts)
                    {
                        if (data.Date >= startDate && data.Date <= endDate)
                        {
                            filteredData.Add(data);
                        }
                    }

                    // Return the filtered weather data
                    return Ok(filteredData);
                }
                catch (HttpRequestException ex)
                {
                    return BadRequest($"Error: {ex.Message}");
                }
            }
        }

    }
}