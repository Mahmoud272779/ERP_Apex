using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Setup
{
    public class UserAndPermissionConfigurations : IEntityTypeConfiguration<UserAndPermission>
    {
        public void Configure(EntityTypeBuilder<UserAndPermission> builder)
        {
            builder.ToTable("UserAndPermission");
            builder.HasKey(x => x.Id);
        }
    }
}
