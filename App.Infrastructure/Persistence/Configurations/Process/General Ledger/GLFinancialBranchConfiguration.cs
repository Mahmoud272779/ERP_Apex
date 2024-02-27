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
    class GLFinancialBranchConfiguration : IEntityTypeConfiguration<GLFinancialBranch>
    {
        public void Configure(EntityTypeBuilder<GLFinancialBranch> builder)
        {
            builder.ToTable("GLFinancialBranch");
            builder.HasKey(c => new { c.BranchId, c.FinancialId });
        }
    }
}
