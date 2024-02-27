
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class GetUserDataResponse
    {
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FULL_NAME_ARABIC { get; set; }
        public string FULL_NAME_ENGLISH { get; set; }
        public string SHORT_NAME_ARABIC { get; set; }
        public string SHORT_NAME_ENGLISH { get; set; }
        public string BRANCH_CODE { get; set; }

        public string Token { get; set; }

        public int BRANCH_ID { get; set; }
        public int ModuleId { get; set; }
        public byte[] Logo { get; set; }
        public string BranchArabicName { get; set; }
        public string BranchLatinName { get; set; }
        public bool IsUseMenu { get; set; }
        public string MenuURL { get; set; }
    }
}
