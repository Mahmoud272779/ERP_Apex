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
    public class GLSafeConfigurations : IEntityTypeConfiguration<GLSafe>
    {
        public void Configure(EntityTypeBuilder<GLSafe> builder)
        {
            builder.ToTable("GLSafe");
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.financialAccount)
                   .WithMany(q => q.Treasuries)
                   .HasForeignKey(q => new { q.FinancialAccountId })
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.Branch)
                   .WithMany(q => q.Treasuries)
                   .HasForeignKey(q => new { q.BranchId })
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
