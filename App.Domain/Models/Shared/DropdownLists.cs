using System.Collections.Generic;

namespace App.Domain.Models.Shared
{
    public class DropdownLists
    {

        public virtual IReadOnlyCollection<LookUps> Units { get; set; }
        
    }
    public enum DropDownSelection
    {
        Units,
    }

}
