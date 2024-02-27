using App.Domain.Entities.Process;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Persistence.Configurations.Process
{
  public  class SizesHistoryConfigurations : IEntityTypeConfiguration<InvSizesHistory>
    {
        public void Configure(EntityTypeBuilder<InvSizesHistory> builder)
        {
            builder.ToTable("InvSizesHistory");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.employeesId).HasDefaultValue(1);
        }
    }
}
