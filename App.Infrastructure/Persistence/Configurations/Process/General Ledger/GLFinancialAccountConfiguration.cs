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
    public class GLFinancialAccountConfiguration : IEntityTypeConfiguration<GLFinancialAccount>
    {
        public void Configure(EntityTypeBuilder<GLFinancialAccount> builder)
        {
            builder.ToTable("GLFinancialAccount");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.ArabicName).HasColumnName("ArabicName");
            builder.Property(e => e.LatinName).HasColumnName("LatinName");
            builder.Property(x => x.HasCostCenter).HasDefaultValue(0);
            builder.HasOne(e=>e.Currency)
                   .WithMany(q=>q.financialAccounts)
                   .HasForeignKey(q=>new { q.CurrencyId })
                   .OnDelete(DeleteBehavior.NoAction);
            

            builder.HasOne(e => e.financialAccount)
                  .WithMany(e => e.financialAccounts)
                  .HasForeignKey(q => q.ParentId)
                  .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
