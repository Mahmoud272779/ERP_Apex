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
    public class GLFinancialSettingConfigurations : IEntityTypeConfiguration<GLFinancialSetting>
    {
        public void Configure(EntityTypeBuilder<GLFinancialSetting> builder)
        {
            builder.ToTable("GLFinancialSetting");
            builder.HasKey(e => e.Id);
            builder.HasOne(e => e.FinancialAccount)
                   .WithMany(d => d.financialSettings)
                   .HasForeignKey(q => q.FinancialAccountId);
        }
    }
}
