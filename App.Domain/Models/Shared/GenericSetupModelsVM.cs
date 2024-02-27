using System.Collections.Generic;

namespace App.Domain.Models.Shared
{
   public  class GenericSetupModelsVM
    {
      public SetupVM SetupVM { get; set; }
    }
     
    public class GenericSetupList
    {
        public List<SetupVM> SetupVM { get; set; }
    }
}
