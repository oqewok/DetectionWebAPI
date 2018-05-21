using DetectionAPI.Database.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectionAPI.Database
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext() : base ("DetectionApiDatabase")
        {

        }



        public DbSet<User> Users { get; set; }
    }
}
