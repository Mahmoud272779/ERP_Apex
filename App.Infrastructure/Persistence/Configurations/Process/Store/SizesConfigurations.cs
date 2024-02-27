﻿using App.Domain.Entities.Process;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Process
{
   public class SizesConfigurations : IEntityTypeConfiguration<InvSizes>
    {
        public void Configure(EntityTypeBuilder<InvSizes> builder)
        {
            builder.ToTable("InvSizes");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.ArabicName).IsRequired();
            builder.HasIndex(a => a.Code).IsUnique();
        }
    }
}