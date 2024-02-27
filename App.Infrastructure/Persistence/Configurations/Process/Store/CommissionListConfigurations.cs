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
    public class CommissionListConfigurations : IEntityTypeConfiguration<InvCommissionList>
    {
        public void Configure(EntityTypeBuilder<InvCommissionList> builder)
        {
            builder.ToTable("InvCommissionList");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.ArabicName).IsRequired();
            builder.Property(e => e.Type).IsRequired();
            builder.HasMany(e => e.Slides).WithOne(a => a.CommList).HasForeignKey(a => a.CommissionId).OnDelete(DeleteBehavior.NoAction);
            builder.HasIndex(a => a.Code).IsUnique();
        }
    }
}
