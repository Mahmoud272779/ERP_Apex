using App.Domain.Entities.Process;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Persistence.Configurations.Process
{
   public class ColorsHistoryConfigurations : IEntityTypeConfiguration<InvColorsHistory>
    {
        public void Configure(EntityTypeBuilder<InvColorsHistory> builder)
        {
            builder.ToTable("InvColorsHistory");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.employeesId).HasDefaultValue(1);

        }
    }
}
