using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization;

namespace DetectionAPI.Database.Entities
{
    public sealed class Session
    {
        #region Configuration
        public sealed class SessionConfiguration : EntityTypeConfiguration<Session>
        {
            public SessionConfiguration()
            {
                ToTable("Sessions");

                HasKey(e => e.SessionId);

                Property(e => e.SessionId)
                    .IsRequired()
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

                Property(e => e.ImageCount)
                    .IsRequired();

                Property(e => e.PlatesCount)
                    .IsRequired();

                Property(e => e.CreationTime)
                    .IsRequired();

                Property(e => e.ExpiryDate)
                    .IsRequired();

                Property(e => e.SessionType)
                    .IsRequired();

                Property(e => e.IsLimitReached)
                    .IsRequired();

            }

        }

        public static EntityTypeConfiguration<Session> CreateConfiguration() => new SessionConfiguration();
        #endregion


        #region Properties

        public long SessionId { get; set; }

        public long ImageCount { get; set; }

        public long PlatesCount { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime ExpiryDate { get; set; }

        public long SessionType { get; set; }

        public bool IsLimitReached { get; set; }

        #endregion
    }
}
