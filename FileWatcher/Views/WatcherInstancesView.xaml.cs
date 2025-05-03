using System.Windows.Controls;
using FileWatcher.Models;

namespace FileWatcher.Views
{
    /// <summary>
    /// Interaction logic for WatcherInstancesView.xaml
    /// </summary>
    public partial class WatcherInstancesView : UserControl
    {
        private readonly WatcherOptions _options = new();
        public WatcherInstancesView(WatcherOptions options)
        {
            _options = options;
            InitializeComponent();
        }
    }
}
