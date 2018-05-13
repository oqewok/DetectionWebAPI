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
        public Detector det;

        public FakeDetector f_detector;

        public ABCController()
        {

        }

        //public ABCController(FakeDetector fd)
        //{
        //    f_detector = fd;
        //}

        public ABCController(Detector detector, FakeDetector fd)
        {
            det = detector;
            f_detector = fd;

        }

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
                //Detector det = new Detector(new AlgManager(new FasterRcnnProvider()));
                if (det == null) return BadRequest();

                var det_result = det.Detect(image1);

                return Ok(det_result);
            }

            catch (Exception exc)
            {
                Trace.WriteLine(exc.StackTrace);
            }

            return BadRequest();

            
        }



        [HttpGet]
        [Route("api/abc/fake")]
        public IHttpActionResult TryFakeDetection()
        {
            Console.WriteLine("Hi, fake");

            if (f_detector == null) { return BadRequest(); }

            var res = f_detector.Detect();



            return Ok(res);
        }

    }
}
