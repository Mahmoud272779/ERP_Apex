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
    public class GLBankBranchConfugrations : IEntityTypeConfiguration<GLBankBranch>
    {
        public void Configure(EntityTypeBuilder<GLBankBranch> builder)
        {
            builder.ToTable("GLBankBranch");

            builder.HasKey(c => new { c.BankId, c.BranchId });

            builder.HasOne(br => br.Branch)
                .WithMany(br => br.BankBranches)
                .HasForeignKey(e => e.BranchId);

            builder.HasOne(br => br.Bank)
                .WithMany(br => br.BankBranches)
                .HasForeignKey(br => br.BankId);
        }
    }
}
