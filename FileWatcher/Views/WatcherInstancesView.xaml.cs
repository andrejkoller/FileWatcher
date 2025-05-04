using System.Windows.Controls;
using FileWatcher.Services;
using FileWatcher.ViewModels;

namespace FileWatcher.Views
{
    /// <summary>  
    /// Interaction logic for WatcherInstancesView.xaml  
    /// </summary>  
    public partial class WatcherInstancesView : UserControl
    {
        public WatcherInstancesView(WatcherService watcherService)
        {
            InitializeComponent();
            DataContext = new WatcherInstancesViewModel(watcherService);
        }
    }
}
