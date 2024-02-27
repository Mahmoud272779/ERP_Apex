using App.Domain.Entities.Process;
using App.Domain.Entities.Process.General_Ledger;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.General
{
    public class GLPrinterConfiguration : IEntityTypeConfiguration<GLPrinter>
    {
        public void Configure(EntityTypeBuilder<GLPrinter> builder)
        {
            builder.ToTable("GLPrinter");
            builder.HasKey(e => e.Id);
            builder.HasOne(e => e.Branchs).WithMany(p => p.gLPrinter).HasForeignKey(k => k.BranchId);
            builder.Property(e => e.Code).HasMaxLength(20).IsRequired(true);
            builder.Property(e => e.ArabicName).IsRequired(true);
            builder.Property(e => e.IP).HasColumnType("char(15)").HasMaxLength(15).IsFixedLength(true).IsRequired(true);
            builder.Property(e => e.Notes).HasMaxLength(1000);

        }
    }
}
