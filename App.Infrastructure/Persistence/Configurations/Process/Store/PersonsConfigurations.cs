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
  public  class PersonsConfigurations : IEntityTypeConfiguration<InvPersons>
    {
        public void Configure(EntityTypeBuilder<InvPersons> builder)
        {
            builder.ToTable("InvPersons");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.ArabicName).IsRequired();
            //builder.HasIndex(a => a.Email).IsUnique();
            //builder.HasIndex(a => a.Fax).IsUnique();
            //builder.HasIndex(a => a.Phone).IsUnique();
        }
    }
}
