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
    public class GLFinancialAccountBranchesConfigurations : IEntityTypeConfiguration<GLFinancialAccountBranch>
    {
        public void Configure(EntityTypeBuilder<GLFinancialAccountBranch> builder)
        {
            builder.ToTable("GLFinancialAccountBranch");
            builder.HasKey(c => new { c.BranchId, c.FinancialAccountId });
      
        }
    }
}
