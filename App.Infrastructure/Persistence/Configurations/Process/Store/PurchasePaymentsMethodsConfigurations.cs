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
    public class PurchasePaymentsMethodsConfigurations : IEntityTypeConfiguration<InvoicePaymentsMethods>
    {
        public void Configure(EntityTypeBuilder<InvoicePaymentsMethods> builder)
        {
            builder.ToTable("InvoicePaymentsMethods");
            builder.HasKey(e => e.InvoicePaymentsMethodId);


            builder.HasOne(a => a.InvoicesMaster).WithMany(e => e.InvoicePaymentsMethods)
          .HasForeignKey(w => w.InvoiceId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.PaymentMethod).WithMany(e => e.InvoicesPaymentsMethods)
        .HasForeignKey(w => w.PaymentMethodId).OnDelete(DeleteBehavior.NoAction);

        }
    }
}
