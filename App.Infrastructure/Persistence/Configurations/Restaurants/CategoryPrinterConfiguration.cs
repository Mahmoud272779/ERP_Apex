using App.Domain.Entities.Process.General_Ledger;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Domain.Entities.Process.Restaurants;
using System.Reflection.Emit;

namespace App.Infrastructure.Persistence.Configurations.Restaurants
{
    public class CategoryPrinterConfiguration : IEntityTypeConfiguration<CategoriesPrinters>
    {
        public void Configure(EntityTypeBuilder<CategoriesPrinters> builder)
        {
            builder.ToTable("RstCategoriesPrinters");
            builder.HasKey(e => new { e.CategoryId, e.PrinterId });
            builder.HasOne(c => c.Category).WithMany(cp => cp.rstCategoriesPrinters).HasForeignKey(cp => cp.CategoryId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(c => c.Printer).WithMany(cp => cp.rstCategoriesPrinters).HasForeignKey(cp => cp.PrinterId).OnDelete(DeleteBehavior.Cascade);


        }
    }
}
