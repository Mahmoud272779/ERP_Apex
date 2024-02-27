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
   public class OfferPriceMasterConfigurations : IEntityTypeConfiguration<OfferPriceMaster>
    {
        public void Configure(EntityTypeBuilder<OfferPriceMaster> builder)
        {
            builder.ToTable("OfferPriceMaster");
            builder.HasKey(e => e.InvoiceId);
            builder.HasIndex(a => a.InvoiceType).IsUnique();

            builder.HasOne(a => a.Branch).WithMany(e => e.OfferPriceMaster)
                .HasForeignKey(w => w.BranchId).OnDelete(DeleteBehavior.NoAction);


            builder.HasOne(a => a.store).WithMany(e => e.OfferPriceMaster)
                .HasForeignKey(w => w.StoreId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.Person).WithMany(e => e.OfferPriceMaster)
          .HasForeignKey(w => w.PersonId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.salesMan).WithMany(e => e.OfferPriceMaster)
          .HasForeignKey(w => w.SalesManId).OnDelete(DeleteBehavior.NoAction);

           

            builder.HasOne(a => a.Employee).WithMany(e => e.OfferPriceMaster)
          .HasForeignKey(w => w.EmployeeId).OnDelete(DeleteBehavior.NoAction);
            builder.Property(h => h.EmployeeId).HasDefaultValue(1);

        }
    }
}
