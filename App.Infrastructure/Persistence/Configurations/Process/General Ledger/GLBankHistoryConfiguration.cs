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
    public class GLBankHistoryConfiguration : IEntityTypeConfiguration<GLBanksHistory>
    {
        public void Configure(EntityTypeBuilder<GLBanksHistory> builder)
        {
            builder.ToTable("GLBanksHistory");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.employeesId).HasDefaultValue(1);

        }
    }
}
