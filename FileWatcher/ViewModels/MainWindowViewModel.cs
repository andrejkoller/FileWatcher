using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using FileWatcher.Helpers;
using FileWatcher.Models;
using FileWatcher.Services;
using FileWatcher.Views;

namespace FileWatcher.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private UserControl? _currentView;
        public UserControl? CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public ICommand ShowOptionsCommand { get; }
        public ICommand ShowWatchersCommand { get; }
        public ICommand ShowLogCommand { get; }

        public MainWindowViewModel(WatcherModel watcher, WatcherService watcherService)
        {
            ShowOptionsCommand = new RelayCommand<object>(_ => CurrentView = new OptionsView(watcher, watcherService));
            ShowWatchersCommand = new RelayCommand<object>(_ => CurrentView = new WatcherInstancesView(watcherService));
            ShowLogCommand = new RelayCommand<object>(_ => CurrentView = new LogView(watcherService));

            CurrentView = new OptionsView(watcher, watcherService);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
