using DockerImpliciteTest.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Squads.Infrastructure.EntityTypeConfigurations
{
    public class ImageUploadTypeConfiguration : IEntityTypeConfiguration<ImageUpload>
    {
        public void Configure(EntityTypeBuilder<ImageUpload> builder)
        {
            #region Define properties
            // Define properties
            builder.HasKey(x => x.ImageUploadId);
            builder.Property(x => x.ImageUploadId).IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Name).IsRequired();
            #endregion

            #region Define data
            // Define data
            #endregion
            #region Seed data
            // Seed data
            #endregion
        }
    }
}
