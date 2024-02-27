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
    public class InvPurchasesAdditionalCostsConfigurations : IEntityTypeConfiguration<InvPurchasesAdditionalCosts>
    {
        public void Configure(EntityTypeBuilder<InvPurchasesAdditionalCosts> builder)
        {
            builder.ToTable("InvPurchasesAdditionalCosts");
            builder.HasKey(e => e.PurchasesAdditionalCostsId);
            builder.Property(e => e.ArabicName).IsRequired();
            builder.HasIndex(a => a.Code).IsUnique();
        }
    }

}
