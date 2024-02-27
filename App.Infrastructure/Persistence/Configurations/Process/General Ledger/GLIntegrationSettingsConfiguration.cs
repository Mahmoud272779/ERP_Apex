using App.Domain.Entities.Process;
using App.Domain.Entities.Process.General_Ledger;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Process
{
    public class GLIntegrationSettingsConfiguration : IEntityTypeConfiguration<GLIntegrationSettings>
    {
        public void Configure(EntityTypeBuilder<GLIntegrationSettings> builder)
        {
            builder.ToTable("GLIntegrationSettings");
            builder.HasKey(e => e.Id);
            builder.HasOne(x => x.GLFinancialAccount).WithMany(x => x.gLIntegrationSettings).HasForeignKey(x => x.GLFinancialAccountId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.GLBranch).WithMany(x => x.gLIntegrationSettings).HasForeignKey(x => x.GLBranchId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}
