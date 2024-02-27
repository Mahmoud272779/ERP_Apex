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
    public class InvGeneralSettingsConfigurations : IEntityTypeConfiguration<InvGeneralSettings>
    {
        public void  Configure(EntityTypeBuilder<InvGeneralSettings> builder)
        {
            builder.ToTable("InvGeneralSettings");
            builder.HasKey(e => e.Id);
        }
    }
}
