using App.Domain.Entities;
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
    internal class ClientAccountsConfiguration : IEntityTypeConfiguration<userAccount>
    {
        public void Configure(EntityTypeBuilder<userAccount> builder)
        {
            builder.ToTable("userAccount");
            builder.HasKey(e => e.id);
        }
    }
}
