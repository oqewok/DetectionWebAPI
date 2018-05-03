using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;

namespace DetectionAPI.Controllers
{

    [Route("api/test")]
    //[Authorize]
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

        [HttpGet]
        [Route("api/test/content")]
        public IHttpActionResult ContentNotFound()
        {
            Console.WriteLine(Request.RequestUri);
            Console.WriteLine(DateTime.Now.ToString("HH:MM:ss.fff"));
            return Content(HttpStatusCode.NotFound, "There's no api/test/content action");
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
        [Route("api/test/third", Name = "Method3")]
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
        [Route("api/test/posting", Name = "PostingJsonRouteNameProperty")]
        [ResponseType(typeof(PostingValue))]
        public async Task<IHttpActionResult> PostingJson([FromBody] PostingValue postedValue)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine(postedValue.ToString());
                return Ok(postedValue);
            }

            else
            {
                var dict = new Dictionary<string, string>();
                dict.Add("key1", "value1");
                dict.Add("key2", "value2");
                dict.Add("key3", "value3");

                //Location - /api/test/posting?id=1&tldr=fghkdjfhgwepbpoa234j2123 - for routeValuesObject
                //content: dict values appears in body of response
                //return CreatedAtRoute(routeName: "PostingJsonRouteNameProperty", routeValues: new {id = postedValue.Id, tldr = postedValue.Token}, content: dict);
                return BadRequest(ModelState);
            }
        }

        [HttpPost]
        [Route("api/test/badrequest", Name = "BadRequestInTestController")]
        //[ResponseType(typeof(PostingValue))]
        public IHttpActionResult BadRequestOnly([FromBody] PostingValue postedValue)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine(postedValue.ToString());
                return BadRequest();
                //return CreatedAtRoute("BadRequestInTestController", new {description = "msg_value", id = 5 }, "bad request success");
            }

            else
            {
                return BadRequest(ModelState);
            }
        }



        public class PostingValue
        {
            
            [Required]
            public string Id { get; set; }

            [Required]
            public string Token { get; set; }

            [Required, MinLength(5)]
            public string UserName { get; set; }

            [Required, MinLength(5)]
            public string UserSurname { get; set; }

            public override string ToString()
            {
                var s = $@"Id: {Id}; Token: {Token}; UserName: {UserName}; UserSurname: {UserSurname};";

                return s;
            }
        }
    }
}
