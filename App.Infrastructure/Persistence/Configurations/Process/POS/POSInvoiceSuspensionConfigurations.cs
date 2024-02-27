using App.Domain.Entities.POS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Process.POS
{
    public class POSInvoiceSuspensionConfigurations : IEntityTypeConfiguration<POSInvoiceSuspension>
    {
        public void Configure(EntityTypeBuilder<POSInvoiceSuspension> builder)
        {
            builder.ToTable("POSInvoiceSuspension");
            builder.HasKey(e => e.InvoiceId);
            //builder.HasIndex(a => a.Code).IsUnique();

            builder.HasOne(a => a.Branch).WithMany(e => e.POSInvoiceSuspension)
                .HasForeignKey(w => w.BranchId).OnDelete(DeleteBehavior.NoAction);


            builder.HasOne(a => a.store).WithMany(e => e.POSInvoiceSuspension)
                .HasForeignKey(w => w.StoreId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.Person).WithMany(e => e.POSInvoiceSuspension)
          .HasForeignKey(w => w.PersonId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.salesMan).WithMany(e => e.POSInvoiceSuspension)
          .HasForeignKey(w => w.SalesManId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.Employee).WithMany(e => e.POSInvoiceSuspension)
          .HasForeignKey(w => w.EmployeeId).OnDelete(DeleteBehavior.NoAction);
            builder.Property(h => h.EmployeeId).HasDefaultValue(1);

        }
    }
}
