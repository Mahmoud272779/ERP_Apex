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
   public class GLGeneralSettingConfigurations : IEntityTypeConfiguration<GLGeneralSetting>
    {
        public void Configure(EntityTypeBuilder<GLGeneralSetting> builder)
        {
            builder.ToTable("GLGeneralSetting");
            builder.HasKey(e => e.Id);
        }
    }
}
