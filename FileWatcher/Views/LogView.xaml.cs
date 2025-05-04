using System.Windows.Controls;
using FileWatcher.Services;
using FileWatcher.ViewModels;

namespace FileWatcher.Views
{
    /// <summary>
    /// Interaction logic for StatusOutputView.xaml
    /// </summary>
    public partial class LogView : UserControl
    {
        public LogView(WatcherService watcherService)
        {
            InitializeComponent();
            DataContext = new LogViewModel(watcherService.Watchers);
        }
    }
}
