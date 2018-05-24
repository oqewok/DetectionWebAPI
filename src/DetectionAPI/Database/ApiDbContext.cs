using DetectionAPI.Database.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            //modelBuilder
            //    .Entity<User>()
            //    .HasMany(c => c.Sessions)
            //    .WithRequired(e => e.User)
            //    .HasForeignKey(o => o.Id);

            //modelBuilder
            //    .Entity<Session>()
            //    .HasRequired(c => c.User)
            //    .WithMany(e => e.Sessions)
            //    .HasForeignKey(o => o.UserId);

            //modelBuilder
            //    .Entity<ImageInfo>()
            //    .HasRequired(e => e.Session)
            //    .WithMany(e => e.Images)
            //    .HasForeignKey(o => o.SessionId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
