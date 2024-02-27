using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Entities.Process.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Process
{
    internal class systemHistoryLogsConfiguration : IEntityTypeConfiguration<SystemHistoryLogs>
    {
        public void Configure(EntityTypeBuilder<SystemHistoryLogs> builder)
        {
            builder.ToTable("SystemHistoryLogs");
            builder.HasKey(e => e.Id);
            builder.HasOne(e=> e.GLBranch).WithMany(e=> e.SystemHistoryLogs).HasForeignKey(x=> x.BranchId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(e=> e.employees).WithMany(e=> e.SystemHistoryLogs).HasForeignKey(x=> x.employeesId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
