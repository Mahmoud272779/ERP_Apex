using App.Domain.Entities.Process.Barcode;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Process
{
    public class BarcodeTemplateConfigurations : IEntityTypeConfiguration<InvBarcodeTemplate>
    {
        public void Configure(EntityTypeBuilder<InvBarcodeTemplate> builder)
        {
            builder.ToTable("InvBarcodeTemplate");
            builder.HasKey(a => a.BarcodeId);
            builder.Property(a => a.ArabicName).IsRequired();
            builder.HasMany(a => a.BarcodeItems).WithOne(a => a.BarcodeTemplate)
                .HasForeignKey(a => a.BarcodeId).OnDelete(DeleteBehavior.NoAction);



        }

    }
}
