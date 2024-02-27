using App.Domain.Entities.Process.Barcode;
using App.Domain.Entities.Process.Store.EInvoice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Process.Store.EInvoice
{
    public class CSIDConfiguration : IEntityTypeConfiguration<CSID>
    {
        public void Configure(EntityTypeBuilder<CSID> builder)
        {
            builder.ToTable(nameof(CSID));
            builder.HasKey(c => c.Id);
            builder.HasOne(c=> c.branch).WithMany(c=> c.CSID).HasForeignKey(c=>c.branchId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
