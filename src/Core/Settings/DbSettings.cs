using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Settings
{
    public class DbSettings
    {
        public string LogConnString { get; set; }
        public string UserConnString { get; set; }
        public string SessionConnString { get; set; }
        public string CompanyConnString { get; set; }
    }
}
