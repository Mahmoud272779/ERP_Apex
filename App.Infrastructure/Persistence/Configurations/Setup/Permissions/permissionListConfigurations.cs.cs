using App.Domain.Entities;
using App.Domain.Entities.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Setup
{
    public class permissionListConfigurations : IEntityTypeConfiguration<permissionList>
    {
        public void Configure(EntityTypeBuilder<permissionList> builder)
        {
            builder.ToTable("permissionList");
            builder.HasKey(x => x.Id);
            
        }
    }
}
