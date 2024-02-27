using App.Domain.Entities.Process;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Domain.Entities.Process.Store;

namespace App.Infrastructure.Persistence.Configurations.Process.Store
{
    

    public class InvPurchasesAdditionalCostsHistoryConfigurations : IEntityTypeConfiguration<InvPurchasesAdditionalCostsHistory>
    {
        public void Configure(EntityTypeBuilder<InvPurchasesAdditionalCostsHistory> builder)
        {
            builder.ToTable("InvPurchasesAdditionalCostsHistory");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.employeesId).HasDefaultValue(1);
        }
    }
}
