using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DetectionAPI.Controllers
{
    public class UploadController : ApiController
    {
        //public async Task<List<string>> PostMultipartStream()
        //{
        //    // Verify that this is an HTML Form file upload request
        //    if (!Request.Content.IsMimeMultipartContent("form-data"))
        //    {
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //    }
     
        //    // Create a stream provider for setting up output streams that saves the output under c:\tmp\uploads
        //    // If you want full control over how the stream is saved then derive from MultipartFormDataStreamProvider
        //    // and override what you need.
        //    MultipartFormDataStreamProvider streamProvider = new MultipartFormDataStreamProvider("c:\\tmp\\uploads");
     
        //    // Read the MIME multipart content using the stream provider we just created.
        //    IEnumerable<HttpContent> bodyparts = await Request.Content.ReadAsMultipartAsync(streamProvider);
     
        //    // The submitter field is the entity with a Content-Disposition header field with a "name" parameter with value "submitter"
        //    string submitter;
        //    if (!bodyparts.TryGetFormFieldValue("submitter", out submitter))
        //    {
        //        submitter = "unknown";
        //    }
     
        //    // Get a dictionary of local file names from stream provider.
        //    // The filename parameters provided in Content-Disposition header fields are the keys.
        //    // The local file names where the files are stored are the values.
        //    IDictionary<string, string> bodyPartFileNames = streamProvider.BodyPartFileNames;
     
        //    // Create response containing information about the stored files.
        //    List<string> result = new List<string>();
        //    result.Add(submitter);
     
        //    IEnumerable<string> localFiles = bodyPartFileNames.Select(kv => kv.Value);
        //    result.AddRange(localFiles);
     
        //    return result;
        //}



       




    }
}
