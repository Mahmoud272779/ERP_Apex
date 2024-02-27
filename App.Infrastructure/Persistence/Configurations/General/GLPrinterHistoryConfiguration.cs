using App.Domain.Entities.Process.Restaurants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Domain.Entities.Process.General_Ledger;

namespace App.Infrastructure.Persistence.Configurations.General
{
    internal class GLPrinterHistoryConfiguration : IEntityTypeConfiguration<GLPrinterHistory>
    {
        public void Configure(EntityTypeBuilder<GLPrinterHistory> builder)
        {
            builder.ToTable("GLPrinterHistory");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.employeesId).HasDefaultValue(1);

        }
    }
}
