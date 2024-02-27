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
    class FundsBanksSafesDetailsConfigurations : IEntityTypeConfiguration<InvFundsBanksSafesDetails>
    {
        public void Configure(EntityTypeBuilder<InvFundsBanksSafesDetails> builder)
        {
            builder.ToTable("InvFundsBanksSafesDetails");
            builder.HasKey(e => e.Id);
            builder.HasOne(a => a.FundsMaster_B_S).WithMany(a => a.FundsDetails_B_S).HasForeignKey(a => a.DocumentId)
         .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
