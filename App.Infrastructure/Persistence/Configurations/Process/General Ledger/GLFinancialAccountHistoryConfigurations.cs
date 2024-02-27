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
    public class GLFinancialAccountHistoryConfigurations : IEntityTypeConfiguration<GLFinancialAccountHistory>
    {
        public void Configure(EntityTypeBuilder<GLFinancialAccountHistory> builder)
        {
            builder.ToTable("GLFinancialAccountHistory");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.ArabicName).HasColumnName("ArabicName");
            builder.Property(e => e.LatinName).HasColumnName("LatinName");
            builder.Property(e => e.employeesId).HasDefaultValue(1);
        }
    }
}