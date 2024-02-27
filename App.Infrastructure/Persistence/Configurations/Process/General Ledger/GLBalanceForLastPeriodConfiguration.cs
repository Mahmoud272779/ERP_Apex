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
    class GLBalanceForLastPeriodConfiguration : IEntityTypeConfiguration<GLBalanceForLastPeriod>
    {
        public void Configure(EntityTypeBuilder<GLBalanceForLastPeriod> builder)
        {
            builder.ToTable("GLBalanceForLastPeriod");
            builder.HasKey(e => e.Id);
        }
    }
}
