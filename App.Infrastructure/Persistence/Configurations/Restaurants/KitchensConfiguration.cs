using App.Domain.Entities.Process.General_Ledger;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Domain.Entities.Process.Restaurants;
using System.Reflection.Emit;

namespace App.Infrastructure.Persistence.Configurations.Restaurants
{
    public class KitchensConfiguration : IEntityTypeConfiguration<Kitchens>
    {
        public void Configure(EntityTypeBuilder<Kitchens> builder)
        {
            builder.ToTable("Kitchens");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.ArabicName).IsRequired(true);
            builder.Property(e => e.Notes).HasMaxLength(1000);
            builder.HasMany(k => k.Categories).WithOne(c => c.kitchens).HasForeignKey(a =>a.kitchenId);
        }
    }
}
