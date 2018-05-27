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
using System.Threading;
using PlateDetector.Detection;
using DetectionAPI.Helpers;
using DetectionAPI.Database;
using DetectionAPI.Database.Entities;

namespace DetectionAPI.Controllers
{
    public class FinalDetectionController : ApiController
    {
        [HttpPost]
        [RealBearerAuthenticationFilter]
        [Route("api/f/detection")]
        public IHttpActionResult Detection([FromUri] string algorythm)
        {
            var authorizedUserToken = Thread.CurrentPrincipal.Identity.Name;

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

            if (AlgorythmType == AvailableAlgs.Unknown)
            {
                var algNotSelectedMessage = new AlgNotSelectedMessage
                {
                    MessageText = "Please, specify detection algorythm"
                };

                return BadRequest(algNotSelectedMessage.MessageText);
            }

            long currentUserId = -1;

            //Check limits
            using(var dbContext = new ApiDbContext())
            {
                var certainUser = dbContext.Set<User>().Where(p => p.AccessToken == authorizedUserToken).ToList().FirstOrDefault();

                if (certainUser != null)
                {
                    currentUserId = certainUser.Id;
                }
            }

            if (currentUserId == -1)
            {
                return BadRequest();
            }

            var availableLimits = CheckHelper.CheckLimitByUserId(currentUserId);

            //return if limit is reached already
            if (availableLimits.IsLimitReached == true)
            {
                var limitReachedMessage = new LimitReachedMessage
                {
                    AvailableLimits = availableLimits,
                    MessageText = "Your current limit is reached for a period, check it and try again or change your plan"
                };

                return BadRequest(limitReachedMessage.MessageText);
            }

            else
            {
                //Set current SessionId for updating session data (images count and plates count)
                var currentSessionId = CheckHelper.CheckExpirySessionByUserId(currentUserId);

                //TODO : You are here
                //check if multipart/form-data
                if (!Request.Content.IsMimeMultipartContent())
                {
                    var msg = new AlgNotSelectedMessage
                    {
                        MessageText = "Your content should be POST and multipart/form-data",
                    };

                    return BadRequest(msg.MessageText);
                }

                //Localization
                if (AlgorythmType == AvailableAlgs.Neuro)
                {
                    //
                    try
                    {
                        Bitmap image1 = new Bitmap("E:\\rus_car_front.jpg");

                        var detResult = null as PlateDetector.Detection.DetectionResult;

                        var networkDetector = new Detector(new AlgManager(new FasterRcnnProvider()));

                        detResult = networkDetector.Detect(image1);

                        return Ok(detResult);
                    }

                    catch (Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                        Console.WriteLine(exc.StackTrace);
                    }

                    return BadRequest();
                }

                if (AlgorythmType == AvailableAlgs.Haar)
                {
                    return Ok();
                }

                return BadRequest();
            }
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

    [DataContract]
    public class LimitReachedMessage
    {
        [DataMember]
        [JsonProperty(PropertyName = "message")]
        public string MessageText { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "limits")]
        public AvailableLimits AvailableLimits { get; set; }
    }
}