using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using VncDotnet;
using VncMatrix.Models;
using VncMatrix.Views;

namespace VncMatrix.ViewModels
{
    public class VncMatrixViewModel : INotifyPropertyChanged
    {
        public CancellationTokenSource CancelSource = new CancellationTokenSource();
        public VncServer[]? VncServers { get; set; } = null;
        private readonly Dictionary<int, (VncWindow window, LocalMonitor monitor)> OpenWindows = new Dictionary<int, (VncWindow, LocalMonitor)>();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void RaisePropertyChangedEvent(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public VncMatrixViewModel()
        {

        }

        public void UpdateFromConfig()
        {
            var config = JsonSerializer.Deserialize<VncMatrixConfig>(File.ReadAllText("config.json"));
            VncServers = new VncServer[config.VncServers.Length];
            for (int j = 0; j < config.VncServers.Length; j++)
            {
                var configServer = config.VncServers[j];
                var monitors = new VncMonitor[configServer.Monitors.Length];
                for (int i = 0; i < configServer.Monitors.Length; i++)
                {
                    var configMonitor = configServer.Monitors[i];
                    var localMonitors = config.Monitors.Select(cm => new LocalMonitor(cm)).ToArray();
                    monitors[i] = new VncMonitor(configMonitor.DisplayName,
                        configServer.Port + configMonitor.PortOffset,
                        localMonitors,
                        configMonitor.VisualOffset);
                }
                var server = new VncServer(configServer.DisplayName,
                    IPAddress.Parse(configServer.Address),
                    configServer.Mac,
                    configServer.Password,
                    configServer.Port,
                    monitors);
                if (server.Mac != null)
                    WakeOnLan.SendMagicPacket(server.Mac);
                if (server.Monitors != null)
                {
                    foreach (var _monitor in server.Monitors)
                    {
                        var monitor = _monitor;
                        Task.Run(() => HandleVncMonitor(server, monitor));
                    }
                }
                VncServers[j] = server;
            }
            RaisePropertyChangedEvent(nameof(VncServers));
        }

        internal void OpenMonitor(VncServer vncServer, VncMonitor vncMonitor, LocalMonitor localMonitor)
        {
            if (OpenWindows.ContainsKey(localMonitor.Number))
            {
                (VncWindow window, LocalMonitor monitor) = OpenWindows[localMonitor.Number];
                window.Close();
                monitor.CanBeOpened = true;
                monitor.RaisePropertyChangedEvent(nameof(LocalMonitor.CanBeOpened));
                OpenWindows.Remove(localMonitor.Number);
            }
            var w = new VncWindow(vncServer, vncMonitor, localMonitor);
            OpenWindows[localMonitor.Number] = (w, localMonitor);
            localMonitor.CanBeOpened = false;
            localMonitor.RaisePropertyChangedEvent(nameof(LocalMonitor.CanBeOpened));
            w.Show();
        }

        public async Task HandleVncMonitor(VncServer server, VncMonitor monitor)
        {
            while (!CancelSource.IsCancellationRequested)
            {
                try
                {
                    var rfbConn = await RfbConnection.ConnectAsync(server.Address.ToString(),
                        monitor.Port,
                        server.Password,
                        RfbConnection.SupportedSecurityTypes,
                        monitor.VisualOffset,
                        CancelSource.Token);

                    monitor.Connection = rfbConn;
                    Application.Current.Dispatcher.Invoke(() => RaisePropertyChangedEvent(nameof(VncServers)));
                    await rfbConn.Start();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"{e}\n{e.StackTrace}");
                    await Task.Delay(5000);
                }
            }
        }
    }
}
