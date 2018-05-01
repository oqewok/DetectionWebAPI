using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DetectionAPI.Controllers
{

    [Route("api/test")]
    public class TestController : ApiController
    {
        [HttpGet]
        public IHttpActionResult JustMethod()
        {
            Console.WriteLine(Request.RequestUri);
            var dr = "Test controller works";
            Console.WriteLine(dr.ToString() + " " + DateTime.Now.ToString("HH:MM:SS.fff"));

            if (dr == null)
            {
                //return StatusCode(HttpStatusCode.NotFound);
                return NotFound();
            }
            return Ok(dr);
        }


    }
}
