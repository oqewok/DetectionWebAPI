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
        public long SessionId { get; set; }

        public long ImageCount { get; set; }

        public long PlatesCount { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime ExpiryDate { get; set; }

        public long SessionType { get; set; }

        public bool IsLimitReached { get; set; }
    }
}
