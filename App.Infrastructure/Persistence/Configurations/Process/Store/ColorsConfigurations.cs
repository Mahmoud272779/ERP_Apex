using App.Domain.Entities.Process;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Persistence.Configurations.Process
{
    public class ColorsConfigurations : IEntityTypeConfiguration<InvColors>
    {

        public void Configure(EntityTypeBuilder<InvColors> builder)
        {
            builder.ToTable("InvColors");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.ArabicName).IsRequired();
            builder.HasIndex(a => a.Code).IsUnique();

        }
    }
}
