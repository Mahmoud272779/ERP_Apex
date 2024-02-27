using App.Domain.Models.Shared;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Infrastructure
{
    public static class stringValidation
    {
        public static bool CheckEmailFormat(string Email)
        {
            if (!Regex.IsMatch(Email, "^\\S+@\\S+\\.\\S+$"))
                return false;
            return true;
        }

    }
}
