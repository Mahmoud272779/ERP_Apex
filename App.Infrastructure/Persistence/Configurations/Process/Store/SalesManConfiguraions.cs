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
    public class SalesManConfiguraions : IEntityTypeConfiguration<InvSalesMan>
    {
        public void Configure(EntityTypeBuilder<InvSalesMan> builder)
        {
            builder.ToTable("InvSalesMan");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.ArabicName).IsRequired();
            builder.HasOne(a => a.CommissionList).WithMany(a => a.SalesMan).HasForeignKey(a => a.CommissionListId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(a => a.persons).WithOne(a => a.SalesMan).HasForeignKey(a => a.SalesManId).OnDelete(DeleteBehavior.NoAction);

            //  builder.HasOne(a => a.Branch).WithMany(a => a.SalesMan).HasForeignKey(a => a.BranchId).OnDelete(DeleteBehavior.NoAction);
            builder.HasIndex(a => a.Code).IsUnique();
          //  builder.HasIndex(a => a.Email).IsUnique();
           // builder.HasIndex(a => a.Phone).IsUnique();

        }
    }
}
