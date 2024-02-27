using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Entities.Process.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Entities.Chat.chat;

namespace App.Infrastructure.Persistence.Configurations.Process
{
    public class chatGroupsConfiguration : IEntityTypeConfiguration<chatGroups>
    {
        public void Configure(EntityTypeBuilder<chatGroups> builder)
        {
            builder.ToTable("chatGroups");
            builder.HasKey(e => e.Id);
            builder.HasOne(x => x.groupCreator).WithMany(x => x.chatGroups).HasForeignKey(x => x.groupCreatorId).OnDelete(DeleteBehavior.NoAction);
        }
    }
    public class chatGroupMembersConfiguration : IEntityTypeConfiguration<chatGroupMembers>
    {
        public void Configure(EntityTypeBuilder<chatGroupMembers> builder)
        {
            builder.ToTable("chatGroupMembers");
            builder.HasKey(e => e.Id);
            builder.HasOne(x => x.invEmployeeMember).WithMany(x => x.chatGroupMembers).HasForeignKey(x => x.memberId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.group).WithMany(x => x.chatGroupMembers).HasForeignKey(x => x.groupId).OnDelete(DeleteBehavior.NoAction);
        }
    }
    public class chatMessagesMembersConfiguration : IEntityTypeConfiguration<chatMessages>
    {
        public void Configure(EntityTypeBuilder<chatMessages> builder)
        {
            builder.ToTable("chatMessages");
            builder.HasKey(e => e.Id);
            builder.HasOne(x => x.InvEmployeesFrom).WithMany(x => x.chatMessagesFrom).HasForeignKey(x => x.fromId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.InvEmployeesTo).WithMany(x => x.chatMessagesTo).HasForeignKey(x => x.toId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.group).WithMany(x => x.chatMessages).HasForeignKey(x => x.groupId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
