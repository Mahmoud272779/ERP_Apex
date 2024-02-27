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
    public class ItemSerialConfigurations : IEntityTypeConfiguration<InvStpItemCardSerials>
    {
        public void Configure(EntityTypeBuilder<InvStpItemCardSerials> builder)
        {
            builder.ToTable("InvStpItemCardSerials");
            builder.HasKey(e => new { e.ItemId, e.SerialNo });
         //   builder.HasOne(e => e.Item).WithMany(s => s.ItemSerials).HasForeignKey(e => e.ItemId).OnDelete(DeleteBehavior.NoAction);
            builder.HasAlternateKey(e => e.SerialNo);
        }
    }
}
