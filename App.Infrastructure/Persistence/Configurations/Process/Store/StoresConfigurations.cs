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
   public  class StoresConfigurations : IEntityTypeConfiguration<InvStpStores>
    {
        public void Configure(EntityTypeBuilder<InvStpStores> builder)
        {
           
            builder.ToTable("InvStpStores");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.ArabicName).IsRequired();
            //builder.HasOne(e => e.Branch)
            //    .WithMany(s => s.Stores)
            //    .HasForeignKey(r => r.BranchId);
            builder.HasIndex(a => a.Code).IsUnique();


        }
    }
}
