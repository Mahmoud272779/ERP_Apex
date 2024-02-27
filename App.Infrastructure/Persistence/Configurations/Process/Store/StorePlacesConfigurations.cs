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
    public class StorePlacesConfigurations : IEntityTypeConfiguration<InvStorePlaces>
    {
        public void Configure(EntityTypeBuilder<InvStorePlaces> builder)
        {
            builder.ToTable("InvStorePlaces");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.ArabicName).IsRequired();
            builder.HasIndex(a => a.Code).IsUnique();
            builder.HasMany(x => x.Items).WithOne(x => x.StorePlace).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
