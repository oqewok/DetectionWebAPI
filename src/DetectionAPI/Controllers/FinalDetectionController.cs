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
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace DetectionAPI.Controllers
{
    public class FinalDetectionController : ApiController
    {
        [HttpPost]
        [RealBearerAuthenticationFilter]
        [Route("api/f/detection")]
        public IHttpActionResult Detection([FromUri] string algorythm)
        {
            object syncRoot = new object();

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
            long currentImageId = -1;

            //Check limits
            using(var dbContext = new ApiDbContext())
            {
                var certainUser = dbContext.Set<User>().Where(p => p.AccessToken == authorizedUserToken).ToList().FirstOrDefault();
                var certainImage = dbContext.Set<ImageInfo>().ToList().Last();

                if (certainUser != null)
                {
                    currentUserId = certainUser.Id;
                }

                if (certainImage != null)
                {
                    currentImageId = certainImage.ImageId + 1;
                }
            }

            if (currentUserId == -1)
            {
                return BadRequest();
            }

            var availableLimits = CheckHelper.CheckLimitByUserId(currentUserId);

            long lastSavedImageId = -1;
            string fullName = string.Empty;

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

                
                //Check if multipart/form-data
                if (!Request.Content.IsMimeMultipartContent())
                {
                    var msg = new AlgNotSelectedMessage
                    {
                        MessageText = "Your request shold be POST and content should be an image as multipart/form-data",
                    };

                    return BadRequest(msg.MessageText);
                }

                //TODO : You are here
                //Load and save image, create record with new ImageInfo (in Images table)
                if (Request.Content.IsMimeMultipartContent())
                {
                    //For larger files, this might need to be added:
                    //Request.Content.LoadIntoBufferAsync().Wait();
                    try
                    {
                        Task myTask = new Task(() =>
                        {
                            Request.Content.LoadIntoBufferAsync().Wait();
                            Request.Content.ReadAsMultipartAsync(
                            new MultipartMemoryStreamProvider()).ContinueWith((task) =>
                            {
                                MultipartMemoryStreamProvider provider = task.Result;

                                foreach (HttpContent content in provider.Contents)
                                {
                                    Stream stream = content.ReadAsStreamAsync().Result;
                                    Image image = Image.FromStream(stream);
                                    var testName = content.Headers.ContentDisposition.Name;
                                    //String filePath = HostingEnvironment.MapPath("~/Images/");
                                    string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DetectionAPI", "Images", currentUserId.ToString());
                                    Directory.CreateDirectory(filePath);

                                    //Note that the ID is pushed to the request header,
                                    //not the content header:
                                    //String[] headerValues = (String[])Request.Headers.GetValues("image_token");
                                    string headerValues = "image";

                                    var origNameAndExtension = content.Headers.ContentDisposition.FileName.Trim('\"');
                                    var origName = Path.GetFileNameWithoutExtension(origNameAndExtension);

                                    //string fileName = headerValues + "_" + origName + "_" + Guid.NewGuid().ToString() + ".jpg";
                                    string fileName = currentSessionId.ToString() + "_" + currentImageId + ".jpg";

                                    //string tmpName = Guid.NewGuid().ToString();
                                    //String fileName = tmpName + ".jpg";
                                    string fullPath = Path.Combine(filePath, fileName);

                                    var newImage = new ImageInfo
                                    {
                                        ImagePath = fullPath,
                                        PlatesCount = 0,
                                        SessionId = currentSessionId,
                                        UploadDate = DateTime.Now,
                                        UserId = currentSessionId,
                                    };


                                    using (var dbContext = new ApiDbContext())
                                    {
                                        using (var transaction = dbContext.Database.BeginTransaction())
                                        {
                                            try
                                            {
                                                dbContext.Images.Add(newImage);
                                                dbContext.SaveChanges();
                                                transaction.Commit();
                                            }

                                            catch (Exception exc)
                                            {
                                                transaction.Rollback();
                                            }

                                        }

                                    }

                                    image.Save(fullPath);
                                }
                            });
                        });

                        myTask.Start();

                        if (myTask.IsCompleted)
                        {
                            Console.WriteLine("Uploading image is complited");
                        }

                    }

                    catch (Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                    }

                    finally
                    {
                    }
                }
                else
                {
                    throw new HttpResponseException(Request.CreateResponse(
                            HttpStatusCode.NotAcceptable,
                            "This request is not properly formatted"));
                }

                //Jesus! I can't save image in another thread so i have to wait
                //used Task work = Task.Factory.StartNew to save image
                //and work.Wait() after it in this thread, but it doesn't work
                //I couldn't solve it with monitors
                Thread.Sleep(200);

                try
                {

                    using (var dbc = new ApiDbContext())
                    {

                        var lastSavedImage = dbc.Images.Where(p => p.UserId == currentUserId).Where(p => p.SessionId == currentUserId).ToList().Last();

                        //var lastSavedImage = dbc.Images.Where(p => p.ImageId == currentImageId).ToList().Last();

                        if (lastSavedImage != null)
                        {
                            lastSavedImageId = lastSavedImage.ImageId;
                            fullName = lastSavedImage.ImagePath;
                        }
                    }
                }
                finally
                {
                }
                
                //

                if (fullName == string.Empty)
                {
                    return BadRequest();
                }

                //Localization
                if (AlgorythmType == AvailableAlgs.Neuro)
                {
                    //
                    try
                    {
                       
                        Bitmap image1 = new Bitmap(fullName);

                        var detResult = null as PlateDetector.Detection.DetectionResult;

                        var networkDetector = new Detector(new AlgManager(new FasterRcnnProvider()));

                        detResult = networkDetector.Detect(image1);

                        int detectetPlatesCount = detResult.GetDetectionsList().Count;

                        using(var dbContext = new ApiDbContext())
                        {
                            //Update counts
                            var currentSession = dbContext.Sessions.Where(p => p.Id == currentSessionId).ToList().LastOrDefault();
                            currentSession.ImageCount++;
                            currentSession.PlatesCount += detectetPlatesCount;

                            //Update MarkupPath
                            if (detectetPlatesCount != 0)
                            {
                                var image = dbContext.Images.Where(p => p.UserId == currentUserId).Where(p => p.SessionId == currentUserId).ToList().LastOrDefault();
                                image.PlatesCount = detectetPlatesCount;

                                //Create user's markup folder
                                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DetectionAPI", "Markup", currentUserId.ToString());
                                Directory.CreateDirectory(filePath);

                                //Make filename
                                string fileName = currentSessionId.ToString() + "_" + currentImageId + ".json";

                                //Make full filepath
                                string fullPath = Path.Combine(filePath, fileName);

                                image.MarkupPath = fullPath;

                                //Write json
                                TextWriter writer = new StreamWriter(fullPath, false);

                                string jsonData = JsonConvert.SerializeObject(detResult, Formatting.Indented);
                                try
                                {
                                    writer.Write(jsonData);
                                }

                                finally
                                {
                                    writer.Close();
                                }
                            }
                            dbContext.SaveChanges();

                        }

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
                    //
                    try
                    {

                        Bitmap image1 = new Bitmap(fullName);

                        var detResult = null as PlateDetector.Detection.DetectionResult;

                        var networkDetector = new Detector(new AlgManager(new HaarCascadeProvider()));

                        detResult = networkDetector.Detect(image1);

                        int detectetPlatesCount = detResult.GetDetectionsList().Count;

                        using (var dbContext = new ApiDbContext())
                        {
                            //Update counts
                            var currentSession = dbContext.Sessions.Where(p => p.Id == currentSessionId).ToList().LastOrDefault();
                            currentSession.ImageCount++;
                            currentSession.PlatesCount += detectetPlatesCount;

                            //Update MarkupPath
                            if (detectetPlatesCount != 0)
                            {
                                var image = dbContext.Images.Where(p => p.UserId == currentUserId).Where(p => p.SessionId == currentUserId).ToList().LastOrDefault();
                                image.PlatesCount = detectetPlatesCount;

                                //Create user's markup folder
                                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DetectionAPI", "Markup", currentUserId.ToString());
                                Directory.CreateDirectory(filePath);

                                //Make filename
                                string fileName = currentSessionId.ToString() + "_" + currentImageId + ".json";

                                //Make full filepath
                                string fullPath = Path.Combine(filePath, fileName);

                                image.MarkupPath = fullPath;

                                //Write json
                                TextWriter writer = new StreamWriter(fullPath, false);

                                string jsonData = JsonConvert.SerializeObject(detResult, Formatting.Indented);
                                try
                                {
                                    writer.Write(jsonData);
                                }

                                finally
                                {
                                    writer.Close();
                                }
                            }
                            dbContext.SaveChanges();

                        }

                        return Ok(detResult);
                    }

                    catch (Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                        Console.WriteLine(exc.StackTrace);
                    }

                    return BadRequest();
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