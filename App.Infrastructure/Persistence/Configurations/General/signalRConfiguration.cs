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
    internal class signalRConfiguration : IEntityTypeConfiguration<signalR>
    {
        public void Configure(EntityTypeBuilder<signalR> builder)
        {
            builder.ToTable("signalR");
            builder.HasKey(e => e.Id);
            builder.HasOne(e=> e.InvEmployees).WithMany(e=> e.signalR).HasForeignKey(x=> x.InvEmployeesId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
