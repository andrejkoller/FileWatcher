using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
                StatusText.Text = $"Error initializing watcher: {ex.Message}";
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
            if (isFileTypeSupported(e.FullPath, _supportedExtensions))
            {
                Dispatcher.Invoke(() =>
                {
                    var message = $"{e.ChangeType} file: {e.FullPath}\n";
                    StatusText.Text += message;
                });
            }
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            if (isFileTypeSupported(e.FullPath, _supportedExtensions))
            {
                Dispatcher.Invoke(() =>
                {
                    var message = $"Renamed file: {e.OldFullPath} to {e.FullPath}\n";
                    StatusText.Text += message;
                });
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(PathTextBox.Text))
            {
                StatusText.Text += "Cannot start watcher. Invalid directory path.\n";
                return;
            }

            CreateFileTypeComboBox(sender, e);
            InitializeFileWatcher();
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            IncludeSubdirectoriesCheckbox.IsEnabled = true;
            FileTypeComboBox.IsEnabled = true;
            StatusText.Text += "Watcher started.\n";
            StatusText.Text += "Listening for changes...\n";
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFileTypeComboBoxItems(sender, e);
            DisposeFileWatcher();
            StopButton.IsEnabled = false;
            StartButton.IsEnabled = true;
            IncludeSubdirectoriesCheckbox.IsEnabled = false;
            FileTypeComboBox.IsEnabled = false;
            StatusText.Text += "Watcher stopped.\n";
        }

        private void PathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PathTextBox.Text))
            {
                StatusText.Text += "Please enter a valid path.\n";
            }
            else if (!Directory.Exists(PathTextBox.Text) && StatusText != null)
            {
                StatusText.Text += "Invalid directory path.\n";
            }

            if (_watcher != null && StatusText != null)
            {
                _watcher.Path = PathTextBox.Text;
                StatusText.Text += $"Path changed to: {PathTextBox.Text}\n";
            }
        }

        private void IncludeSubdirectoriesCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (_watcher != null)
            {
                _watcher.IncludeSubdirectories = true;
                StatusText.Text += "Watching subdirectories.\n";
            }
        }

        private void IncludeSubdirectoriesCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_watcher != null)
            {
                _watcher.IncludeSubdirectories = false;
                StatusText.Text += "Not watching subdirectories.\n";
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

        private bool isFileTypeSupported(string filePath, string[] extensions)
        {
            var fileExtension = Path.GetExtension(filePath)?.ToLowerInvariant();
            return extensions.Contains(fileExtension);
        }

        private void FileTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_watcher != null && FileTypeComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is not null)
            {
                _watcher.Filter = selectedItem.Tag.ToString()!;
                StatusText.Text += $"File type filter set to: {selectedItem.Content}\n";

                if (_watcher.Filter == "*.jpg;*.png;*.gif")
                {
                    _supportedExtensions = new[] { ".jpg", ".png", ".gif" };
                }
                else if (_watcher.Filter == "*.mp4;*.avi")
                {
                    _supportedExtensions = new[] { ".mp4", ".avi" };
                }
                else if (_watcher.Filter == "*.mp3;*.wav")
                {
                    _supportedExtensions = new[] { ".mp3", ".wav" };
                }
                else if (_watcher.Filter == "*.txt")
                {
                    _supportedExtensions = new[] { ".txt" };
                }
                else
                {
                    _supportedExtensions = Array.Empty<string>();
                }
            }
        }
    }
}