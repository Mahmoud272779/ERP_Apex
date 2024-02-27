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
    public class CommissionSlidesConfigurations : IEntityTypeConfiguration<InvCommissionSlides>
    {
        public void Configure(EntityTypeBuilder<InvCommissionSlides> builder)
        {
            builder.ToTable("InvCommissionSlides");
            builder.HasKey(e => e.Id);
            builder.HasOne(e => e.CommList).WithMany(e => e.Slides).HasForeignKey(e => e.CommissionId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
