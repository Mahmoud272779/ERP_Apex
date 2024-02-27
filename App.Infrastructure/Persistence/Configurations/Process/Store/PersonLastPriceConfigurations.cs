using App.Domain.Entities.Process;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Domain.Entities.Process.Store;

namespace App.Infrastructure.Persistence.Configurations.Process.Store
{
    public  class PersonLastPriceConfigurations : IEntityTypeConfiguration<InvPersonLastPrice>
    {
        public void Configure(EntityTypeBuilder<InvPersonLastPrice> builder)
        {
            builder.ToTable("InvPersonLastPrice");
            builder.HasKey(e => e.id);

        }
    }
}
