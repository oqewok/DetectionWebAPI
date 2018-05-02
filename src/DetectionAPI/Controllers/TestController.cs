using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;

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


        [HttpGet]
        [Route("api/test/third")]
        public IHttpActionResult Method3([FromUri] string q, [FromUri] string s, [FromUri] int d)
        {
            try
            {
                Console.WriteLine($@"q = {q}; s = {s}; d = {d};");
                Console.WriteLine();
                return Ok("God request");
            }

            catch
            {
                return BadRequest("Bad request");
            }

        }

        [HttpPost]
        [Route("api/test/posting")]
        public IHttpActionResult PostingJson([FromBody] PostingValue postedValue)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine(postedValue.ToString());
                return Ok(postedValue);
            }

            else
            {
                return BadRequest(ModelState);
            }

        }

        public class PostingValue
        {
            public int Id { get; set; }
            public string Token { get; set; }

            [MinLength(5)]
            public string UserName { get; set; }

            [MinLength(5)]
            public string UserSurname { get; set; }

            public override string ToString()
            {
                var s = $@"Id: {Id}; Token: {Token}; UserName: {UserName}; UserSurname: {UserSurname};";

                return s;
            }
        }
    }
}
