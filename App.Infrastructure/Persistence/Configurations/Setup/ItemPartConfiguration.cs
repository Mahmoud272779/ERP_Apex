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
    public class ItemPartConfiguration : IEntityTypeConfiguration<InvStpItemCardParts>
    {
        public void Configure(EntityTypeBuilder<InvStpItemCardParts> builder)
        {
            builder.ToTable("InvStpItemCardParts");
            // builder.HasKey(e => e.Id);

             builder.HasKey(e => new { e.ItemId, e.PartId });
             builder.HasOne(e => e.CardMaster).WithMany(r => r.Parts).HasForeignKey(f => f.ItemId).OnDelete(DeleteBehavior.Cascade);
             builder.HasOne(e => e.PartDetails).WithMany(r => r.ItemParts).HasForeignKey(f => f.PartId).OnDelete(DeleteBehavior.NoAction);
             builder.HasOne(e => e.Unit).WithMany(r => r.ItemParts).HasForeignKey(f => f.UnitId);

        }
    }
}
