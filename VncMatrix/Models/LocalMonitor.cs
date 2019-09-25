using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace VncMatrix.Models
{
    public class LocalMonitor : INotifyPropertyChanged
    {
        public int Number { get; set; }
        public bool CanBeOpened { get; set; } = true;

        public event PropertyChangedEventHandler? PropertyChanged;
        public void RaisePropertyChangedEvent(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public LocalMonitor(int number)
        {
            Number = number;
        }
    }
}
