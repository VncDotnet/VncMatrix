using System;
using System.Collections.Generic;
using System.Text;
using VncDotnet;

namespace VncMatrix.Models
{
    public class VncMonitor
    {
        public string? DisplayName { get; }
        public int Port { get; }
        public int[] LocalMonitors { get; }
        public MonitorSnippet? VisualOffset { get; }
        public RfbConnection? Connection { get; set; }

        public VncMonitor(string? displayName, int portOffset, int[] localMonitors, MonitorSnippet? visualOffset)
        {
            DisplayName = displayName;
            Port = portOffset;
            LocalMonitors = localMonitors;
            VisualOffset = visualOffset;
        }
    }
}
