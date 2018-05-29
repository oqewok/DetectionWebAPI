using DetectionAPI.Database.Entities;
using System.Data.Common;
using System.Data.Entity;

namespace DetectionAPI.Database
{
    public class ApiDbContext : DbContext
    {
        static ApiDbContext()
        {
            System.Data.Entity.Database.SetInitializer(new ApiDbContextInitializer());
        }

        public ApiDbContext() : base("DetectionApiDatabase")
        {

        }

        public ApiDbContext(DbConnection connection, bool contextOwnsConnection = true)
            : base(connection, contextOwnsConnection)
        {

        }

        public DbSet<User> Users { get; set; }

        public DbSet<Session> Sessions { get; set; }

        public DbSet<ImageInfo> Images { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder
                .Configurations
                .Add(User.CreateConfiguration());

            modelBuilder
                .Configurations
                .Add(Session.CreateConfiguration());

            modelBuilder
                .Configurations
                .Add(ImageInfo.CreateConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
