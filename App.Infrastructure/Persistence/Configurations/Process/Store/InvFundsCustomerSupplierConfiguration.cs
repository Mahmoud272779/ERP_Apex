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
    public class InvFundsCustomerSupplierConfiguration : IEntityTypeConfiguration<InvFundsCustomerSupplier>
    {
        public void Configure(EntityTypeBuilder<InvFundsCustomerSupplier> builder)
        {
            builder.ToTable("InvFundsCustomerSupplier");
            builder.HasKey(e => e.Id);
            builder.HasMany(e => e.Person).WithOne(a => a.FundsCustomerSuppliers).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.branch).WithMany(c => c.FundsCustomerSupplier).HasForeignKey(c => c.branchId).OnDelete(DeleteBehavior.NoAction);
            builder.Property(c => c.branchId).HasDefaultValue(1);
        }
    }
}
