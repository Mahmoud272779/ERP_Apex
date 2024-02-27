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
    public class ItemColorSizeConfigurations : IEntityTypeConfiguration<InvStpItemColorSize>
    {
        public void Configure(EntityTypeBuilder<InvStpItemColorSize> builder)
        {
            builder.ToTable("InvStpItemColorSize");
            builder
                .HasKey(e => new { e.ItemId,e.UnitId, e.ColorId, e.SizeId });
            builder
                .HasOne(e => e.Size)
                .WithMany(s => s.Items)
                .HasForeignKey(r => r.SizeId)
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .HasOne(e => e.Color)
                .WithMany(s => s.Items)
                .HasForeignKey(r => r.ColorId)
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .HasOne(e => e.Unit)
                .WithMany(s => s.ItemColorsSizes)
                .HasForeignKey(r => r.UnitId)
                .OnDelete(DeleteBehavior.NoAction);



        }
    }
}
