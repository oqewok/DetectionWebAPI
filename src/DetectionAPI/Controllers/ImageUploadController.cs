using DetectionAPI.Models;
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


        /// <summary>
        /// Read raw binary data (image)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/imageupload/ReadBinaryImage")]
        public async Task<IHttpActionResult> ReadFormData()
        {
            try
            {
                var result = await Request.Content.ReadAsByteArrayAsync();

                using (var ms = new MemoryStream(result))
                {
                    MemoryStream memstr = new MemoryStream(result);

                    Image img = Image.FromStream(memstr);
                    ////or
                    //Bitmap pic = new Bitmap(memstr);
                    ////or
                    //Bitmap correctedPic = GetImageFromByteArray(result);

                    var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DetectionAPI");
                    Directory.CreateDirectory(path);

                    //save all pics
                    img.Save(path + "\\img" + Guid.NewGuid() + ".jpg");
                    //pic.Save(path + "\\" + Guid.NewGuid() + ".jpg");
                    //correctedPic.Save(path + "\\CORRECTED" + Guid.NewGuid() + ".jpg");
                }
            }

            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            return Ok();

        }

        [HttpPost]
        [Route("api/imageupload/ReadImageFile")]
        public async Task<HttpResponseMessage> ReadImageFile([FromBody] UploadedFileFormData data)
        {
            MediaTypeHeaderValue header_value = new MediaTypeHeaderValue("application/json");


            return Request.CreateResponse(statusCode: HttpStatusCode.OK, value: "asdfghjkl", mediaType: header_value);
        }

        /// <summary>  
        /// Upload Document.....  
        /// </summary>        
        /// <returns></returns>  
        [HttpPost]
        [Route("api/DocumentUpload/MediaUpload")]
        public async Task<HttpResponseMessage> MediaUpload()
        {
            // Check if the request contains multipart/form-data.  
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = await Request.Content.ReadAsMultipartAsync<InMemoryMultipartFormDataStreamProvider>(new InMemoryMultipartFormDataStreamProvider());
            //access form data  
            NameValueCollection formData = provider.FormData;
            //access files  
            IList<HttpContent> files = provider.Files;

            string URL = String.Empty;

            //HttpContent imageFile = files[0];
            foreach (var imageFile in files)
            {
                var originalFileName = imageFile.Headers.ContentDisposition.FileName.Trim('\"');

                string filename = Guid.NewGuid().ToString() + ".jpg";
                Stream input = await imageFile.ReadAsStreamAsync();

                var directoryName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DetectionAPI");
                Directory.CreateDirectory(directoryName);

                //string tempDocUrl = WebConfigurationManager.AppSettings["DocsUrl"];
                string tempDocUrl = "E:\\";

                if (formData["Image"] == "Image")
                {
                    var path = HttpRuntime.AppDomainAppPath;
                    directoryName = System.IO.Path.Combine(path, "ClientImage");
                    filename = System.IO.Path.Combine(directoryName, originalFileName);

                    //Deletion exists file  
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }

                    string DocsPath = tempDocUrl + "/" + "ClientImage" + "/";
                    URL = DocsPath + originalFileName;

                }

                //Directory.CreateDirectory(@directoryName);  
                using (Stream file = File.OpenWrite(filename))
                {
                    input.CopyTo(file);
                    //close file  
                    file.Close();
                }
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("DocsUrl", URL);
            return response;

        }

        public class UploadedParameter
        {
            public string Some_text { get; set; }
        }

        public class UploadedFileFormData
        {
            public string Token { get; set; }
            public Image Image { get; set; }
        }


        /// <summary>
        /// Function that converts from byte[] to Bitmap with correction of glitch of png files
        /// </summary>
        private static readonly ImageConverter _imageConverter = new ImageConverter();
        public static Bitmap GetImageFromByteArray(byte[] byteArray)
        {
            Bitmap bm = (Bitmap)_imageConverter.ConvertFrom(byteArray);

            if (bm != null && (bm.HorizontalResolution != (int)bm.HorizontalResolution ||
                               bm.VerticalResolution != (int)bm.VerticalResolution))
            {
                // Correct a strange glitch that has been observed in the test program when converting 
                //  from a PNG file image created by CopyImageToByteArray() - the dpi value "drifts" 
                //  slightly away from the nominal integer value
                bm.SetResolution((int)(bm.HorizontalResolution + 0.5f),
                                 (int)(bm.VerticalResolution + 0.5f));
            }

            return bm;
        }



    }
}
