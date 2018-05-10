using DetectionAPI.Models;
using DetectionAPI.Filters;
using DetectionAPI.Detection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace DetectionAPI.Misc.tmp
{
    public class Class2 : ApiController
    {
        //2
        //
        [HttpPost]
        //[Route("api/detection/post/img")]
        public async Task<HttpResponseMessage> PostFormData()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            try
            {
                //string root = HttpContext.Current.Server.MapPath("~/App_Data");

                string r = "~/App_Data";
                var provider = new MultipartFormDataStreamProvider(r);

                try
                {
                    // Read the form data.
                    await Request.Content.ReadAsMultipartAsync(provider);

                    // This illustrates how to get the file names.
                    foreach (MultipartFileData file in provider.FileData)
                    {
                        Console.WriteLine(file.Headers.ContentDisposition.FileName);
                        Console.WriteLine("Server file path: " + file.LocalFileName);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (System.Exception e)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
                }

            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc);
            }

        }
    }
}
