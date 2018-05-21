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
        public long ImageId { get; set; }

        public string ImagePath { get; set; }

        public string MarkupPath { get; set; }

        public long PlatesCount { get; set; }

        public long UserId { get; set; }

        public long SessionId { get; set; }

        public DateTime UploadDate { get; set; }

    }
}
