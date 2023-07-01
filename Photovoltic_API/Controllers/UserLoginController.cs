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
using System.Web.Script.Serialization;
using System.Web.Http.Description;

namespace Photovoltic_API.Controllers
{

    // GET: UserLogin

    [System.Web.Http.RoutePrefix("api/User")]

    public class UserLoginController : ApiController
    {
       DB_WeatherEntities DB = new DB_WeatherEntities();
        [Route("Login")]
       
        [HttpPost]
        public IHttpActionResult userLogin(Login login)
        {
            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            //  string password = AesOperation.DecryptString(key,Convert.ToString(login.Password));
            string password = Convert.ToString(login.Password);
            var log = DB.Users.Where(x => x.Email.Equals(login.Email) && x.Password.Equals(login.Password)).FirstOrDefault();

            if (log == null)
            {
                return Ok(new { status = 401, isSuccess = false, message = "Invalid User", });
            }
            else

                return Ok(new { status = 200, isSuccess = true, message = "User Login successfully", user = log });
        }
        [Route("InsertUser")]
        [HttpPost]
        public object InsertUser(Register Reg)
        {
            try
            {
                DB_WeatherEntities DB = new DB_WeatherEntities();
                User EL = new User();
                if (EL.Id == 0)
                {
                    EL.UserName = Reg.UserName;

                    EL.Email = Reg.Email;
                    // EL.Password = AesOperation.EncryptString(Reg.Password);
                     EL.Password =Reg.Password;

                    DB.Users.Add(EL);
                    DB.SaveChanges();
                 
                }
            }
            catch (Exception)
            {

                return new Response { Code = 401, Status = "Error", Message = "Invalid Data." };
            }
            return new Response { Code = 200, Status = "Success", Message = "Record SuccessFully Saved." };
        }
        [Route("DeleteUser")]
        [HttpDelete]
        public object DeleteUser(int ID)
        {
            var json = "";
            var resp = new Response();
            JavaScriptSerializer _jss = new JavaScriptSerializer();
            try
            {
                var tbl_user = DB.Users.Where(x => x.Id == ID).FirstOrDefault();

                if (tbl_user == null)
                {
                    resp.Code = 401;
                    resp.Status = "Not Found";
                    resp.Message = "Record Not Found..";
                }
                else
                {
                    DB.Users.Remove(tbl_user);
                    DB.SaveChanges();

                    resp.Code = 200;
                    resp.Status = "success";
                    resp.Message = "User Deleted successfully..";

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

        [Route("UpdateUser")]
        [HttpPut]
        public object UpdateUser(int id, User userDto)
        {
            var json = "";
            var resp = new Response();
            JavaScriptSerializer _jss = new JavaScriptSerializer();

            var user = DB.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            user.UserName = userDto.UserName;
            user.Email = userDto.Email;
            user.Address = userDto.Address;
            user.City = userDto.City;
            user.State = userDto.State;
            user.ZipCode = userDto.ZipCode;

            // Save changes to the database
            DB.SaveChanges();

            resp.Code = 200;
            resp.Status = "success";
            resp.Message = "User Update successfully..";

            json = _jss.Serialize(resp);
            return json;
        }
    }

}