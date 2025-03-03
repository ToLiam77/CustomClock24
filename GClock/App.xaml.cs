using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Windows;

namespace GClock
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public ObservableCollection<Alarm> Alarms { get; set; } = new ObservableCollection<Alarm>();
    }

}
