using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DetectionAPI.Database.Entities
{
    public class ImageInfo
    {
        public int ImageId { get; set; }

        public string ImagePath { get; set; }

        public string MarkupPath { get; set; }

        public int PlatesCount { get; set; }

        public int UserId { get; set; }

        public int SessionId { get; set; }

        public DateTime UploadDate { get; set; }

    }
}
