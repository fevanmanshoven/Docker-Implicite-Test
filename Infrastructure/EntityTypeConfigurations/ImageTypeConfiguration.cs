using DockerImpliciteTest.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Squads.Infrastructure.EntityTypeConfigurations
{
    public class ImageTypeConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            #region Define properties
            // Define properties
            builder.HasKey(x => x.ImageId);
            builder.Property(x => x.ImageId).IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Source).IsRequired();
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
