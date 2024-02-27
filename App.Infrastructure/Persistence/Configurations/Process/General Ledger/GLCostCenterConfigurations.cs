using App.Domain.Entities.Process;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Process
{
    public class GLCostCenterConfigurations : IEntityTypeConfiguration<GLCostCenter>
    {
        public void Configure(EntityTypeBuilder<GLCostCenter> builder)
        {
            builder.ToTable("GLCostCenter");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.ArabicName).HasColumnName("ArabicName");
            builder.Property(e => e.LatinName).HasColumnName("LatinName");
            builder.HasOne(e => e.Cost_Center)
                   .WithMany(e => e.costCenters)
                   .HasForeignKey(q => q.ParentId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
