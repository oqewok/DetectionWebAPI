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
    public class ABCController : ApiController
    {
        public Detector det { get; set; }

        //public ABCController()
        //{

        //}

        //public ABCController(Detector detector)
        //{
        //    det = detector;
        //}

        [HttpGet]
        [Route("api/abc")]
        public IHttpActionResult TryDetection()
        {
            Console.WriteLine("Hi");

            //images
            Bitmap image1 = new Bitmap("D:\\images\\car10326495.jpg");
            //Mat image1 = new Mat("D:\\images\\car10326495.jpg");

            try
            {
                //detector
                Detector det = new Detector(new AlgManager(new FasterRcnnProvider()));
                if (det == null) return BadRequest();

                var det_result = det.Detect(image1);

                var json = JsonConvert.SerializeObject(det_result, Formatting.None);

                var drs = det_result.ToString();

                var json_drs = JsonConvert.SerializeObject(drs, Formatting.Indented);

                return Ok(json);
            }

            catch (Exception exc)
            {
                Trace.WriteLine(exc.StackTrace);
            }

            return BadRequest();

            
        }
    }
}
