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
  public  class Discount_A_P_Configurations : IEntityTypeConfiguration<InvDiscount_A_P>
    {
        public void Configure(EntityTypeBuilder<InvDiscount_A_P> builder)
        {
            builder.ToTable("InvDiscount_A_P");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Amount).IsRequired();
            builder.Property(a => a.PersonId).IsRequired();
            builder.Property(a => a.DocDate).IsRequired();
            builder.HasOne(a => a.Person)
                .WithMany(a => a.Discount_A_P)
                .HasForeignKey(p => p.PersonId).OnDelete(DeleteBehavior.NoAction);

        }
    }
}
