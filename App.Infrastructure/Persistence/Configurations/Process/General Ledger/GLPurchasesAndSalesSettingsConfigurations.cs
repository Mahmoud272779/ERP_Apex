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
   public class GLPurchasesAndSalesSettingsConfigurations : IEntityTypeConfiguration<GLPurchasesAndSalesSettings>
    {
        public void Configure(EntityTypeBuilder<GLPurchasesAndSalesSettings> builder)
        {
            builder.ToTable("GLPurchasesAndSalesSettings");
            builder.HasKey(e => e.Id);
            builder.HasOne(x => x.GLBranch).WithMany(x => x.GLPurchasesAndSalesSettings).HasForeignKey(x => x.branchId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
