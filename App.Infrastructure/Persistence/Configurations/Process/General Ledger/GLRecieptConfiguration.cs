using App.Domain.Entities;
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
    public class GLRecieptsConfiguration : IEntityTypeConfiguration<GlReciepts>
    {
        public void Configure(EntityTypeBuilder<GlReciepts> builder)
        {
            builder.ToTable("GlReciepts");
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.FinancialAccount)
                   .WithMany(q => q.reciepts)
                   .HasForeignKey(q => new { q.FinancialAccountId })
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.Safes)
                .WithMany(q => q.reciept)
                .HasForeignKey(q => new { q.SafeID })
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.Banks)
                .WithMany(q => q.reciept)
                .HasForeignKey(q => new { q.BankId })
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.person)
               .WithMany(q => q.reciept)
               .HasForeignKey(q => new { q.PersonId })
               .OnDelete(DeleteBehavior.NoAction);
   
            builder.HasOne(e => e.PaymentMethods)
                .WithMany(q => q.Receipts)
                .HasForeignKey(q => new { q.PaymentMethodId })
                .OnDelete(DeleteBehavior.NoAction); 
            
            //Authority
            builder.HasOne(e => e.SalesMan)
               .WithMany(q => q.reciept)
               .HasForeignKey(q => new { q.SalesManId })
               .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.OtherAuthorities)
               .WithMany(q => q.reciept)
               .HasForeignKey(q => new { q.OtherAuthorityId })
               .OnDelete(DeleteBehavior.NoAction);


            builder.HasIndex(a =>new { a.RectypeWithPayment , a.CollectionCode , a.BranchId}).IsUnique();

        }
    }
}
