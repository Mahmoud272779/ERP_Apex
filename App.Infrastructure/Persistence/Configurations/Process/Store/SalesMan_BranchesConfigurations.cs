using App.Domain.Entities.Process.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Process.Store
{
   public  class SalesMan_BranchesConfigurations : IEntityTypeConfiguration<InvSalesMan_Branches>
    {
        public void Configure(EntityTypeBuilder<InvSalesMan_Branches> builder)
        {
            builder.ToTable("InvSalesMan_Branches");
            builder.HasKey(e => new { e.BranchId, e.SalesManId });

            builder.HasOne(e => e.SalesMan)
                   .WithMany(a => a.SalesManBranch)
                   .HasForeignKey(p => p.SalesManId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.Branch)
                   .WithMany(a => a.SalesManBranch)
                   .HasForeignKey(p => p.BranchId).OnDelete(DeleteBehavior.NoAction);

        }
    }
}
