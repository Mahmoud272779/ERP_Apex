using App.Domain.Entities.Process;

namespace App.Domain.Entities
{
    public  class InvStoreBranch
    {
        public int StoreId { get; set; }
        public int BranchId { get; set; }
        public virtual InvStpStores Store { get; set; }
        public virtual GLBranch Branch { get; set; }
    }
}
