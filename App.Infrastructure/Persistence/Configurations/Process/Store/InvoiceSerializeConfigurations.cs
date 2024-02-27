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
    public class InvoiceSerializeConfigurations : IEntityTypeConfiguration<InvoiceSerialize>
    {

        public void Configure(EntityTypeBuilder<InvoiceSerialize> builder)
        {
            builder.ToTable("InvoiceSerialize");
            builder.HasKey(e => e.InvoiceSerializeId);
            //builder.HasOne(a => a.InvoiceMaster).WithMany(e => e.InvoiceSerializes)
            //  .HasForeignKey(w => w.InvoiceId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
