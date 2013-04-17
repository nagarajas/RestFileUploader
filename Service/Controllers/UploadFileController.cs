using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Threading.Tasks;
using System.IO;

namespace RestFileUploader.Service.Controllers
{
    public class UploadFileController : ApiController
    {
        // POST api/values
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uploadedFile"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Post()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["fileStorePath"].ToString());
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Reads and stores the data in the specified path.
                await Request.Content.ReadAsMultipartAsync(provider);

                // Rename existing file name with uploaded filenames.
                string fileName = string.Empty;
                foreach (MultipartFileData fileData in provider.FileData)
                {
                    // Check for Bad request
                    if (string.IsNullOrEmpty(fileData.Headers.ContentDisposition.FileName))
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "This request is not properly formatted");
                    }
                    fileName = fileData.Headers.ContentDisposition.FileName;
                    if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                    {
                        fileName = fileName.Trim('"');
                    }
                    if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                    {
                        fileName = Path.GetFileName(fileName);
                    }
                    System.IO.File.Move(fileData.LocalFileName, Path.Combine(root, fileName));
                }
                //All Done 
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}
