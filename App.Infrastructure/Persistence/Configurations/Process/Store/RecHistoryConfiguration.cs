using App.Domain.Entities.Process;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Persistence.Configurations.Process
{
    public class RecHistoryConfiguration : IEntityTypeConfiguration<RecHistory>
    {
        public void Configure(EntityTypeBuilder<RecHistory> builder)
        {
            builder.ToTable("RecHistory");
            builder.HasKey(e => e.Id);
           
        }
    }
}
