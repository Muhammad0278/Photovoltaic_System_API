using Photovoltic_API.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Photovoltic_API.Controllers
{
    public class WeatherResponse
    {
        [Table(Name = "WeatherData")]
        public class WeatherData
        {
            [Column(IsPrimaryKey = true)]
            public int ID { get; set; }

            //[Column]
            //public int ProductAssignmentID { get; set; }

            //[Column]
            //public int ProjectID { get; set; }

            //[Column]
            //public string ProjectName { get; set; }

            //[Column]
            //public int ProductID { get; set; }

            //[Column]
            //public string ProductName { get; set; }

            //[Column]
            //public int UserID { get; set; }

            //[Column]
            //public string UserName { get; set; }

            [Column]
            public DateTime LocalObservationDateTime { get; set; }

            [Column]
            public int EpochTime { get; set; }

            [Column]
            public string WeatherText { get; set; }

            [Column]
            public int WeatherIcon { get; set; }

            [Column]
            public bool HasPrecipitation { get; set; }

            [Column]
            public string PrecipitationType { get; set; }

            [Column]
            public bool IsDayTime { get; set; }

            [Column]
            public double TemperatureMetricValue { get; set; }

            [Column]
            public string TemperatureMetricUnit { get; set; }

            [Column]
            public int TemperatureMetricUnitType { get; set; }

            [Column]
            public int TemperatureImperialValue { get; set; }

            [Column]
            public string TemperatureImperialUnit { get; set; }

            [Column]
            public int TemperatureImperialUnitType { get; set; }

            [Column]
            public string MobileLink { get; set; }

            [Column]
            public string Link { get; set; }
           // [Column]
            //public string Latitude { get; set; }
            //[Column]
            //public string Longitude { get; set; }

            //[Column]
            //public DateTime CreatedDate { get; set; }
        }

        //public class DB_WeatherEntities : DataContext
        //{
        //    public Table<tbl_WeatherData> WeatherData { get { return GetTable<tbl_WeatherData>(); } }

        //    public DB_WeatherEntities(string connectionString) : base(connectionString) { }
        //}

    }
}