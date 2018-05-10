using DetectionAPI.Models;
using DetectionAPI.Filters;
using DetectionAPI.Detection;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;
using DetectionAPI.Detection.DetectionResult;

namespace DetectionAPI.Controllers
{
    public class DetectionController : ApiController
    {

        /// <summary>
        /// Method, that consumes request's form-data images and produces detection result
        /// </summary>
        /// <returns>Result of detection <see cref=""/> on success detection or <see cref="HttpStatusCode.Unauthorized"/>
        /// if user have not been authorized to perform this action</returns>
        [HttpPost]
        [RealBasicAuthenticationFilter]
        [Route("api/detection")]
        public async Task<IHttpActionResult> TryDetection()
        {
            int MaxContentLength = 1024 * 1024 * 3; // 3MB



            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            try
            {
                //Make Directory for images
                var directoryName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DetectionAPI", "UserImages");
                Directory.CreateDirectory(directoryName);

                var provider = new MultipartFormDataStreamProvider(directoryName);

                try
                {
                    // Read the form data.
                    //await Request.Content.ReadAsMultipartAsync(provider);

                    //// This illustrates how to get the file names.
                    //foreach (MultipartFileData file in provider.FileData)
                    //{
                    //    Console.WriteLine(file.Headers.ContentDisposition.FileName);
                    //    Console.WriteLine("Server file path: " + file.LocalFileName);
                    //}


                    var filesReadToProvider = await Request.Content.ReadAsMultipartAsync();

                    foreach (var stream in filesReadToProvider.Contents)
                    {
                        //getting of content as byte[], picture name and picture type
                        var fileBytes = await stream.ReadAsByteArrayAsync();
                        var pictureName = stream.Headers.ContentDisposition.FileName.Trim('\"');
                        var contentType = stream.Headers.ContentType.MediaType.Trim('\"');
                    }



                    return Ok();
                }
                catch (Exception e)
                {
                    return InternalServerError(e);
                }

            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return InternalServerError(exc);
            }


            //DetectionResultProvider drp = new DetectionResultProvider();
            //var dr = drp.DetectionResult();

            //if (dr == null)
            //{
            //    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "Detection failed"));
            //}

            //return Ok(dr);
        }






        #region DeteteThisExamples

        //1
        //TODO: works with IIS only due to HttpContext
        [HttpPost]
        [Route("api/detection/postimage")]
        public async Task<HttpResponseMessage> PostUserImage()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                ///Todo : null
                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {

                        int MaxContentLength = 1024 * 1024 * 3; //Size = 3 MB

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {

                            var message = string.Format("Please Upload image of type .jpg,.gif,.png.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {

                            var message = string.Format("Please Upload a file upto 3 mb.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else
                        {

                            //YourModelProperty.imageurl = userInfo.email_id + extension;
                            //  where you want to attach your imageurl

                            //if needed write the code to update the table

                            //var filePath = HttpContext.Current.Server.MapPath("~/Userimage/" + userInfo.email_id + extension);
                            //var filePath = HttpContext.Current.Server.MapPath("~/Userimage/" + Guid.NewGuid() + extension);

                            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DetectionAPI", "UserImages");
                            Directory.CreateDirectory(filePath);

                            string origNameAndExtension = postedFile.FileName.Trim('\"');
                            var origName = Path.GetFileNameWithoutExtension(origNameAndExtension);


                            string filename = origName + "_" + Guid.NewGuid().ToString() + extension;
                            filename = Path.Combine(filePath, filename);


                            //Userimage myfolder name where i want to save my image
                            postedFile.SaveAs(filename);

                        }
                    }

                    var message1 = string.Format("Image Updated Successfully.");
                    return Request.CreateErrorResponse(HttpStatusCode.Created, message1); ;
                }
                var res = string.Format("Please Upload a image.");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            catch (Exception ex)
            {
                var res = string.Format("Internal error occured");
                dict.Add("Error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
        }

        //2
        //
        [HttpPost]
        [Route("api/detection/post/img")]
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


        //3
        //
        /// <summary>  
        /// Upload Document.....  
        /// </summary>        
        /// <returns></returns>  
        [HttpPost]
        [Route("api/detection/MediaUpload")]
        public async Task<HttpResponseMessage> MediaUpload()
        {
            // Check if the request contains multipart/form-data.  
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = await Request.Content.ReadAsMultipartAsync<InMemoryMultipartFormDataStreamProvider>(new InMemoryMultipartFormDataStreamProvider());
            //access form data  
            NameValueCollection formData = provider.FormData;
            //access files  
            IList<HttpContent> files = provider.Files;

            string URL = String.Empty;
            string filename = String.Empty;

            //HttpContent imageFile = files[0];
            foreach (var imageFile in files)
            {
                var origNameAndExtension = imageFile.Headers.ContentDisposition.FileName.Trim('\"');
                var originalFileName = Path.GetFileNameWithoutExtension(origNameAndExtension);

                filename = originalFileName + "_" + Guid.NewGuid().ToString() + ".jpg";
                Stream input = await imageFile.ReadAsStreamAsync();

                var directoryName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DetectionAPI", "UserImages");
                Directory.CreateDirectory(directoryName);

                filename = Path.Combine(directoryName, filename);

                //string tempDocUrl = WebConfigurationManager.AppSettings["DocsUrl"];
                string tempDocUrl = "E:\\";

                /**
                if (formData["Image"] == "Image")
                {
                    var path = HttpRuntime.AppDomainAppPath;
                    directoryName = System.IO.Path.Combine(path, "ClientImage");
                    filename = System.IO.Path.Combine(directoryName, originalFileName);

                    //Deletion exists file  
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }

                    string DocsPath = tempDocUrl + "/" + "ClientImage" + "/";
                    URL = DocsPath + originalFileName;

                }
                **/

                //Directory.CreateDirectory(@directoryName);  
                using (Stream file = File.OpenWrite(filename))
                {
                    input.CopyTo(file);
                    //close file  
                    file.Close();
                }
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            //response.Headers.Add("X-ImageURL", filename);
            return response;

        }

        //4
        //
        /// <summary>
        /// Previous way was good bad a bit weird, another way is...
        /// At this moment, can not process anything except images in request's body and token
        /// given to all the images posted
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/detection/IncomingMessage")]
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

        #endregion

    }
}
