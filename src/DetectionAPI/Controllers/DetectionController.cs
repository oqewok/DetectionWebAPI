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
using Newtonsoft.Json;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;
using DetectionAPI.Detection.DetectionResult;
using PlateDetector.Detection;
using System.Diagnostics;
using Ninject;
using OpenCvSharp;

namespace DetectionAPI.Controllers
{
    public class DetectionController : ApiController
    {
        #region Properties
        private Detector _MainDetector { get; set; }

        //private FakeDetector _FakeDetector { get; set; }

        private IDetector _detector;

        #endregion


        #region .ctor
        //public DetectionController()
        //{

        //}

        //public DetectionController(IDetector detector)
        //{
        //    _detector = detector;

        //}


        //public DetectionController()
        //{

        //}

        public DetectionController(Detector detector)
        {
            _MainDetector = detector;
        }

        //public DetectionController(FakeDetector detector)
        //{
        //    _FakeDetector = detector;
        //}
        #endregion


        /// <summary>
        /// Method, that consumes request's form-data images and produces detection result
        /// </summary>
        /// <returns>Result of detection <see cref=""/> on success detection or <see cref="HttpStatusCode.Unauthorized"/>
        /// if user have not been authorized to perform this action</returns>
        [HttpPost]
        [RealBasicAuthenticationFilter]
        [Route("api/detection/old")]
        public IHttpActionResult TryDetectionOld()
        {

            string n = string.Empty;

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

                                var guid = Guid.NewGuid().ToString();

                                //String fileName = headerValues[0] + "_" + origName + "_"+ Guid.NewGuid().ToString() + ".jpg";
                                String fileName = headerValues + "_" + origName + "_" + guid + ".jpg";
                                n = headerValues + "_" + origName + "_" + guid + ".json";

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

                try
                {
                    //Detector det = new Detector(new AlgManager(new FasterRcnnProvider()));

                    Bitmap img = new Bitmap("D:\\images\\car10326495.jpg");

                    //Detector det = new Detector(new AlgManager(new FasterRcnnProvider()));

                    //Mat image1 = new Mat("D:\\images\\car10326495.jpg");
                    //var det_result = det.Detect(image1);

                    //var det_result = _detector.Detect();

                    //var fc = new FakeDetector();
                    //var det_result = fc.Detect();

                    var det_result = _MainDetector.Detect(img);

                    if (det_result == null)
                    {
                        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "Detection failed"));
                    }

                    n = "markup.json";
                    string fp = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DetectionAPI", "ImagesMarkup");
                    Directory.CreateDirectory(fp);
                    string jsonPath = Path.Combine(fp, n);

                    var json = JsonConvert.SerializeObject(det_result, Formatting.Indented);
                    File.WriteAllText(jsonPath, json);


                    return Ok(det_result);

                }

                catch (Exception ex)
                {
                    Trace.WriteLine(ex.StackTrace);
                    return BadRequest();
                }


            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(
                        HttpStatusCode.NotAcceptable,
                        "This request is not properly formatted"));
            }
        }


        [HttpPost]
        [RealBasicAuthenticationFilter]
        [Route("api/detection")]
        public async Task<IHttpActionResult> TryDetection()
        {
            int MaxContentLength = 1024 * 1024 * 3; //Size = 3 MB
            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".jpeg", ".png" };
            string n = string.Empty;

            Dictionary<string, object> ErrorInfoDict = new Dictionary<string, object>();

            if (!Request.Content.IsMimeMultipartContent())
            {
                ErrorInfoDict.Add("content_error", "Request is not multipart typed");
                return BadRequest(ErrorInfoDict.ToString());
            }

            else
            {

                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DetectionAPI", "UserImages");
                Directory.CreateDirectory(filePath);

                var multipartFormDataStreamProvider = new MultipartFormDataStreamProvider(filePath);

                Request.Content.LoadIntoBufferAsync().Wait();
                await Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider);

                if (ErrorInfoDict == null)
                {

                }

                foreach (var file in multipartFormDataStreamProvider.FileData)
                {
                    try
                    {


                        if (file.Headers.ContentLength > MaxContentLength)
                        {
                            ErrorInfoDict.Add("content_error", $@"A file in a request is larger, than {MaxContentLength}");
                            return BadRequest(ErrorInfoDict.ToString());
                        }


                        Console.WriteLine(file.Headers.ContentDisposition.FileName);
                        Console.WriteLine("Server file path: " + file.LocalFileName);
                    }
                    catch (Exception exc)
                    {

                    }

                }






                return Ok();
            }

            
        }



    }
}
