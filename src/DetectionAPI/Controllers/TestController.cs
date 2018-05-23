using DetectionAPI.Database;
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
using System.Web.UI.WebControls;

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

        [HttpPost]
        [Route("api/test/image/load", Name = "TestControllerImageLoad")]
        public IHttpActionResult LoadImage([FromBody] string user_image, [FromBody] string access_token)
        {
            try
            {
                return CreatedAtRoute(routeName: "TestControllerImageLoad", routeValues: new { }, content: new { });
            }

            catch
            {
                return BadRequest();
            }

            //if (access_token!= null)
            //{
            //    Console.WriteLine($@"Access_token {access_token}");
            //}
            //else
            //{
            //    Console.WriteLine($@"Access_token not found in request's body");
            //}

            //if (user_image != null)
            //{
            //    Console.WriteLine($@"User_image {user_image}");
            //}
            //else
            //{
            //    Console.WriteLine($@"User_image not found in request's body");
            //}

            
        }


        ///!BAD
        //[HttpPost]
        //[Route("api/test/parameters", Name = "TestControllerBodyParameters")]
        //public IHttpActionResult HttpRequestBase(HttpRequestBase httpRequest)
        //{
        //    return Ok();
        //}


        //TODO: null, works with IIS only due to HttpContext
        [HttpPost]
        [Route("api/test/postimage")]
        public async Task<HttpResponseMessage> PostUserImage()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                ///Todo : null
                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {

                        int MaxContentLength = 1024 * 1024 * 3; //Size = 3 MB

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {

                            var message = string.Format("Please Upload image of type .jpg,.gif,.png.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {

                            var message = string.Format("Please Upload a file upto 3 mb.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else
                        {

                            //YourModelProperty.imageurl = userInfo.email_id + extension;
                            //  where you want to attach your imageurl

                            //if needed write the code to update the table

                            //var filePath = HttpContext.Current.Server.MapPath("~/Userimage/" + userInfo.email_id + extension);
                            //var filePath = HttpContext.Current.Server.MapPath("~/Userimage/" + Guid.NewGuid() + extension);

                            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DetectionAPI", "UserImages");
                            Directory.CreateDirectory(filePath);

                            string origNameAndExtension = postedFile.FileName.Trim('\"');
                            var origName = Path.GetFileNameWithoutExtension(origNameAndExtension);


                            string filename = origName + "_" + Guid.NewGuid().ToString() + extension;
                            filename = Path.Combine(filePath, filename);


                            //Userimage myfolder name where i want to save my image
                            postedFile.SaveAs(filename);

                        }
                    }

                    var message1 = string.Format("Image Updated Successfully.");
                    return Request.CreateErrorResponse(HttpStatusCode.Created, message1); ;
                }
                var res = string.Format("Please Upload a image.");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            catch (Exception ex)
            {
                var res = string.Format("Internal error occured");
                dict.Add("Error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
        }

        //
        [HttpPost]
        [Route("api/test/post/image")]
        [ResponseType(typeof(FileUpload))]
        public IHttpActionResult PostFileUpload()
        {
            if (HttpContext.Current.Request.Files.AllKeys.Any())
            {
                // Get the uploaded image from the Files collection  
                var httpPostedFile = HttpContext.Current.Request.Files["UploadedImage"];
                if (httpPostedFile != null)
                {
                    //FileUpload imgupload = new FileUpload();
                    //int length = httpPostedFile.ContentLength;
                    //imgupload.imagedata = new byte[length]; //get imagedata  
                    //httpPostedFile.InputStream.Read(imgupload.imagedata, 0, length);
                    //imgupload.imagename = Path.GetFileName(httpPostedFile.FileName);
                    //db.FileUploads.Add(imgupload);
                    //db.SaveChanges();
                    //// Make sure you provide Write permissions to destination folder
                    //string sPath = @"C:\Users\xxxx\Documents\UploadedFiles";
                    //var fileSavePath = Path.Combine(sPath, httpPostedFile.FileName);
                    //// Save the uploaded file to "UploadedFiles" folder  
                    //httpPostedFile.SaveAs(fileSavePath);
                    return Ok("Image Uploaded");
                }
            }
            return Ok("Image is not Uploaded");
        }

        //



        // TODO : good exmple too
        [HttpPost]
        [Route("api/test/post/img")]
        public async Task<HttpResponseMessage> PostFormData()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            try
            {
                //string root = HttpContext.Current.Server.MapPath("~/App_Data");

                string r = "~/App_Data";
                var provider = new MultipartFormDataStreamProvider(r);

                try
                {
                    // Read the form data.
                    await Request.Content.ReadAsMultipartAsync(provider);

                    // This illustrates how to get the file names.
                    foreach (MultipartFileData file in provider.FileData)
                    {
                        Console.WriteLine(file.Headers.ContentDisposition.FileName);
                        Console.WriteLine("Server file path: " + file.LocalFileName);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (System.Exception e)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
                }

            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc);
            }

        }
        //


        [HttpPost]
        [Route("api/test/body_parameters", Name = "TestControllerBodyParameters")]
        public IHttpActionResult BodyParams([FromBody] string user_name)
        {
             
            HttpContent requestContent = Request.Content;
            string jsonContent = requestContent.ReadAsStringAsync().Result;


            var cs = Newtonsoft.Json.JsonConvert.SerializeObject(user_name, typeof(string) , Newtonsoft.Json.Formatting.None, null);

            var cd = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent);
            //if (access_token != null)
            //{
            //    Console.WriteLine($@"Access_token {access_token}");
            //}
            //else
            //{
            //    Console.WriteLine($@"Access_token not found in request's body");
            //}

            if (user_name != null)
            {
                Console.WriteLine($@"User_name {user_name}");
            }
            else
            {
                Console.WriteLine($@"User_name not found in request's body");
            }

            return Ok();
        }


        public class PostingValue
        {
            //TODO : look at this
            [Required(ErrorMessage = "Id is required parameter")]
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


        [HttpGet]
        [Route("api/db")]
        public IHttpActionResult Db()
        {
            try
            {
                ApiDbContext dbContext = new ApiDbContext();

                var allUsers = dbContext.Users;

                foreach(var u in allUsers)
                {
                    Console.WriteLine(u.ToString());
                }
                
            }

            catch(Exception exc)
            {

            }

            return Ok();
        }
    }
}


//TODO ; client call
/***
private static string Consume(string endpoint, string user, string password)
{
    var client = new HttpClient();
    client.BaseAddress = new Uri(endpoint);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

    var credential = new Credentials { User = user, Password = password };
    var credentialString = Newtonsoft.Json.JsonConvert.SerializeObject(credential, Formatting.None);


    var response = client.PostAsync("api/ticket", credentialString).Result;
    response.EnsureSuccessStatusCode();
    if (response.IsSuccessStatusCode)
            ... //process

    return string.Empty;
}


/***/