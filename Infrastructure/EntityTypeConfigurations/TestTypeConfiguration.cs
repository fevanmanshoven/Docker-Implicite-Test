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
    public class TestTypeConfiguration : IEntityTypeConfiguration<Test>
    {
        public void Configure(EntityTypeBuilder<Test> builder)
        {
            #region Define properties
            // Define properties
            builder.HasKey(x => x.TestId);
            builder.Property(x => x.TestId).IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Name).IsRequired();
            builder.HasMany(x => x.PosCategories);
            builder.HasMany(x => x.PosImageUploads);
            builder.HasMany(x => x.NegCategories);
            builder.HasMany(x => x.NegImageUploads);
            builder.HasMany(x => x.Fases);
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
