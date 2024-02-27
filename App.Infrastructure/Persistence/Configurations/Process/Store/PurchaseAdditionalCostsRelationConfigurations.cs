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
    public class PurchaseAdditionalCostsRelationConfigurations : IEntityTypeConfiguration<InvPurchaseAdditionalCostsRelation>
    {
        public void Configure(EntityTypeBuilder<InvPurchaseAdditionalCostsRelation> builder)
        {
            builder.ToTable("InvPurchaseAdditionalCostsRelation");
            builder.HasKey(e => e.PurchaseAdditionalCostsId);


            builder.HasOne(a => a.InvoiceMaster).WithMany(e => e.InvPurchaseAdditionalCostsRelations)
          .HasForeignKey(w => w.InvoiceId).OnDelete(DeleteBehavior.NoAction);


            builder.HasOne(a => a.InvoiceAdditionalCosts).WithMany(e => e.InvPurchaseAdditionalCostsRelations)
          .HasForeignKey(w => w.AddtionalCostId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
