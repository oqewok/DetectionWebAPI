using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;


namespace DetectionAPI.Controllers
{
    public class ImageUploadController : ApiController
    {
        /// <summary>
        /// Method to POST image to API server
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/upload/image")]
        public async Task<IHttpActionResult> UploadImage()
        {
            return Ok();
        }

        /// <summary>
        /// Method to upload a arameter to API in request's body
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/upload/parameters")]
        public async Task<IHttpActionResult> UploadParameters([FromBody] UploadedParameter p)
        {


            var m = this.Request.Content.IsFormData();
            var dict = new Dictionary<string, string>();
            dict.Add("result", "Upload OK!" + p.Some_text);
            return Ok(content: dict);
        }

        /**
         * public string MyMethod([FromBody]JObject data)
         * {
         *      Customer customer = data["customerData"].ToObject<Customer>();
         *      Product product = data["productData"].ToObject<Product>();
         *      ....
         *      ....
         * }
         * 
         * */

        
        /// <summary>
        /// Can bind one parameter only, multiple FromBody parameter binding is not allowed
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/upload/parameters/frombody")]
        public async Task<IHttpActionResult> ParametersFromBody([FromBody] string a)
        {
            return Ok($@"{a}");
        }

        /// <summary>
        /// Read raw request content
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> ManualPostRawBuffer()
        {
            string result = await Request.Content.ReadAsStringAsync();
            return result;

        }

        public class UploadedParameter
        {
            public string Some_text { get; set; }
        }



    }
}
