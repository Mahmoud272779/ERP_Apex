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
    public class UnitsConfigurations : IEntityTypeConfiguration<InvStpUnits>
    {
        public void Configure(EntityTypeBuilder<InvStpUnits> builder)
        {
            builder.ToTable("InvStpUnits");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.ArabicName).IsRequired();
            builder.HasIndex(a => a.Code).IsUnique();
            
        }
    }
}