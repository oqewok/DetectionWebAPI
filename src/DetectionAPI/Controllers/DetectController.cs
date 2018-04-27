using DetectionAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using DetectionAPI.Detection.DetectionResult;

namespace DetectionAPI.Controllers
{
    public class DetectController : ApiController
    {
        DetectionResultProvider drp = new DetectionResultProvider();


        public IHttpActionResult GetDetect()
        {
            Console.WriteLine(Request.RequestUri);
            var dr = drp.DetectionResult();
            Console.WriteLine(dr.ToString() + " " + DateTime.Now.ToString("hh.mm.ss.ffffff"));

            if (dr == null)
            {
                //return StatusCode(HttpStatusCode.NotFound);
                return NotFound();
            }
            return Ok(dr);
        }

        [HttpPost]
        public IHttpActionResult PostDetect()
        {
            Console.WriteLine(Request.RequestUri);

            HttpContent requestContent = Request.Content;
            string jsonContent = requestContent.ReadAsStringAsync().Result;
            //Console.WriteLine(jsonContent);
            //T contact = JsonConvert.DeserializeObject<T>(jsonContent);

            var dr = drp.DetectionResult();
            Console.WriteLine(dr.ToString() + " " + DateTime.Now.ToString("hh.mm.ss.ffffff"));

            if (dr == null)
            {
                //return StatusCode(HttpStatusCode.NotFound);
                return NotFound();
            }
            return Ok(dr);
        }
    }
}
