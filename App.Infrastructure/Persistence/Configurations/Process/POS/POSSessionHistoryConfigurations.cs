using App.Domain.Entities.POS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Process.POS
{
    public class POSSessionHistoryConfigurations : IEntityTypeConfiguration<POSSessionHistory>
    {
        public void Configure(EntityTypeBuilder<POSSessionHistory> builder)
        {
            builder.ToTable("POSSessionHistory");
            builder.HasKey(e => e.Id);
            //builder.HasIndex(a => a.Code).IsUnique();

            builder.HasOne(a => a.employees).WithMany(e => e.pOSSessionHistories)
                .HasForeignKey(w => w.employeesId).OnDelete(DeleteBehavior.NoAction);


            builder.HasOne(a => a.POSSession).WithMany(e => e.pOSSessionHistories)
                .HasForeignKey(w => w.POSSessionId).OnDelete(DeleteBehavior.NoAction);

        }
    }
}
