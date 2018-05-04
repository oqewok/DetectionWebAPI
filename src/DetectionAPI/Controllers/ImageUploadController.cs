using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public async Task<IHttpActionResult> UploadParameters()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("result", "Upload OK!");
            return Ok(content: dict);
        }



    }
}
