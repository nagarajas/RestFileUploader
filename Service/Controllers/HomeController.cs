using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestSharp;
using System.Configuration;
using System.IO;

namespace RestFileUploader.Service.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            if (postedFile.ContentLength > 0)
            {
                string serviceUrl = ConfigurationManager.AppSettings["fileUploadService"].ToString();
                RestClient restClient = new RestClient(serviceUrl);
                RestRequest restRequest = new RestRequest(Method.POST);                
                byte[] filedata = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    postedFile.InputStream.CopyTo(ms);
                    filedata = ms.ToArray();
                }
                restRequest.AddHeader("ContentType", "application/x-www-form-urlencoded"); 
                restRequest.AddFile("uploadedFile", filedata, postedFile.FileName);
                var response = restClient.Execute(restRequest);
            }
            return View();
        }
    }
}
