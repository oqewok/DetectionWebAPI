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
            Console.WriteLine(dr.ToString() + " " + DateTime.Now.ToString("HH:MM:ss.fff"));

            if (dr == null)
            {
                //return StatusCode(HttpStatusCode.NotFound);
                return NotFound();
            }
            return Ok(dr);
        }

        [HttpPost]
        public IHttpActionResult JustAnotherMethod([FromBody]string someValue, [FromUri] int rrr)
        {
            Console.WriteLine($@"1) RequestUri: {Request.RequestUri.ToString()};");
            Console.WriteLine();
            Console.WriteLine();

            var headers = RequestContext.Url.Request.Headers;

            if (headers != null)
            {
                Console.WriteLine($@"2) RequestHeaders{'\n'}: {RequestContext.Url.Request.Headers.ToString()};");
                Console.WriteLine();
                Console.WriteLine();
            }

            Console.WriteLine($@"3) RequestContext.Url.Request: {RequestContext.Url.Request.ToString()};");
            Console.WriteLine();
            Console.WriteLine();

            var cs = RequestContext.ClientCertificate;
            if (cs != null)
            {
                Console.WriteLine($@"4) Client cert: {cs.ToString()};");
                Console.WriteLine();
                Console.WriteLine();
            }
            Console.WriteLine($@"5) JustAnotherMethod with {someValue} {rrr} {DateTime.Now.ToString("HH:MM:ss.fff")};");
            Console.WriteLine();
            Console.WriteLine();

            var resp = Ok("JustAnotherMethod Ok");
            return resp;

        }


    }
}
