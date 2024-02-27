using App.Domain.Entities.Notification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Persistence.Configurations.NotificationSystem
{
    public class NotificationsSeenConfiguration : IEntityTypeConfiguration<NotificationSeen>
    {
        public void Configure(EntityTypeBuilder<NotificationSeen> builder)
        {
            builder.ToTable("NotificationSeen");
            builder.HasKey(x => x.id);
            builder.HasOne(x => x.user).WithMany(x => x.NotificationSeen).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
