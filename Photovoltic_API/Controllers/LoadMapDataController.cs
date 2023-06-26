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
    }
}