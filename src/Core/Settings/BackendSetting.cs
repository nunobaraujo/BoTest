using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Settings
{
    public class BackendSetting
    {
        public string ApiKey { get; set; }
        public int SocketPort { get; set; }
        public DbSettings Db { get; set; }

    }
}
