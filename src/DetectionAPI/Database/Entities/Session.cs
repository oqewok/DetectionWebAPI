using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DetectionAPI.Database.Entities
{
    public class Session
    {
        #region Configuration
        public sealed class SessionConfiguration : EntityTypeConfiguration<Session>
        {
            public SessionConfiguration()
            {
                ToTable("Sessions");

                HasKey(e => e.Id);

                Property(e => e.Id)
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

        //PK
        public long Id { get; set; }

        public long ImageCount { get; set; }

        public long PlatesCount { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime ExpiryDate { get; set; }

        public long SessionType { get; set; }

        public bool IsLimitReached { get; set; }

        //FK
        public long UserId { get; set; }

        [ForeignKey("UserId")]
        //Navigetional property
        public virtual User User { get; set; }

        public virtual ICollection<ImageInfo> Images { get; set; }

        #endregion

        public Session()
        {
            Images = new List<ImageInfo>();
        }

        public override string ToString() => $@"Id: {Id}, ImageCount: {ImageCount}, PlatesCount: {PlatesCount}, Created: {CreationTime}, Expiry: {ExpiryDate}, SessionType: {SessionType}, IsLimitReached: {IsLimitReached}, UserId: {UserId}";
    }
}
