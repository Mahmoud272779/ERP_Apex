using App.Domain.Entities.Process;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Domain.Entities.Process.Restaurants;

namespace App.Infrastructure.Persistence.Configurations.Restaurants
{
    public class KitchensHistoryConfiguration : IEntityTypeConfiguration<KitchensHistory>
    {
        public void Configure(EntityTypeBuilder<KitchensHistory> builder)
        {
            builder.ToTable("KitchensHistory");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.employeesId).HasDefaultValue(1);

        }
    }
}
