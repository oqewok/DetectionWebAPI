﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
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

        public class UploadedParameter
        {
            public string Some_text { get; set; }
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
