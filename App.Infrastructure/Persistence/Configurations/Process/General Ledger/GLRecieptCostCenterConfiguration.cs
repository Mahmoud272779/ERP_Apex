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
    public class GLRecieptCostCenterConfiguration : IEntityTypeConfiguration<GLRecieptCostCenter>
    {
        public void Configure(EntityTypeBuilder<GLRecieptCostCenter> builder)
        {
            builder.ToTable("GLRecieptCostCenter");
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Reciept)
                   .WithMany(q => q.RecieptsCostCenters)
                   .HasForeignKey(q => new { q.SupportId })
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.CostCenter)
                .WithMany(q => q.supportCostCenters)
                .HasForeignKey(q => new { q.CostCenterId })
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
