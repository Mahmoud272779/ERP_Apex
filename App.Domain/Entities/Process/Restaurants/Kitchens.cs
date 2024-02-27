using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process.Restaurants
{
    public class Kitchens
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string LatinName { get; set; } = "";
        public string ArabicName { get; set; } = "";
        public string Notes { get; set; } = "";
        public int Status { get; set; } = 1;
        public bool CanDelete { get; set; } = true;
        public DateTime UTime { get; set; }
        public ICollection<InvCategories> Categories { get; set; }

    }
}
