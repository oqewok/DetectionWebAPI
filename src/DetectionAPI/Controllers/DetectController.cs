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
            var dr = drp.DetectionResult();
            Console.WriteLine(dr.ToString());

            if (dr == null)
            {
                //return StatusCode(HttpStatusCode.NotFound);
                return NotFound();
            }
            return Ok(dr);
        }
    }
}
