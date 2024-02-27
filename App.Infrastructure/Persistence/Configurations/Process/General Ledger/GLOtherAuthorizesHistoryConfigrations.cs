using App.Domain.Entities.Process;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Process.General_Ledger
{
    public class GLOtherAuthorizesHistoryConfigrations: IEntityTypeConfiguration<GLOtherAuthoritiesHistory>
    {
       
            public void Configure(EntityTypeBuilder<GLOtherAuthoritiesHistory> builder)
            {
                builder.ToTable("GLOtherAuthoritiesHistory");
                builder.HasKey(e => e.Id);
                builder.Property(e => e.employeesId).HasDefaultValue(1);

            }

       
    }
}
