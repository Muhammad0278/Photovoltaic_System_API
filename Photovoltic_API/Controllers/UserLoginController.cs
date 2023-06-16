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
                    return new Response
                    { Status = "Success", Message = "Record SuccessFully Saved." };
                }
            }
            catch (Exception)
            {

                throw;
            }
            return new Response
            { Status = "Error", Message = "Invalid Data." };
        }
    }

}