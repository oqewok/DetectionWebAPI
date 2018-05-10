using DetectionAPI.Models;
using DetectionAPI.Filters;
using DetectionAPI.Detection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Collections.Specialized;

namespace DetectionAPI.Misc.tmp
{
    public class Class3 : ApiController
    {
        //3
        //
        /// <summary>  
        /// Upload Document.....  
        /// </summary>        
        /// <returns></returns>  
        [HttpPost]
        //[Route("api/detection/MediaUpload")]
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
            string filename = String.Empty;

            //HttpContent imageFile = files[0];
            foreach (var imageFile in files)
            {
                var origNameAndExtension = imageFile.Headers.ContentDisposition.FileName.Trim('\"');
                var originalFileName = Path.GetFileNameWithoutExtension(origNameAndExtension);

                filename = originalFileName + "_" + Guid.NewGuid().ToString() + ".jpg";
                Stream input = await imageFile.ReadAsStreamAsync();

                var directoryName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DetectionAPI", "UserImages");
                Directory.CreateDirectory(directoryName);

                filename = Path.Combine(directoryName, filename);

                //string tempDocUrl = WebConfigurationManager.AppSettings["DocsUrl"];
                string tempDocUrl = "E:\\";

                /**
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
                **/

                //Directory.CreateDirectory(@directoryName);  
                using (Stream file = File.OpenWrite(filename))
                {
                    input.CopyTo(file);
                    //close file  
                    file.Close();
                }
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            //response.Headers.Add("X-ImageURL", filename);
            return response;

        }
    }
}
