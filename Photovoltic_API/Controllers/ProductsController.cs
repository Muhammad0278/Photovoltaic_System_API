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

namespace Photovoltic_API.Controllers
{
    [System.Web.Http.RoutePrefix("api/Products")]
    public class ProductsController : ApiController
    {
        DB_WeatherEntities DB = new DB_WeatherEntities();
        [Route("GetProducts")]
        [HttpPost]
        public IHttpActionResult GetAllProducts(tbl_Projects _Project)
        {
            var log = DB.tbl_Products.Where(x => x.UserID == _Project.UserID).ToList();

            if (log == null)
            {
                return Ok(new { status = 401, isSuccess = false, message = "Invalid data", });
            }
            else

                return Ok(new { status = 200, isSuccess = true, message = "User Login successfully", data = log });
        }
        [Route("GetProject")]
        [HttpGet]
        public object GetAllProject(int UserID)
        {

            var json = "";
            var resp = new Response();
            JavaScriptSerializer _jss = new JavaScriptSerializer();
            try
            {
                var log = DB.tbl_Projects.Where(x => x.UserID == UserID).Select(x => new { x.ProjectID, x.ProjectName }).ToList();

                if (log != null)
                {
                    //  return Ok(new { status = 401, isSuccess = false, message = "Invalid data", });
                    resp.Code = 200;
                    resp.Status = "success";
                    resp.Message = "Project Deleted successfully..";
                    resp.data = log;
                }
                else
                {
                    resp.Code = 401;
                    resp.Status = "Not Found";
                    resp.Message = "Record Not Found..";
                }
            }
            catch(Exception ex)
            {

            }
            json = _jss.Serialize(resp);
            //return Ok(new { status = 200, isSuccess = true, message = "User Login successfully", data = json });
             return json;
        }
        [Route("InsertProducts")]
        [HttpPost]
        public object InsertProducts(tbl_Products _obj)
        {
            var json = "";
            var resp = new Response();
            JavaScriptSerializer _jss = new JavaScriptSerializer();
            try
            {

                tbl_Products Pt = new tbl_Products();
                if (Pt.ProjectID == 0)
                {
                    Pt.ProjectID = _obj.ProjectID;
                    Pt.ProjectName = _obj.ProjectName;
                    //Pt.ProductID = _obj.ProductID;
                    Pt.ProductName = _obj.ProductName;
                    Pt.Manufacturer = _obj.Manufacturer;
                    Pt.Date = DateTime.Now;
                    Pt.Wattage = _obj.Wattage;
                    Pt.Efficiency = _obj.Efficiency;
                    Pt.WarrantyYears = _obj.WarrantyYears;
                    Pt.Price = _obj.Price;
                    Pt.Latitude = _obj.Latitude;
                    Pt.Longitude = _obj.Longitude;
                    Pt.UserID = _obj.UserID;
                    Pt.UserName = _obj.UserName;
                    Pt.Description = _obj.Description;
                    Pt.IsActive = true;
                    DB.tbl_Products.Add(Pt);
                    DB.SaveChanges();
                    resp.Code = 200;
                    resp.Status = "success";
                    resp.Message = "Project Deleted successfully..";
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


        [Route("GetbyProducts")]
        [HttpGet]
        public object GetByProducts(int _ProjectID)
        {
            var json = "";
            var resp = new Response();
            JavaScriptSerializer _jss = new JavaScriptSerializer();
            try
            {
                var tblProjects = DB.tbl_Projects.Where(x => x.ProjectID == _ProjectID).FirstOrDefault();

                if (tblProjects == null)
                {
                    resp.Code = 401;
                    resp.Status = "Not Found";
                    resp.Message = "Record Not Found..";
                }
                else
                {


                    resp.Code = 200;
                    resp.Status = "success";
                    resp.Message = "Project Deleted successfully..";
                    resp.data = tblProjects;
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

        [Route("DeleteProducts")]
        [HttpDelete]
        public object DeleteProducts(int _ProjectID)
        {
            var json = "";
            var resp = new Response();
            JavaScriptSerializer _jss = new JavaScriptSerializer();
            try
            {
                var tblProjects = DB.tbl_Projects.Where(x => x.ProjectID == _ProjectID).FirstOrDefault();

                if (tblProjects == null)
                {
                    resp.Code = 401;
                    resp.Status = "Not Found";
                    resp.Message = "Record Not Found..";
                }
                else
                {
                    DB.tbl_Projects.Remove(tblProjects);
                    DB.SaveChanges();

                    resp.Code = 200;
                    resp.Status = "success";
                    resp.Message = "Project Deleted successfully..";

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
        [Route("UpdateProject")]
        [HttpPut]
        public object UpdateProducts(tbl_Projects objProject)
        {
            var json = "";
            var resp = new Response();
            JavaScriptSerializer _jss = new JavaScriptSerializer();
            try
            {
                var tblProduct = DB.tbl_Projects.Where(x => x.ProjectID == objProject.ProjectID).FirstOrDefault();
                if (tblProduct != null)
                {
                    tblProduct.ProjectName = objProject.ProjectName;
                    tblProduct.Description = objProject.Description;
                    tblProduct.OwnerName = objProject.OwnerName;
                    tblProduct.StartDate = DateTime.Now;
                    tblProduct.ContactEmail = objProject.ContactEmail;
                    tblProduct.ContactPhone = objProject.ContactPhone;
                    DB.SaveChanges();


                }
                if (tblProduct == null)
                {
                    resp.Code = 401;
                    resp.Status = "Not Found";
                    resp.Message = "Record Not Found..";
                }
                else
                {
                    resp.Code = 200;
                    resp.Status = "success";
                    resp.Message = "Project Deleted successfully..";

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

        [Route("GetProducts")]
        [HttpGet]
        public object GetPrByProjectID(int _ProjectID)
        {
            var json = "";
            var resp = new Response();
            JavaScriptSerializer _jss = new JavaScriptSerializer();
            try
            {
                var tblProjects = DB.tbl_Products.Where(x => x.ProjectID == _ProjectID).ToList();

                if (tblProjects.Count> 0)
                {
                    resp.Code = 200;
                    resp.Status = "success";
                    resp.Message = "Project Deleted successfully..";
                    resp.data = tblProjects;
                   
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