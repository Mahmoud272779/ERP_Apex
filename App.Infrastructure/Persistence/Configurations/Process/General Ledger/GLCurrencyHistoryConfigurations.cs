using App.Domain.Entities.Process.General_Ledger;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Process.General_Ledger
{
    class GLCurrencyHistoryConfigurations : IEntityTypeConfiguration<GLCurrencyHistory>
    {
        public void Configure(EntityTypeBuilder<GLCurrencyHistory> builder)
        {
            builder.ToTable("GLCurrencyHistory");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.ArabicName).HasColumnName("ArabicName");
            builder.Property(e => e.LatinName).HasColumnName("LatinName");
            builder.Property(e => e.employeesId).HasDefaultValue(1);
        }
    }
}
