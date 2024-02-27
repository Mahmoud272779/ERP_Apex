using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Setup
{
    public class otherSettingsConfiguration : IEntityTypeConfiguration<otherSettings>
    {
        public void Configure(EntityTypeBuilder<otherSettings> builder)
        {
            builder.ToTable("otherSettings");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.userAccount).WithMany(x => x.otherSettings).HasForeignKey(x => x.userAccountId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
