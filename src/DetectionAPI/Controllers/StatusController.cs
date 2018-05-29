using System.Web.Http;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace DetectionAPI.Controllers
{
    public class StatusController : ApiController
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("api/status")]
        public IHttpActionResult Status()
        {
            var msg = new StatusMessage()
            {
                Status = "Service is up"
            };

            return Ok(msg);
        }
    }

    [DataContract]
    public class StatusMessage
    {
        [DataMember]
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }
}
