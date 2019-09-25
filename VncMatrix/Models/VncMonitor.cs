using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using VncDotnet;

namespace VncMatrix.Models
{
    public class VncMonitor
    {
        public string? DisplayName { get; }
        public int Port { get; }
        public LocalMonitor[] LocalMonitors { get; }
        public MonitorSnippet? VisualOffset { get; }
        public RfbConnection? Connection { get; set; }
        public Brush Background { get; set; } = SystemColors.ControlBrush;

        public VncMonitor(string? displayName, int portOffset, LocalMonitor[] localMonitors, MonitorSnippet? visualOffset)
        {
            DisplayName = displayName;
            Port = portOffset;
            LocalMonitors = localMonitors;
            VisualOffset = visualOffset;
        }
    }
}
