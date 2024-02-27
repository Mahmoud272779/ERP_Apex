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
    public class OtherSettingsBanksConfigurations : IEntityTypeConfiguration<OtherSettingsBanks>
    {
        public void Configure(EntityTypeBuilder<OtherSettingsBanks> builder)
        {
            builder.ToTable("OtherSettingsBanks");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.GLBank).WithMany(x => x.OtherSettingsBanks).HasForeignKey(x => x.gLBankId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.otherSettings).WithMany(x => x.otherSettingsBanks).HasForeignKey(x => x.otherSettingsId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
