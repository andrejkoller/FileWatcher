using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using FileWatcher.Helpers;
using FileWatcher.Models;
using FileWatcher.Services;

namespace FileWatcher.ViewModels
{
    public class WatcherInstancesViewModel : INotifyPropertyChanged
    {
        private readonly WatcherService _watcherService;

        public ObservableCollection<WatcherModel> Watchers { get; }

        public ICommand RemoveWatcherCommand { get; }

        public WatcherInstancesViewModel(WatcherService watcherService)
        {
            _watcherService = watcherService;
            Watchers = new ObservableCollection<WatcherModel>(_watcherService.Watchers);
            RemoveWatcherCommand = new RelayCommand<WatcherModel>(RemoveWatcher);
        }
        private void RemoveWatcher(WatcherModel? watcher)
        {
            if (watcher == null)
            {
                MessageBox.Show("No watcher selected.", "Error");
                return;
            }

            _watcherService.RemoveWatcher(watcher.Name);
            if (Watchers.Remove(watcher))
            {
                MessageBox.Show($"Watcher '{watcher.Name}' removed successfully.", "Success");
            }
            else
            {
                MessageBox.Show($"Watcher '{watcher.Name}' not found in the list.", "Error");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
