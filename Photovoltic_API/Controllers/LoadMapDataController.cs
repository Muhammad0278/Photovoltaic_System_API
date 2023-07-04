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

namespace Photovoltic_API.Controllers
{
    class lstWeather
    {
        public string date { get; set; }
        public double value { get; set; }
    }
    class CustomList
    {
        public int ID { get; set; }
        public string ProjectName { get; set; }
        public int ProjectID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public double Wattage { get; set; }
        public string WarrantyYears { get; set; }
        public string Price { get; set; }
        public string ptDes { get; set; }
        public string LocalizedName { get; set; }
        public string ImagePath { get; set; }
        public bool IsActive { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<lstWeather> lstdata { get; set; }
    }
    [System.Web.Http.RoutePrefix("api/MapData")]
    public class LoadMapDataController : ApiController
    {
        DB_WeatherEntities DB = new DB_WeatherEntities();
        [Route("GetMapProductsByProID")]
        [HttpGet]
        public object GetPrByProjectID(int UserID,int ProjectID,bool Status)
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
                          pj.IsActive== Status
                             select new { a.ID, a.ProjectName, a.ProjectID,
                                 a.ProductName, a.ProductID, pj.Description, pt.Wattage, pt.WarrantyYears,
                                 pt.Price, ptDes = pt.Description, a.ImagePath,pj.IsActive,
                                 Latitude=a.Latitude,
                                 Longitude=a.Longitude 
                                
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

        [Route("GetAllProductByPID")]
        [HttpGet]
        public object GetAllProdByProjectID(int UserID, int ProjectID, bool Status)
        {
            var json = "";
            var resp = new Response();
            JavaScriptSerializer _jss = new JavaScriptSerializer();
            try
            {
                var query = (from a in DB.tbl_ProductAssignment
                             join pj in DB.tbl_Projects on a.ProjectID equals pj.ProjectID
                             join pt in DB.tbl_Products on a.ProductID equals pt.ProductID

                             where a.UserID == UserID && a.ProjectID == ProjectID 
                          
                             select new
                             {
                                 a.ID,    a.ProjectName,  a.ProjectID,
                                 a.ProductName, a.ProductID,
                                 pj.Description,   pt.Wattage,    pt.WarrantyYears,
                                 pt.Price, ptDes = pt.Description,   a.ImagePath,
                                 pj.IsActive,   Latitude = a.Latitude,   Longitude = a.Longitude

                             }).ToList();
                List<CustomList> customList = new List<CustomList>();
                foreach (var q in query)
                {
                    var dl= new CustomList();
                    dl.ID = q.ID;
                    dl.ProjectID = Convert.ToInt32(q.ProjectID);
                    dl.ProjectName = q.ProjectName;
                    dl.ProductName = q.ProductName;
                    dl.ProductID =  Convert.ToInt32(q.ProductID);
                    dl.Description = q.Description;
                    dl.ptDes = q.ptDes;
                    dl.Wattage = Convert.ToDouble(q.Wattage);
                    dl.WarrantyYears = Convert.ToString(q.WarrantyYears);
                    dl.Price = Convert.ToString(q.Price);
                    dl.ImagePath = q.ImagePath;
                    dl.Latitude = Convert.ToDouble(q.Latitude);
                    dl.Longitude = Convert.ToDouble(q.Longitude);
                  
                    var tblHistoryW = DB.tbl_WeatherData.Where(x => x.ProductAssignmentID == dl.ID).ToList();
                   
                    List<lstWeather> ListW = new List<lstWeather>();
                    foreach (var tbl in tblHistoryW)
                    {
                        var LB =  new lstWeather();
                        LB.date = Convert.ToDateTime(tbl.CreatedDate).ToString("yyyy-MM-dd");
                        LB.value =Convert.ToDouble(tbl.CalElectricity);
                        ListW.Add(LB);
                    }
                    dl.lstdata = ListW;

                    customList.Add(dl);

                }

                if (customList.Count > 0)
                {
                    resp.Code = 200;
                    resp.Status = "success";
                    resp.Message = "Details successfully..";
                    resp.data = customList;

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
    }
}