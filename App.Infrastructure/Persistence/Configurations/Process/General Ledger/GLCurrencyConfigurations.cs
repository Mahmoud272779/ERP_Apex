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
    class GLCurrencyConfigurations : IEntityTypeConfiguration<GLCurrency>
    {
        public void Configure(EntityTypeBuilder<GLCurrency> builder)
        {
            builder.ToTable("GLCurrency");
            builder.HasKey(c => c.Id);
            //builder.HasOne(e => e.FinancialAccount)
            //       .WithOne(q => q.Currency)
            //       .HasForeignKey<Currency>(q => new { q.FinancialAccountId })
            //       .OnDelete(DeleteBehavior.Cascade);

        }
    }

    
}
