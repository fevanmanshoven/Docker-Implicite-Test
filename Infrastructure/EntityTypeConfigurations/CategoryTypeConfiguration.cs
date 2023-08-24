using DockerImpliciteTest.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Squads.Infrastructure.EntityTypeConfigurations
{
    public class CategoryTypeConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            #region Define properties
            // Define properties
            builder.HasKey(x => x.CategoryId);
            builder.Property(x => x.CategoryId).IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Name).IsRequired();
            builder.HasMany(x => x.ImageUploads).WithMany(x => x.Categories);
            #endregion

            #region Define data
            // Define data
            Category category1 = new Category
            {
                CategoryId = -1,
                Name = "Mannen"
            };
            Category category2 = new Category
            {
                CategoryId = -2,
                Name = "Vrouwen"
            };
            Category category3 = new Category
            {
                CategoryId = -3,
                Name = "Jong"
            };
            Category category4 = new Category
            {
                CategoryId = -4,
                Name = "Oud"
            };
            Category category5 = new Category
            {
                CategoryId = -5,
                Name = "Positive"
            };
            Category category6 = new Category
            {
                CategoryId = -6,
                Name = "Negative"
            };
            Category category7 = new Category
            {
                CategoryId = -7,
                Name = "Random"
            };
            #endregion
            #region Seed data
            // Seed data
            builder.HasData
            (
                category1,
                category2,
                category3,
                category4,
                category5,
                category6
            );
            #endregion
        }
    }
}
