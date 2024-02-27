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
    public class OtherSettingsSafesConfigurations : IEntityTypeConfiguration<OtherSettingsSafes>
    {
        public void Configure(EntityTypeBuilder<OtherSettingsSafes> builder)
        {
            builder.ToTable("OtherSettingsSafes");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.GLSafe).WithMany(x => x.OtherSettingsSafes).HasForeignKey(x => x.gLSafeId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.otherSettings).WithMany(x => x.otherSettingsSafes).HasForeignKey(x => x.otherSettingsId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
