using System.IO;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FileWatcher.Models;

namespace FileWatcher.Views
{
    /// <summary>
    /// Interaction logic for OptionsView.xaml
    /// </summary>
    public partial class OptionsView : UserControl
    {
        private readonly WatcherOptions _options;

        public OptionsView(WatcherOptions options)
        {
            _options = options;
            InitializeComponent();
            CreateFileTypeComboBox();
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (StatusOptionsPanel is null)
            {
                return;
            }

            _options.Name = NameTextBox.Text;
            Logger.LogStatus(StatusOptionsPanel, $"Name set to: {NameTextBox.Text}", Brushes.Black);
        }

        private void WatchFolderTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (StatusOptionsPanel is null)
            {
                return;
            }

            if (Directory.Exists(WatchFolderTextBox.Text))
            {
                _options.Folder = WatchFolderTextBox.Text;
                Logger.LogStatus(StatusOptionsPanel, $"Path set to: {WatchFolderTextBox.Text}", Brushes.Black);
            }
            else
            {
                Logger.LogStatus(StatusOptionsPanel, "Invalid directory path entered.", Brushes.Red);
            }
        }
        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void IncludeSubdirectoriesCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            IncludeSubdirectoriesCheckbox.IsChecked = true;
            Logger.LogStatus(StatusOptionsPanel, "Include subdirectories option checked.", Brushes.Black);
        }

        private void IncludeSubdirectoriesCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            IncludeSubdirectoriesCheckbox.IsChecked = false;
            Logger.LogStatus(StatusOptionsPanel, "Include subdirectories option unchecked.", Brushes.Black);
        }

        private void CreateFileTypeComboBox()
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

        private void FileTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FileTypeComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is not null)
            {
                var tag = selectedItem.Tag.ToString();
                _options.SupportedExtensions = tag switch
                {
                    "*.jpg;*.png;*.gif" => [".jpg", ".png", ".gif"],
                    "*.mp4;*.avi" => [".mp4", ".avi"],
                    "*.mp3;*.wav" => [".mp3", ".wav"],
                    "*.txt" => [".txt"],
                    "*.*" => ["*"],
                    _ => Array.Empty<string>()
                };

                Logger.LogStatus(StatusOptionsPanel, $"File type set to: {selectedItem.Content}", Brushes.Black);
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

        private bool TryValidatePath()
        {
            if (!Directory.Exists(WatchFolderTextBox.Text))
            {
                Logger.LogStatus(StatusOptionsPanel, "Cannot add watcher. Invalid directory path entered.", Brushes.Red);
                return false;
            }
            else if (!HasAccessRights(WatchFolderTextBox.Text))
            {
                Logger.LogStatus(StatusOptionsPanel, "Cannot add watcher. Insufficient access rights.", Brushes.Red);
                return false;
            }
            else if (_options.Watchers.Any(w => w.Value.Path.Equals(WatchFolderTextBox.Text, StringComparison.OrdinalIgnoreCase)))
            {
                Logger.LogStatus(StatusOptionsPanel, "Cannot add watcher. A watcher for this path already exists.", Brushes.Red);
                return false;
            }
            else
            {
                return true;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (TryValidatePath())
            {
                var newWatcher = new FileSystemWatcher
                {
                    Path = _options.Folder,
                    IncludeSubdirectories = _options.IncludeSubdirectories,
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite,
                    Filter = string.Join(";", _options.SupportedExtensions)
                };

                _options.AddWatcher(_options.Name, newWatcher);

                Logger.LogStatus(StatusOptionsPanel, $"Watcher {_options.Name} for path '{WatchFolderTextBox.Text}' added successfully.");
            }
        }
    }
}
