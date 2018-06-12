using System.Web.Http;
using System.Web.Http.Cors;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace DetectionAPI.Controllers
{
    /// <summary>
    /// Класс, принимающий запросы удаленных пользователей и отвечающий за
    /// передачу уведомлений о доступности сервиса
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class StatusController : ApiController
    {
        /// <summary>
        /// Обрабатывает запрос удаленного пользователя о досупности сервиса и гоовности
        /// обрабатывать иные запросы
        /// </summary>
        /// <returns><see cref="StatusMessage"/></returns>
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

    /// <summary>
    /// Структура сообщения о доступности сервиса
    /// </summary>
    [DataContract]
    public class StatusMessage
    {
        [DataMember]
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }
}
