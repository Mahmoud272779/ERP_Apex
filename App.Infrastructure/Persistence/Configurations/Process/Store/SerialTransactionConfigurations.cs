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
    public class SerialTransactionConfigurations : IEntityTypeConfiguration<InvSerialTransaction>
    {
        public void Configure(EntityTypeBuilder<InvSerialTransaction> builder)
        {
            builder.ToTable("InvSerialTransaction");
            builder.HasKey(e => e.Id);

        //    builder.HasOne(a => a.InvoicesMaster).WithMany(e => e.Serials)
         //       .HasForeignKey(w => w.InvoiceId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.Items).WithMany(e => e.Serials)
                .HasForeignKey(w => w.ItemId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.Stores).WithMany(a => a.Serials).HasForeignKey(a => a.StoreId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
