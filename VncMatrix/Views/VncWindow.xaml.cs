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
        readonly LocalMonitor DestinationMonitor;

        public VncWindow(VncServer server, VncMonitor monitor, LocalMonitor destinationMonitor)
        {
            InitializeComponent();
            Server = server;
            Monitor = monitor;
            DestinationMonitor = destinationMonitor;
            Loaded += VncWindow_Loaded;
            Closed += VncWindow_Closed;
            PreviewKeyDown += VncWindow_PreviewKeyDown;
        }

        private async void VncWindow_Closed(object? sender, EventArgs e)
        {
            CancelSource.Cancel();
            await Vnc.Stop();
        }

        private void VncWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private async void VncWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DestinationMonitor.Number < System.Windows.Forms.Screen.AllScreens.Length)
            {
                var screen = System.Windows.Forms.Screen.AllScreens[DestinationMonitor.Number];
                var area = screen.WorkingArea;
                Top = area.Top;
                Left = area.Left;
            }
            else
            {
                MessageBox.Show($"You don't have a screen with id {DestinationMonitor}");
            }
            WindowState = WindowState.Maximized;
            if (Monitor.Connection != null)
            {
                await Vnc.Attach(Monitor.Connection, Monitor.VisualOffset);
            }
        }
    }
}
