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
    
    public class StoresHistoryConfigurations : IEntityTypeConfiguration<InvStoresHistory>
    {
        public void Configure(EntityTypeBuilder<InvStoresHistory> builder)
        {
            builder.ToTable("InvStoresHistory");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.employeesId).HasDefaultValue(1);
        }
    }
}
