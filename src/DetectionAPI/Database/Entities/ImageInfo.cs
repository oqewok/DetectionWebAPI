using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DetectionAPI.Database.Entities
{
    public class ImageInfo
    {
        #region Configuration

        public sealed class ImageInfoConfiguration : EntityTypeConfiguration<ImageInfo>
        {
            public ImageInfoConfiguration()
            {
                ToTable("ImageInfo");

                HasKey(e => e.ImageId);

                Property(e => e.ImageId)
                    .IsRequired()
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

                Property(e => e.ImagePath)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode();

                Property(e => e.MarkupPath)
                    .HasMaxLength(250)
                    .IsUnicode();

                Property(e => e.PlatesCount)
                    .IsRequired();

                Property(e => e.UserId)
                    .IsRequired();

                Property(e => e.SessionId)
                    .IsRequired();

                Property(e => e.UploadDate)
                    .IsRequired();
            }
        }

        public static EntityTypeConfiguration<ImageInfo> CreateConfiguration() => new ImageInfoConfiguration();

        #endregion


        #region Properties

        public long ImageId { get; set; }

        public string ImagePath { get; set; }

        public string MarkupPath { get; set; }

        public long PlatesCount { get; set; }

        //FK
        public long UserId { get; set; }

        //FK

        public long SessionId { get; set; }

        public DateTime UploadDate { get; set; }

        //Navigational property
        [ForeignKey("SessionId")]
        public virtual Session Session { get; set; }

        #endregion

        public override string ToString() => $@"ImageId: {ImageId}, ImagePath:{ImagePath}, MarkupPath: {MarkupPath}, PlatesCount: {PlatesCount}, UserId: {UserId}, SessionId: {SessionId}, UploadDate: {UploadDate}";
    }
}
