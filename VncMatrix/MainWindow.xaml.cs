using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VncMatrix.Models;
using VncMatrix.ViewModels;
using VncMatrix.Views;
using VncDotnet.WPF;

namespace VncMatrix
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.Initialized += MainWindow_Initialized;
            InitializeComponent();
        }

        private void MainWindow_Initialized(object? sender, EventArgs eventArgs)
        {
            try
            {
                if (DataContext is VncMatrixViewModel vm)
                {
                    vm.PropertyChanged += Vm_PropertyChanged;
                    vm.UpdateFromConfig();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e}");
            }
        }

        private async void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            foreach (VncDotnetControl dotnetVnc in Utils.FindVisualChildren<VncDotnetControl>(this))
            {
                var monitor = (VncMonitor)dotnetVnc.DataContext;
                if (monitor.Connection != dotnetVnc.PreEstablishedConnection && monitor.Connection != null)
                {
                    Debug.WriteLine($"Vm_PropertyChanged {monitor.Port} ({monitor.DisplayName}) {dotnetVnc.GetHashCode()}");
                    await dotnetVnc.Stop();
                    await dotnetVnc.Attach(monitor.Connection, monitor.VisualOffset);
                }
            }
        }

        private void MonitorSelected(VncServer vncServer, VncMonitor vncMonitor, LocalMonitor localMonitor)
        {
            try
            {
                if (DataContext is VncMatrixViewModel vm)
                {
                    vm.OpenMonitor(vncServer, vncMonitor, localMonitor);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e}");
            }
        }
    }
}
