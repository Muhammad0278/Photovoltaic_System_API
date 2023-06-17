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

namespace Photovoltic_API.Controllers
{
    [System.Web.Http.RoutePrefix("api/ProductAssignment")]
    public class ProductAssignmentController : ApiController
    {
        DB_WeatherEntities DB = new DB_WeatherEntities();
        // GET: ProductAssignment
        [Route("InsertProducts")]
        [HttpPost]
        public object InsertProducts(tbl_ProductAssignment _obj)
        {
            var json = "";
            var resp = new Response();
            JavaScriptSerializer _jss = new JavaScriptSerializer();
            try
            {

                tbl_ProductAssignment Pt = new tbl_ProductAssignment();
                if (Pt.ID == 0)
                {
                    Pt.UserID = _obj.UserID;
                    Pt.UserName = _obj.UserName;
                    Pt.ProjectID = _obj.ProjectID;
                    Pt.ProjectName = _obj.ProjectName;
                    Pt.ProductID = _obj.ProductID;
                    Pt.ProductName = _obj.ProductName;
                    var tblProduct = DB.tbl_Products.Where(x => x.ProductID == _obj.ProductID).FirstOrDefault();

                    if (tblProduct != null)
                    {
                        Pt.Powerpeak = tblProduct.Powerpeak;
                        Pt.orientation = tblProduct.orientation;
                        Pt.inclination = tblProduct.inclination;
                        Pt.area = tblProduct.area;

                        //  Pt.Quantity = tblProduct.Quantity;
                        Pt.Latitude = tblProduct.Latitude;
                        Pt.Longitude = tblProduct.Longitude;
                        Pt.ImagePath = tblProduct.ImagePath;
                    }

                    DB.tbl_ProductAssignment.Add(Pt);
                    DB.SaveChanges();
                    resp.Code = 200;
                    resp.Status = "success";
                    resp.Message = "ProductInfor Select successfully..";
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
        [Route("GetSelectedProducts")]
        [HttpGet]
        public object GetPrByProjectID(int UserID)
        {
            var json = "";
            var resp = new Response();
            JavaScriptSerializer _jss = new JavaScriptSerializer();
            try
            {
                var query =
                          (from a in DB.tbl_ProductAssignment
                          join pj in DB.tbl_Projects on a.ProjectID equals pj.ProjectID
                          join pt in DB.tbl_Products on a.ProductID equals pt.ProductID
                          where a.UserID == UserID
                          select new {a.ID,a.ProjectName,a.ProjectID,a.ProductName,a.ProductID,pj.Description,pt.Wattage,pt.WarrantyYears,pt.Price,ptDes=pt.Description }).ToList();
              //  var tbl_ProductAssignment = DB.tbl_ProductAssignment.Join().Where(x => x.UserID == UserID).ToList();

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
   
    [Route("DeleteSelectedProducts")]
    [HttpDelete]
    public object DeleteSelectedProduct(int ID)
    {
        var json = "";
        var resp = new Response();
        JavaScriptSerializer _jss = new JavaScriptSerializer();
        try
        {
            var tbl_Product = DB.tbl_ProductAssignment.Where(x => x.ID == ID).FirstOrDefault();

            if (tbl_Product == null)
            {
                resp.Code = 401;
                resp.Status = "Not Found";
                resp.Message = "Record Not Found..";
            }
            else
            {
                DB.tbl_ProductAssignment.Remove(tbl_Product);
                DB.SaveChanges();

                resp.Code = 200;
                resp.Status = "success";
                resp.Message = "Items Deleted successfully..";

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