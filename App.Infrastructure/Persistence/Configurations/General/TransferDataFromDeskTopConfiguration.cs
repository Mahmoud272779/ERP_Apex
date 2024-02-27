using App.Domain.Entities.Process.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.General
{
    internal class TransferDataFromDeskTopConfiguration : IEntityTypeConfiguration<TransferDataFromDeskTop>
    {
        public void Configure(EntityTypeBuilder<TransferDataFromDeskTop> builder)
        {
            builder.ToTable("TransferDataFromDeskTop");
            builder.HasKey(e => e.Id);
        }
    }
}
