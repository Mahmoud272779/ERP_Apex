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
    public class ItemStoreConfigurations : IEntityTypeConfiguration<InvStpItemCardStores>
    {
        public void Configure(EntityTypeBuilder<InvStpItemCardStores> builder)
        {
            builder.ToTable("InvStpItemStores");
            builder.HasKey(e => new { e.ItemId, e.StoreId });
            builder.Property(e => e.DemandLimit).HasPrecision(18,6).IsRequired();
        }
    }
}
