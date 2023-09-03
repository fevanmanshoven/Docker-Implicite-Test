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
            builder.HasMany(x => x.PosCategories).WithMany(x => x.PostCategorieTests);
            builder.HasMany(x => x.PosImageUploads).WithMany(x => x.PostUploadTests);
            builder.HasMany(x => x.NegCategories).WithMany(x => x.NegCategorieTests);
            builder.HasMany(x => x.NegImageUploads).WithMany(x => x.NegUploadTests);
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
