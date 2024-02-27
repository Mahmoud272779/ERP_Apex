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
    class GLJournalEntryDetailsConfigurations : IEntityTypeConfiguration<GLJournalEntryDetails>
    {
        public void Configure(EntityTypeBuilder<GLJournalEntryDetails> builder)
        {
            builder.ToTable("GLJournalEntryDetails");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.DescriptionAr).HasColumnName("DescriptionAr");
            builder.Property(e => e.DescriptionEn).HasColumnName("DescriptionEn");

            builder.HasOne(e => e.journalEntry)
                   .WithMany(d => d.JournalEntryDetails)
                   .HasForeignKey(q=>q.JournalEntryId);

            builder.HasOne(x => x.GLFinancialAccount)
                .WithMany(x => x.GLJournalEntryDetails)
                .HasForeignKey(x => x.FinancialAccountId);

        }
    }
}
