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
   public class Discount_A_P_HistoryConfigurations : IEntityTypeConfiguration<InvDiscount_A_P_History>
    {
        public void Configure(EntityTypeBuilder<InvDiscount_A_P_History> builder)
        {
            builder.ToTable("InvDiscount_A_P_History");
            builder.HasKey(a => a.Id);
            builder.Property(e => e.employeesId).HasDefaultValue(1);
        }
    }
}
