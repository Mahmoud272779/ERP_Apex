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
    class FundsBanksSafesMasterConfigurations : IEntityTypeConfiguration<InvFundsBanksSafesMaster>
    {
        public void Configure(EntityTypeBuilder<InvFundsBanksSafesMaster> builder)
        {
            builder.ToTable("InvFundsBanksSafesMaster");
            builder.HasKey(e => e.DocumentId);
     
            builder.HasOne(a => a.Bank).WithMany(a => a.FundsBanksSafes).HasForeignKey(a => a.BankId)
               .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(a => a.Safe).WithMany(a => a.FundsBanksSafes).HasForeignKey(a => a.SafeId)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
