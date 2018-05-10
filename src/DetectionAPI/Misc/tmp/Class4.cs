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
using System.Drawing;

namespace DetectionAPI.Misc.tmp
{
    public class Class4 : ApiController
    {
        //4
        //
        /// <summary>
        /// Previous way was good bad a bit weird, another way is...
        /// At this moment, can not process anything except images in request's body and token
        /// given to all the images posted
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[Route("api/detection/IncomingMessage")]
        public HttpResponseMessage IncomingMessage()
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            if (Request.Content.IsMimeMultipartContent())
            {
                //For larger files, this might need to be added:
                //Request.Content.LoadIntoBufferAsync().Wait();
                try
                {
                    Request.Content.LoadIntoBufferAsync().Wait();
                    Request.Content.ReadAsMultipartAsync<MultipartMemoryStreamProvider>(
                        new MultipartMemoryStreamProvider()).ContinueWith((task) =>
                        {
                            MultipartMemoryStreamProvider provider = task.Result;
                            foreach (HttpContent content in provider.Contents)
                            {
                                Stream stream = content.ReadAsStreamAsync().Result;
                                Image image = Image.FromStream(stream);
                                var testName = content.Headers.ContentDisposition.Name;
                                //String filePath = HostingEnvironment.MapPath("~/Images/");
                                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DetectionAPI", "UserImages");
                                Directory.CreateDirectory(filePath);

                                //Note that the ID is pushed to the request header,
                                //not the content header:
                                //String[] headerValues = (String[])Request.Headers.GetValues("image_token");
                                string headerValues = "img";

                                var origNameAndExtension = content.Headers.ContentDisposition.FileName.Trim('\"');
                                var origName = Path.GetFileNameWithoutExtension(origNameAndExtension);

                                //String fileName = headerValues[0] + "_" + origName + "_"+ Guid.NewGuid().ToString() + ".jpg";
                                String fileName = headerValues + "_" + origName + "_" + Guid.NewGuid().ToString() + ".jpg";

                                //string tmpName = Guid.NewGuid().ToString();
                                //String fileName = tmpName + ".jpg";
                                String fullPath = Path.Combine(filePath, fileName);
                                image.Save(fullPath);
                            }
                        });
                }

                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }

                return result;
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(
                        HttpStatusCode.NotAcceptable,
                        "This request is not properly formatted"));
            }
        }


        //5
        //
        public async Task<IHttpActionResult> UploadFile()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return StatusCode(HttpStatusCode.UnsupportedMediaType);
            }

            var filesReadToProvider = await Request.Content.ReadAsMultipartAsync();

            foreach (var stream in filesReadToProvider.Contents)
            {
                //getting of content as byte[], picture name and picture type
                var fileBytes = await stream.ReadAsByteArrayAsync();
                var pictureName = stream.Headers.ContentDisposition.FileName;
                var contentType = stream.Headers.ContentType.MediaType;
            }

            return Ok();
        }
    }
}
