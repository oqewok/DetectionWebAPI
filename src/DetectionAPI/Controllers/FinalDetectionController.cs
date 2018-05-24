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
using System.Runtime.Serialization;

namespace DetectionAPI.Controllers
{
    public class FinalDetectionController : ApiController
    {
        [HttpPost]
        [RealBasicAuthenticationFilter]
        [Route("api/f/detection")]
        public IHttpActionResult Detection([FromUri] string algorythm)
        {
            //check if algorythm passed in URI
            if (algorythm != null)
            {
                if (algorythm == "neuro")
                {
                    AlgorythmType = AvailableAlgs.Neuro;
                }

                else if (algorythm == "haar")
                {
                    AlgorythmType = AvailableAlgs.Haar;
                }

                else
                {
                    AlgorythmType = AvailableAlgs.Unknown;
                }
            }

            if (AlgorythmType == AvailableAlgs.Neuro)
            {
                return Ok();
            }

            if (AlgorythmType == AvailableAlgs.Haar)
            {
                return Ok();
            }

            var algNotSelectedMessage = new AlgNotSelectedMessage
            {
                MessageText = "Please, specify detection algorythm"
            };

            return BadRequest(algNotSelectedMessage.MessageText);
        }

        #region Properties

        public AvailableAlgs AlgorythmType;

        #endregion

    }

    public enum AvailableAlgs : int
    {
        Neuro = 0,
        Haar = 1,
        Unknown = 2
    }

    [DataContract]
    public class AlgNotSelectedMessage
    {
        [DataMember]
        [JsonProperty(PropertyName = "message")]
        public string MessageText { get; set; }
    }
}