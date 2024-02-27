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
    public class OfferPriceDetailsConfigurations : IEntityTypeConfiguration<OfferPriceDetails>
    {
        public void Configure(EntityTypeBuilder<OfferPriceDetails> builder)
        {

            builder.ToTable("OfferPriceDetails");
            builder.HasKey(e => e.Id);

            builder.HasOne(a => a.OfferPriceMaster).WithMany(e => e.OfferPriceDetails)
                .HasForeignKey(w => w.InvoiceId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.Items).WithMany(e => e.OfferPriceDetails)
                .HasForeignKey(w => w.ItemId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.Units).WithMany(e => e.OfferPriceDetails)
                .HasForeignKey(w => w.UnitId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.Sizes).WithMany(e => e.OfferPriceDetails)
               .HasForeignKey(w => w.SizeId).OnDelete(DeleteBehavior.NoAction);

            //builder.HasOne(a => a.ItemUnits).WithOne(a => a.InvoicesDetails);
            //        builder
            //.HasOne(ad => ad.ItemUnits)
            //.WithOne(s => s.InvoicesDetails)
            //.HasForeignKey(ad => ad);

        }
    }
}
