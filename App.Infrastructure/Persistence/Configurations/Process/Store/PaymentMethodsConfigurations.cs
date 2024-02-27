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
   public class PaymentMethodsConfigurations : IEntityTypeConfiguration<InvPaymentMethods>
    {
        public void Configure(EntityTypeBuilder<InvPaymentMethods> builder)
        {
            builder.ToTable("InvPaymentMethods");
            builder.HasKey(a => a.PaymentMethodId);
            builder.Property(a => a.ArabicName).IsRequired();
            builder.HasOne(a => a.bank).WithMany(a => a.PaymentMethod)
                .HasForeignKey(a => a.BankId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.safe).WithMany(a => a.PaymentMethod)
                .HasForeignKey(a => a.SafeId).OnDelete(DeleteBehavior.NoAction);
            builder.HasIndex(a => a.Code).IsUnique();
        }
    }
}
