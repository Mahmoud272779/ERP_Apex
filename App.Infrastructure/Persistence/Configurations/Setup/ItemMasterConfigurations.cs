using App.Domain.Entities.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Setup
{
    public class ItemMasterConfigurations : IEntityTypeConfiguration<InvStpItemCardMaster>
    {
        public void Configure(EntityTypeBuilder<InvStpItemCardMaster> builder)
        {
            builder.ToTable("InvStpItemMaster");
           
            
            builder.HasKey(e => e.Id);
            
            builder.HasMany(e => e.Units).WithOne(s => s.Item).HasForeignKey(r => r.ItemId).OnDelete(DeleteBehavior.Cascade);
           
            builder.HasMany(e => e.Stores).WithOne(s => s.Item).HasForeignKey(r => r.ItemId).OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(e => e.ColorsSizes).WithOne(s => s.ItemMaster).HasForeignKey(r => r.ItemId).OnDelete(DeleteBehavior.NoAction);
           
            builder.HasOne(e => e.Category).WithMany(s => s.Items).HasForeignKey(r => r.GroupId).OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.StorePlace).WithMany(q => q.Items).HasForeignKey(r => r.DefaultStoreId).OnDelete(DeleteBehavior.Cascade);
            
            builder.Property(e => e.ItemCode).HasMaxLength(200).IsRequired();
            
            builder.Property(e => e.ArabicName).HasMaxLength(250).IsRequired();
            
            builder.Property(e => e.GroupId).IsRequired();
            
            builder.Property(e => e.TypeId).IsRequired();
            
            builder.HasIndex(a => a.NationalBarcode).IsUnique();
            
            builder.HasIndex(a => a.ItemCode).IsUnique();

        }
    }
}
