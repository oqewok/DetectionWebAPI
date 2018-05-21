using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DetectionAPI.Database.Entities
{
    public class Session
    {
        public int SessionId { get; set; }

        public int ImageCount { get; set; }

        public int PlatesCount { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime ExpiryDate { get; set; }

        public int SessionType { get; set; }

        public bool IsLimitReached { get; set; }
    }
}
