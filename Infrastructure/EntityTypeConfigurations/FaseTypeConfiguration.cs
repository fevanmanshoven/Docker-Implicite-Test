using ImpliciteTesterServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Squads.Infrastructure.EntityTypeConfigurations
{
    public class FaseTypeConfiguration : IEntityTypeConfiguration<Fase>
    {
        public void Configure(EntityTypeBuilder<Fase> builder)
        {
            #region Define properties
            // Define properties
            builder.HasKey(x => x.FaseId);
            builder.Property(x => x.FaseId).IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Name).IsRequired();
            builder.HasMany(x => x.FaseTypeImages).WithOne(x => x.Fase).OnDelete(DeleteBehavior.Cascade);
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
