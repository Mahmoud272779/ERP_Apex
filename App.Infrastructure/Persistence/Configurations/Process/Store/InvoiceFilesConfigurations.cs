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
    public class InvoiceFilesConfigurations : IEntityTypeConfiguration<InvoiceFiles>
    {
        public void Configure(EntityTypeBuilder<InvoiceFiles> builder)
        {
            builder.ToTable("InvoiceFiles");
            builder.HasKey(e => e.InvoiceFileId);
           

            builder.HasOne(a => a.InvoicesMaster).WithMany(e => e.InvoiceFiles)
          .HasForeignKey(w => w.InvoiceId).OnDelete(DeleteBehavior.NoAction);

        }
    }
}
