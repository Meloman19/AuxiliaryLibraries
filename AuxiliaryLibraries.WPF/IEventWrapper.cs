using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuxiliaryLibrary.WPF
{
    public interface IEventWrapper : INotifyPropertyChanged
    {
        void OnPropertyChanged(object sender, PropertyChangedEventArgs e);
    }
}