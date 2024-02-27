using App.Domain.Entities.POS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.Process.POS
{
    public class POSSessionConfigurations : IEntityTypeConfiguration<POSSession>
    {
        public void Configure(EntityTypeBuilder<POSSession> builder)
        {
            builder.ToTable("POSSession");
            builder.HasKey(x => x.Id);
            builder.HasIndex(a => a.sessionCode).IsUnique();
            builder.HasOne(x=> x.employee).WithMany(x=> x.pOSSessionsStart).HasForeignKey(x=> x.employeeId);
            builder.HasOne(x=> x.employeeCloseSeassion).WithMany(x=> x.pOSSessionsEnd).HasForeignKey(x => x.sessionClosedById);
        }
    }
}
