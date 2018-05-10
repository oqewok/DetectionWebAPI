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
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;
using DetectionAPI.Detection.DetectionResult;

namespace DetectionAPI.Controllers
{
    public class DetectionController : ApiController
    {

        /// <summary>
        /// Method, that consumes request's form-data images and produces detection result
        /// </summary>
        /// <returns>Result of detection <see cref=""/> on success detection or <see cref="HttpStatusCode.Unauthorized"/>
        /// if user have not been authorized to perform this action</returns>
        [HttpPost]
        [RealBasicAuthenticationFilter]
        [Route("api/detection")]
        public async Task<IHttpActionResult> TryDetection()
        {




            DetectionResultProvider drp = new DetectionResultProvider();
            var dr = drp.DetectionResult();

            if (dr == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "Detection failed"));
            }

            return Ok(dr);
        }

    }
}
