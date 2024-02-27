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
    class BarcodeItemsConfigurations : IEntityTypeConfiguration<InvBarcodeItems>
    {
        public void Configure(EntityTypeBuilder<InvBarcodeItems> builder)
        {
            builder.ToTable("InvBarcodeItems");
            builder.HasKey(a => a.Id);


        }

    }
}
