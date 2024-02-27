using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Process
{
    class InvStpItemCardHistoryConfigurations : IEntityTypeConfiguration<InvStpItemCardHistory>
    {
        public void Configure(EntityTypeBuilder<InvStpItemCardHistory> builder)
        {
            builder.ToTable("InvStpItemCardHistory");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.employeesId).HasDefaultValue(1);
        }
    }
}
