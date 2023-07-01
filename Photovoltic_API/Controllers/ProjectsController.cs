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
    [System.Web.Http.RoutePrefix("api/Projects")]
    public class ProjectsController : ApiController
    {
        DB_WeatherEntities DB = new DB_WeatherEntities();
        [Route("GetProjects")]
        [HttpPost]
        public IHttpActionResult GetAllProjects(tbl_Projects _Project)
        {
            var log = DB.tbl_Projects.Where(x => x.UserID == _Project.UserID).ToList();

            if (log == null)
            {
                return NotFound();
            }
            else

                return Ok(new { status = 200, isSuccess = true, message = "successfully", data = log });
        }
       
        [Route("InsertProject")]
        [HttpPost]
        public object InsertUser(tbl_Projects _Project)
        {
            try
            {

                tbl_Projects Pt = new tbl_Projects();
                if (Pt.ProjectID == 0)
                {
                    Pt.ProjectName = _Project.ProjectName;

                    Pt.Description = _Project.Description;
                    // EL.Password = AesOperation.EncryptString(Reg.Password);
                    Pt.OwnerName = _Project.OwnerName;
                    Pt.StartDate = DateTime.Now;
                    Pt.ContactEmail = _Project.ContactEmail;
                    Pt.ContactPhone = _Project.ContactPhone;
                    Pt.UserID = _Project.UserID;
                    Pt.UserName = _Project.UserName;
                    Pt.IsActive = true;
                    DB.tbl_Projects.Add(Pt);
                    DB.SaveChanges();
                   
                }
            }
            catch (Exception)
            {

                return new Response { Code = 404, Status = "Fail", Message = "Bad Request." };
            }
            return new Response  { Code = 200, Status = "Success", Message = "Data SuccessFully Saved." };
        }
        [Route("GetbyProject")]
        [HttpGet]
        public object GetByProjects(int _ProjectID)
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

        [Route("DeleteProject")]
        [HttpDelete]
        public object DeleteProjects(int _ProjectID)
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
        public object UpdateProjects(tbl_Projects objProject)
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
                    resp.Message = "Project Update successfully..";

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