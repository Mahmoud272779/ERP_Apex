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
    public class GLBanksConfiguration : IEntityTypeConfiguration<GLBank>
    {
        public void Configure(EntityTypeBuilder<GLBank> builder)
        {
            builder.ToTable("GLBanks");
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.FinancialAccount)
                   .WithMany(q => q.Banks)
                   .HasForeignKey(q => new { q.FinancialAccountId })
                   .OnDelete(DeleteBehavior.NoAction);          
        }
    }
}
