using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace photovoltaic_API.Controllers
{
    public class Response
    {
        public int Code { set; get; }
        public string Status { set; get; }
        public string Message { set; get; }
        public string Detail { set; get; }
        public object data { set; get; }
    }
}