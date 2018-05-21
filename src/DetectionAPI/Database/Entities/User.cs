using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DetectionAPI.Database.Entities
{
    public class User
    {
        public int UserId { get; set; }

        public int UserType { get; set; }

        public DateTime CreationTime { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string AccessToken { get; set; }

        public int CurrentSessionId { get; set; }  

    }
}
