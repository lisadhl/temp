using System;
using System.Web.Http;


using System.Web;
using System.IO;
using System.Collections.Specialized;
using System.Web.Mvc;

namespace WebApi.Controllers
{
    public class FileToolController : ApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/filetool/UploadFile")]
        public void UploadFile()
        {
            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;

            if (request.Files.Count == 0)
            {
                throw new InvalidOperationException("No file uploaded.");
            }

            string companyCode = request.QueryString["CompanyCode"];
            string storeCode = request.QueryString["storeCode"];
            string LOG_DIRECTORY = $"logs/{companyCode}/{storeCode}";
            string logsPath = context.Server.MapPath($"~/{LOG_DIRECTORY}");

            if (!Directory.Exists(logsPath))
            {
                Directory.CreateDirectory(logsPath);
            }

            HttpPostedFile file = request.Files[0];
            file.SaveAs(Path.Combine(logsPath, file.FileName));
            HttpResponse response = context.Response;
            response.StatusCode = 200;
            response.ContentType = "text/plain";
            
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/filetool/UploadFile2")]
        public void UploadFile2()
        {
            // [FromUri] NameValueCollection queryParameters
            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;

            if (request.Files.Count == 0)
            {
                throw new InvalidOperationException("No file uploaded.");
            }

            string companyCode = request.Form["CompanyCode"];
            string storeCode = request.Form["storeCode"];
            string LOG_DIRECTORY = $"logs/{companyCode}/{storeCode}";
            string logsPath = context.Server.MapPath($"~/{LOG_DIRECTORY}");

            if (!Directory.Exists(logsPath))
            {
                Directory.CreateDirectory(logsPath);
            }

            HttpPostedFile file = request.Files[0];
            file.SaveAs(Path.Combine(logsPath, file.FileName));
            HttpResponse response = context.Response;
            response.StatusCode = 200;
            response.ContentType = "text/plain";

        }

    }
}