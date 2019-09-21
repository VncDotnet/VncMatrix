using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using VncDotnet;
using VncMatrix.Models;

namespace VncMatrix.ViewModels
{
    public class VncMatrixViewModel : INotifyPropertyChanged
    {
        public CancellationTokenSource CancelSource = new CancellationTokenSource();
        public VncServer[]? VncServers { get; set; } = null;

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
                    monitors[i] = new VncMonitor(configMonitor.DisplayName,
                        configServer.Port + configMonitor.PortOffset,
                        config.Monitors,
                        configMonitor.VisualOffset);
                }
                var server = new VncServer(configServer.DisplayName,
                    IPAddress.Parse(configServer.Address),
                    configServer.Mac,
                    configServer.Password,
                    configServer.Port,
                    monitors);
                if (server.Mac != null)
                    WakeOnLan.SendMagicPacket(server.Mac, server.Address);
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
