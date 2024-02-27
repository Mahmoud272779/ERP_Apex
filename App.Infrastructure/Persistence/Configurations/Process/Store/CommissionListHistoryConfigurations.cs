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
    public class CommissionListHistoryConfigurations : IEntityTypeConfiguration<InvCommissionListHistory>
    {
        public void Configure(EntityTypeBuilder<InvCommissionListHistory> builder)
        {
            builder.ToTable("InvCommissionListHistory");
            builder.HasKey(a => a.Id);
            builder.Property(e => e.employeesId).HasDefaultValue(1);
        }
    }
}
