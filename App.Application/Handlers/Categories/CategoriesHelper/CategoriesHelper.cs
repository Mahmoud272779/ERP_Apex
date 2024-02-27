using App.Domain.Entities.Setup;

namespace App.Application.Handlers.Categories
{
    public static class CategoriesHelper
    {
        public static bool isCanDeleteCategory(IQueryable<InvStpItemCardMaster> invStpItemCardMasters, int catId)
        {
            if (catId == 1) return false;
            var isUsed = !invStpItemCardMasters.Where(x => x.GroupId == catId).Any();
            return isUsed;
        }
    }
}
