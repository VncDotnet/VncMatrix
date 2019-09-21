using System;
using System.Collections.Generic;
using System.Text;
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
using VncMatrix.Views;

namespace VncMatrix.Controls
{
    /// <summary>
    /// Interaktionslogik für VncServerControl.xaml
    /// </summary>
    public partial class VncServerControl : UserControl
    {
        public VncServerControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button))
            {
                MessageBox.Show($"Invalid Sender {sender}");
                return;
            }

            if (!(button.DataContext is int localMonitor))
            {
                MessageBox.Show($"Invalid Button DC {button.DataContext}");
                return;
            }

            var parent = button.ParentDataContext();
            if (!(parent is (FrameworkElement parentElement, VncMonitor vncMonitor)))
            {
                MessageBox.Show($"Invalid Parent DC {parent}");
                return;
            }
            parent = parentElement.ParentDataContext();
            if (!(parent is (FrameworkElement _, VncServer vncServer)))
            {
                MessageBox.Show($"Invalid Parent DC {parent}");
                return;
            }
            new VncWindow(vncServer, vncMonitor, localMonitor).Show();
        }
    }
}
