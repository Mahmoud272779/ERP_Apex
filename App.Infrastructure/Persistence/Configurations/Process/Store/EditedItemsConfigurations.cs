using App.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure
{
    public class EditedItemsConfigurations : IEntityTypeConfiguration<EditedItems>
    {
        public void Configure(EntityTypeBuilder<EditedItems> builder)
        {
            builder.ToTable("EditedItems");
            builder.HasKey(e => new { e.itemId, e.sizeId ,e.BranchID});

        }
    }
}
