using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Windows.Input;
using System.Windows;
using FileWatcher.Models;
using FileWatcher.Services;
using Microsoft.Win32;
using FileWatcher.Helpers;

namespace FileWatcher.ViewModels
{
    public class OptionsViewModel : INotifyPropertyChanged
    {
        private readonly WatcherService _watcherService;

        public WatcherModel Watcher { get; }

        public ObservableCollection<KeyValuePair<string, string>> FileTypes { get; }

        private KeyValuePair<string, string>? _selectedFileType;
        public KeyValuePair<string, string>? SelectedFileType
        {
            get => _selectedFileType;
            set
            {
                _selectedFileType = value;
                if (value is not null)
                    Watcher.Filter = value.Value.Value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectFolderCommand { get; }
        public ICommand AddWatcherCommand { get; }

        public OptionsViewModel(WatcherModel watcher, WatcherService watcherService)
        {
            Watcher = watcher;
            _watcherService = watcherService;

            FileTypes = new ObservableCollection<KeyValuePair<string, string>>
            {
                new("All Files", "*.*"),
                new("Text Files", "*.txt"),
                new("PNG Files", "*.png"),
                new("JPG Files", "*.jpg"),
                new("GIF Files", "*.gif"),
                new("MP4 Files", "*.mp4"),
                new("AVI Files", "*.avi"),
                new("MP3 Files", "*.mp3"),
                new("WAV Files", "*.wav")
            };

            SelectFolderCommand = new RelayCommand<object>(_ => SelectFolder());
            AddWatcherCommand = new RelayCommand<object>(_ => AddWatcher());
        }

        private void SelectFolder()
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select Folder",
                InitialDirectory = Watcher.Path,
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                Watcher.Path = dialog.FolderName;
                OnPropertyChanged(nameof(Watcher));
            }
            else
            {
                MessageBox.Show("No folder selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddWatcher()
        {
            if (!Directory.Exists(Watcher.Path))
            {
                MessageBox.Show("Path does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!HasAccessRights(Watcher.Path))
            {
                MessageBox.Show("Insufficient access rights.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_watcherService.Watchers.Any(w => w.Path.Equals(Watcher.Path, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Watcher already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newWatcher = new FileSystemWatcher
            {
                Path = Watcher.Path,
                IncludeSubdirectories = Watcher.IncludeSubdirectories,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite,
                Filter = Watcher.Filter
            };

            _watcherService.AddWatcher(Watcher.Name, newWatcher);
            MessageBox.Show($"Watcher '{Watcher.Name}' added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private static bool HasAccessRights(string path)
        {
            try
            {
                var accessControl = new DirectoryInfo(path).GetAccessControl();
                var rules = accessControl.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
                return rules.OfType<FileSystemAccessRule>().Any(rule =>
                    (rule.FileSystemRights & FileSystemRights.FullControl) == FileSystemRights.FullControl);
            }
            catch
            {
                return false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
