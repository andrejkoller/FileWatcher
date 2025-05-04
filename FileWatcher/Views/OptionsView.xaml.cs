using System.Windows.Controls;
using FileWatcher.Models;
using FileWatcher.Services;
using FileWatcher.ViewModels;

namespace FileWatcher.Views
{
    /// <summary>
    /// Interaction logic for OptionsView.xaml
    /// </summary>
    public partial class OptionsView : UserControl
    {
        public OptionsView(WatcherModel watcher, WatcherService watcherService)
        {
            InitializeComponent();
            DataContext = new OptionsViewModel(watcher, watcherService);
        }
    }
}
