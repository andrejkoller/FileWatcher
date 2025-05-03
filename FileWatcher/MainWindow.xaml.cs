using System.IO;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Path = System.IO.Path;

namespace FileWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileSystemWatcher? _watcher;
        private string[] _supportedExtensions = Array.Empty<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeFileWatcher()
        {
            try
            {
                _watcher = new FileSystemWatcher
                {
                    Path = PathTextBox.Text,
                    IncludeSubdirectories = IncludeSubdirectoriesCheckbox.IsChecked == true,
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite,
                    Filter = "*.*"
                };

                _watcher.Changed += OnFileChanged;
                _watcher.Created += OnFileChanged;
                _watcher.Deleted += OnFileChanged;
                _watcher.Renamed += OnFileRenamed;

                _watcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                LogStatus($"Error initializing watcher: {ex.Message}", Brushes.Red);
                DisposeFileWatcher();
            }
        }

        private void DisposeFileWatcher()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
                _watcher = null;
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (IsFileTypeSupported(e.FullPath, _supportedExtensions))
            {
                Dispatcher.Invoke(() =>
                {
                    var message = $"{e.ChangeType} file: {e.FullPath}";
                    LogStatus(message, Brushes.Black);
                });
            }
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            if (IsFileTypeSupported(e.FullPath, _supportedExtensions))
            {
                Dispatcher.Invoke(() =>
                {
                    var message = $"Renamed file: {e.OldFullPath} to {e.FullPath}";
                    LogStatus(message, Brushes.Black);
                });
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!TryValidatePath()) return;

            if (!HasAccessRights(PathTextBox.Text))
            {
                LogStatus("Cannot start watcher. Insufficient permissions.", Brushes.Red);
                return;
            }

            SetUIState(isWatching: true);
            if (FileTypeComboBox.Items.Count == 0)
            {
                CreateFileTypeComboBox(sender, e);
            }
            InitializeFileWatcher();

            LogStatus($"Watcher started successfully.\nListening for changes...", Brushes.Green);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            SetUIState(isWatching: false);
            DeleteFileTypeComboBoxItems(sender, e);
            DisposeFileWatcher();

            LogStatus("Watcher stopped.", Brushes.Green);
        }

        private bool TryValidatePath()
        {
            if (!Directory.Exists(PathTextBox.Text))
            {
                LogStatus("Cannot start watcher. Invalid directory path entered.", Brushes.Red);
                return false;
            }
            return true;
        }

        private void SetUIState(bool isWatching)
        {
            StartButton.IsEnabled = !isWatching;
            StopButton.IsEnabled = isWatching;
            IncludeSubdirectoriesCheckbox.IsEnabled = isWatching;
            FileTypeComboBox.IsEnabled = isWatching;
            SettingsButton.IsEnabled = !isWatching;
        }

        private void PathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (StatusPanel is null)
            {
                return;
            }

            if (Directory.Exists(PathTextBox.Text))
            {
                if (_watcher != null)
                {
                    _watcher.Path = PathTextBox.Text;
                    LogStatus($"Path set to: {PathTextBox.Text}", Brushes.Black);
                }
            }
            else
            {
                LogStatus("Invalid directory path entered.", Brushes.Red);
            }
        }

        private void IncludeSubdirectoriesCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (_watcher != null)
            {
                _watcher.IncludeSubdirectories = true;
                LogStatus("Subdirectories included.", Brushes.Black);
            }
        }

        private void IncludeSubdirectoriesCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_watcher != null)
            {
                _watcher.IncludeSubdirectories = false;
                LogStatus("Subdirectories excluded.", Brushes.Black);
            }
        }

        private void CreateFileTypeComboBox(object sender, RoutedEventArgs e)
        {
            FileTypeComboBox.Items.Add(new ComboBoxItem { Content = "All Files", Tag = "*.*" });
            FileTypeComboBox.Items.Add(new ComboBoxItem { Content = "Text Files", Tag = "*.txt" });
            FileTypeComboBox.Items.Add(new ComboBoxItem { Content = "Image Files", Tag = "*.jpg;*.png;*.gif" });
            FileTypeComboBox.Items.Add(new ComboBoxItem { Content = "Video Files", Tag = "*.mp4;*.avi" });
            FileTypeComboBox.Items.Add(new ComboBoxItem { Content = "Audio Files", Tag = "*.mp3;*.wav" });
        }

        private void DeleteFileTypeComboBoxItems(object sender, RoutedEventArgs e)
        {
            FileTypeComboBox.Items.Clear();
        }

        private static bool IsFileTypeSupported(string filePath, string[] extensions)
        {
            var fileExtension = Path.GetExtension(filePath)?.ToLowerInvariant();
            return extensions.Contains(fileExtension);
        }

        private void FileTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_watcher != null && FileTypeComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is not null)
            {
                _watcher.Filter = "*.*";
                LogStatus($"File type filter set to: {selectedItem.Content}", Brushes.Black);

                var tag = selectedItem.Tag.ToString();
                if (tag == "*.jpg;*.png;*.gif")
                {
                    _supportedExtensions = new[] { ".jpg", ".png", ".gif" };
                }
                else if (tag == "*.mp4;*.avi")
                {
                    _supportedExtensions = new[] { ".mp4", ".avi" };
                }
                else if (tag == "*.mp3;*.wav")
                {
                    _supportedExtensions = new[] { ".mp3", ".wav" };
                }
                else if (tag == "*.txt")
                {
                    _supportedExtensions = new[] { ".txt" };
                }
                else
                {
                    _supportedExtensions = Array.Empty<string>();
                }
            }
        }

        private void LogStatus(string message, Brush? foregroundColor = null)
        {
            if (StatusPanel is not null)
            {
                var textBlock = new TextBlock
                {
                    Text = $"{DateTime.Now:T} - {message}",
                    Foreground = foregroundColor ?? Brushes.Black
                };

                StatusPanel.Children.Add(textBlock);

                if (VisualTreeHelper.GetParent(StatusPanel) is ScrollViewer scrollViewer)
                {
                    scrollViewer.ScrollToEnd();
                }
            }
            else
            {
                throw new InvalidOperationException("StatusPanel is not initialized.");
            }
        }

        private static bool HasAccessRights(string path)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(path);
                var accessControl = directoryInfo.GetAccessControl();
                var rules = accessControl.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
                foreach (FileSystemAccessRule rule in rules)
                {
                    if ((rule.FileSystemRights & FileSystemRights.FullControl) == FileSystemRights.FullControl)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }

        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow
            {
                Owner = this
            };

            settingsWindow.Show();
        }
    }
}