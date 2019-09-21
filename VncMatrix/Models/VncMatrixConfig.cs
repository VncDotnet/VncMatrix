using System;
using System.Collections.Generic;
using System.Text;

namespace VncMatrix.Models
{
    public class VncMatrixConfig
    {
        public VncServerConfig[] VncServers { get; set; } = new VncServerConfig[0];
        public int[] Monitors { get; set; } = new int[0];
    }
}
