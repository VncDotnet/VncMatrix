using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VncDotnet;
using VncMatrix.Models;

namespace VncMatrix.Views
{
    /// <summary>
    /// Interaktionslogik für VncWindow.xaml
    /// </summary>
    public partial class VncWindow : Window
    {
        readonly CancellationTokenSource CancelSource = new CancellationTokenSource();
        readonly VncServer Server;
        readonly VncMonitor Monitor;
        readonly int DestinationMonitor;

        public VncWindow(VncServer server, VncMonitor monitor, int destinationMonitor)
        {
            InitializeComponent();
            Server = server;
            Monitor = monitor;
            DestinationMonitor = destinationMonitor;
            Loaded += VncWindow_Loaded;
            Closed += VncWindow_Closed;
            PreviewKeyDown += VncWindow_PreviewKeyDown;
        }

        private void VncWindow_Closed(object? sender, EventArgs e)
        {
            CancelSource.Cancel();
            Vnc.Stop();
        }

        private void VncWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void VncWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DestinationMonitor < System.Windows.Forms.Screen.AllScreens.Length)
            {
                var screen = System.Windows.Forms.Screen.AllScreens[DestinationMonitor];
                var area = screen.WorkingArea;
                Top = area.Top;
                Left = area.Left;
            }
            else
            {
                MessageBox.Show($"You don't have a screen with id {DestinationMonitor}");
            }
            WindowState = WindowState.Maximized;
            Vnc.Start(Server.Address.ToString(), Monitor.Port, Server.Password, RfbConnection.SupportedSecurityTypes, Monitor.VisualOffset, CancelSource.Token);
        }
    }
}
