using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Models
{
    public class UserSettings : IUserSettings
    {
        public string UserName { get; set; }
        public string LastOpenCompanyId { get; set; }
    }
}
