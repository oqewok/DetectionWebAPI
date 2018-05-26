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
using DetectionAPI.Database;
using DetectionAPI.Database.Entities;
using System.Threading;

namespace DetectionAPI.Controllers
{
    public class TestingTokensController : ApiController
    {
        [HttpPost]
        [RealBearerAuthenticationFilter]
        [Route("api/authtoken")]
        public IHttpActionResult TestingTokens()
        {
            return Ok();
        }
    }
}
