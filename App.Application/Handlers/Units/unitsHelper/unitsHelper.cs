using App.Domain.Entities.Setup;

namespace App.Application.Handlers.Units
{
    public static class unitsHelper
    {
        public static bool isCanDeleteUnits(IQueryable<InvStpItemCardUnit> invStpItemCardUnits, int unitId)
        {
            if (unitId == 1) return false;
            var isUsed = invStpItemCardUnits.Where(x => x.UnitId == unitId).Any();
            return !isUsed;
        }
    }
}
