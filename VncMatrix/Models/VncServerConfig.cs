using System;
using System.Collections.Generic;
using System.Text;

namespace VncMatrix.Models
{
    public class VncServerConfig
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Mac { get; set; }
        public string Password { get; set; } = string.Empty;
        public int Port { get; set; } = 5900;
        public VncMonitorConfig[] Monitors { get; set; } = new VncMonitorConfig[0];
    }
}
