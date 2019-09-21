using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using VncDotnet;

namespace VncMatrix.Models
{
    public class VncMonitorConfig
    {
        public string? DisplayName { get; set; }
        public int PortOffset { get; set; }
        public MonitorSnippet? VisualOffset { get; set; }
    }
}
