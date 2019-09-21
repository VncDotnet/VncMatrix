using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace VncMatrix.Models
{
    public class VncServer
    {
        public string DisplayName { get;  }
        public IPAddress Address { get; }
        public string? Mac { get;  }
        public string? Password { get; }
        public int Port { get; }
        public VncMonitor[]? Monitors { get; }

        public VncServer(string displayName, IPAddress address, string? mac, string? password, int port, VncMonitor[] monitors)
        {
            DisplayName = displayName;
            Address = address;
            Mac = mac;
            Password = password;
            Port = port;
            Monitors = monitors;
        }
    }
}
