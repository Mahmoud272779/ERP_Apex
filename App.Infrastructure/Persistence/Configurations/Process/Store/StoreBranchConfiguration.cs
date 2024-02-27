using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace App.Infrastructure
{
    public class StoreBranchConfiguration : IEntityTypeConfiguration<InvStoreBranch>
    {
        public void Configure(EntityTypeBuilder<InvStoreBranch> builder)
        {
            builder.ToTable("InvStoreBranch");
            builder.HasKey(e => new { e.StoreId, e.BranchId });
            builder.HasOne(storeBranch => storeBranch.Store)
                .WithMany(store => store.StoreBranches)
                .HasForeignKey(storeBranch => storeBranch.StoreId);
            builder.HasOne(storeBranch => storeBranch.Branch)
                .WithMany(branch => branch.StoreBranches)
                .HasForeignKey(storeBranch => storeBranch.BranchId);

        }
    }
}
