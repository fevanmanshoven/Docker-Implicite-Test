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
    public class FaseTypeImageConfiguration : IEntityTypeConfiguration<FaseTypeImage>
    {
        public void Configure(EntityTypeBuilder<FaseTypeImage> builder)
        {
            #region Define properties
            // Define properties
            builder.HasKey(x => x.FaseTypeImageId);
            builder.Property(x => x.FaseTypeImageId).IsRequired().ValueGeneratedOnAdd();
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
