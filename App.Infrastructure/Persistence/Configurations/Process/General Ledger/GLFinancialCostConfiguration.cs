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
    public class GLFinancialCostConfiguration : IEntityTypeConfiguration<GLFinancialCost>
    {
        public void Configure(EntityTypeBuilder<GLFinancialCost> builder)
        {
            builder.ToTable("GLFinancialCost");
            builder.HasKey(c => new { c.CostCenterId, c.FinancialAccountId });
  
        }
    }
}
