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
   public class InvoiceMasterConfigurations : IEntityTypeConfiguration<InvoiceMaster>
    {
        public void Configure(EntityTypeBuilder<InvoiceMaster> builder)
        {
            builder.ToTable("InvoiceMaster");
            builder.HasKey(e => e.InvoiceId);
            builder.HasIndex(a => new { a.InvoiceType ,a.BranchId}).IsUnique();
           

            builder.HasOne(a => a.Branch).WithMany(e => e.InvoiceMaster)
                .HasForeignKey(w => w.BranchId).OnDelete(DeleteBehavior.NoAction);


            builder.HasOne(a => a.store).WithMany(e => e.InvoiceMaster)
                .HasForeignKey(w => w.StoreId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.Person).WithMany(e => e.InvoiceMaster)
          .HasForeignKey(w => w.PersonId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.salesMan).WithMany(e => e.InvoiceMaster)
          .HasForeignKey(w => w.SalesManId).OnDelete(DeleteBehavior.NoAction);

            //transfer
            builder.HasOne(a => a.storeTo).WithMany(e => e.InvoiceMasterTo)
          .HasForeignKey(w => w.StoreIdTo).OnDelete(DeleteBehavior.NoAction);
            builder.Property(h => h.StoreIdTo).IsRequired(false);


            builder.HasOne(a => a.Employee).WithMany(e => e.invoiceMasters)
          .HasForeignKey(w => w.EmployeeId).OnDelete(DeleteBehavior.NoAction);
            builder.Property(h => h.EmployeeId).HasDefaultValue(1);

           

        }
    }
}
