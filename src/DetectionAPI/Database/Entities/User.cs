using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DetectionAPI.Database.Entities
{
    /// <summary>
    /// /// Класс, отображающий записи табицы базы данных Users в объект User и наоборот
    /// </summary>
    public class User
    {
        #region Configuration
        /// <summary>
        /// Конфигурация таблицы базы данных
        /// </summary>
        public sealed class UserConfiguration : EntityTypeConfiguration<User>
        {
            public UserConfiguration()
            {
                ToTable("Users");
                HasKey(e => e.Id);

                Property(e => e.Id)
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

        //PK
        public long Id { get; set; }

        public long UserType { get; set; }

        public DateTime CreationTime { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string AccessToken { get; set; }

        //FK
        public long? SessionId { get; set; }

        [ForeignKey("SessionId")]
        public virtual ICollection<Session> Sessions { get; set; }

        public User()
        {
            Sessions = new List<Session>();
        }

        #endregion

        public override string ToString() => $@"Id :{Id}, User type: {UserType}, Created: {CreationTime}, Username: {Username}, Password: {Password}, AccessToken: {AccessToken}, SessionId: {SessionId}";

    }
}
