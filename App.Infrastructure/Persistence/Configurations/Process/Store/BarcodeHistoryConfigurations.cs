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
    public class BarcodeHistoryConfigurations : IEntityTypeConfiguration<InvBarcodeHistory>
    {
        public void Configure(EntityTypeBuilder<InvBarcodeHistory> builder)
        {
            builder.ToTable("InvBarcodeHistory");
            builder.HasKey(a => a.Id);
            builder.Property(e => e.employeesId).HasDefaultValue(1);

        }

    }
}
