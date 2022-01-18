using System.ComponentModel;

namespace TacticalSimWpf.ViewModel
{
    public class Changeable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void Changed(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}