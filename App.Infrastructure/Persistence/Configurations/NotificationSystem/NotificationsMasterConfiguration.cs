using App.Domain.Entities.Notification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.NotificationSystem
{
    public class NotificationsMasterConfiguration : IEntityTypeConfiguration<NotificationsMaster>
    {
        public void Configure(EntityTypeBuilder<NotificationsMaster> builder)
        {
            builder.ToTable("NotificationsMaster");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.specialUser).WithMany(x => x.NotificationsMaster).HasForeignKey(x => x.specialUserId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
