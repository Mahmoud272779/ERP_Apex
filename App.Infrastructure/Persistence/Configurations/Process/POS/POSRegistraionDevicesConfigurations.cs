using App.Domain.Entities.POS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Process.POS
{
    public class POSRegistraionDevicesConfigurations : IEntityTypeConfiguration<POS_OfflineDevices>
    {
        public void Configure(EntityTypeBuilder<POS_OfflineDevices> builder)
        {
            builder.ToTable("POS_OfflineDevices");
            builder.HasKey(e => e.Id);
        }
    }
}
