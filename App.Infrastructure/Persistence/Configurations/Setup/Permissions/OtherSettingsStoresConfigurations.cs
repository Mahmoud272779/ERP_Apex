using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Setup
{
    public class OtherSettingsStoresConfigurations : IEntityTypeConfiguration<OtherSettingsStores>
    {
        public void Configure(EntityTypeBuilder<OtherSettingsStores> builder)
        {
            builder.ToTable("OtherSettingsStores");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.InvStpStores).WithMany(x => x.OtherSettingsStores).HasForeignKey(x => x.InvStpStoresId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.otherSettings).WithMany(x => x.OtherSettingsStores).HasForeignKey(x => x.otherSettingsId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
