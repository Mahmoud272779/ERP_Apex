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
   public class CompanyDataConfigurations : IEntityTypeConfiguration<InvCompanyData>
    {
        public void Configure(EntityTypeBuilder<InvCompanyData> builder )
        {
            builder.ToTable("InvCompanyData");
            builder.HasKey(a => a.Id);

        }
    }
}
