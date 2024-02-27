using App.Domain.Entities.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Setup
{
    public class ItemUnitConfigurations : IEntityTypeConfiguration<InvStpItemCardUnit>
    {
        public void Configure(EntityTypeBuilder<InvStpItemCardUnit> builder)
        {
            builder.ToTable("InvStpItemUnit");
            builder.HasKey(e => new { e.ItemId, e.UnitId });


            builder.HasMany(e => e.ItemColorsSizes).WithOne(s => s.ItemUnit).HasForeignKey(r => new  { r.ItemId,r.UnitId}).OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(e => e.Unit).WithMany(s => s.CardUnits).HasForeignKey(r => r.UnitId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(e => e.Item).WithMany(s => s.Units).HasForeignKey(r => r.ItemId).OnDelete(DeleteBehavior.Cascade);
            builder.Property(e => e.ItemId).IsRequired();
            builder.Property(e => e.UnitId).IsRequired();
            builder.Property(e => e.ConversionFactor).IsRequired();
         //   builder.HasIndex(a => a.Barcode).IsUnique();

        }
    }
}
