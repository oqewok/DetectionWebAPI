using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization;

namespace DetectionAPI.Database.Entities
{
    public sealed class User
    {
        #region Configuration

        public sealed class UserConfiguration : EntityTypeConfiguration<User>
        {
            public UserConfiguration()
            {
                

                ToTable("Users");
                HasKey(e => e.UserId);

                Property(e => e.UserId)
                    .IsRequired()
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

                Property(e => e.CreationTime)
                    .IsRequired();

                Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode();

                Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode();

                Property(e => e.AccessToken)
                    .IsUnicode();
            }
        }

        public static EntityTypeConfiguration<User> CreateConfiguration() => new UserConfiguration();

        #endregion

        #region Properties

        public long UserId { get; set; }

        public long UserType { get; set; }

        public DateTime CreationTime { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string AccessToken { get; set; }

        public long CurrentSessionId { get; set; }

        #endregion

        public override string ToString() => $@"Id :{UserId}, User type: {UserType}, Created: {CreationTime}, Username: {Username}, Password: {Password}, AccessToken: {AccessToken}, SeesionId: {CurrentSessionId}";

    }
}
