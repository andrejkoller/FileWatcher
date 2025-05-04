using System.IO;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FileWatcher.Models;
using FileWatcher.Services;

namespace FileWatcher.Views
{
    /// <summary>
    /// Interaction logic for OptionsView.xaml
    /// </summary>
    public partial class OptionsView : UserControl
    {
        private readonly Watcher _watcher;
        private WatcherManager _watcherManager = new();

        public OptionsView(Watcher watcher, WatcherManager watcherManager)
        {
            _watcherManager = watcherManager;
            _watcher = watcher;
            InitializeComponent();
            CreateFileTypeComboBox();
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _watcher.Name = NameTextBox.Text;
        }

        private void WatchFolderTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _watcher.Path = WatchFolderTextBox.Text;
        }
        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void IncludeSubdirectoriesCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            IncludeSubdirectoriesCheckbox.IsChecked = _watcher.IncludeSubdirectories = true;
        }

        private void IncludeSubdirectoriesCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            IncludeSubdirectoriesCheckbox.IsChecked = _watcher.IncludeSubdirectories = false;
        }

        private void CreateFileTypeComboBox()
        {
            FileTypeComboBox.Items.Add(new ComboBoxItem { Content = "All Files", Tag = "*.*" });
            FileTypeComboBox.Items.Add(new ComboBoxItem { Content = "Text Files", Tag = "*.txt" });
            FileTypeComboBox.Items.Add(new ComboBoxItem { Content = "PNG Files", Tag = "*.png" });
            FileTypeComboBox.Items.Add(new ComboBoxItem { Content = "JPG Files", Tag = "*.jpg" });
            FileTypeComboBox.Items.Add(new ComboBoxItem { Content = "GIF Files", Tag = "*.gif" });
            FileTypeComboBox.Items.Add(new ComboBoxItem { Content = "MP4 Files", Tag = "*.mp4" });
            FileTypeComboBox.Items.Add(new ComboBoxItem { Content = "AVI Files", Tag = "*.avi" });
            FileTypeComboBox.Items.Add(new ComboBoxItem { Content = "MP3 Files", Tag = "*.mp3" });
            FileTypeComboBox.Items.Add(new ComboBoxItem { Content = "WAV Files", Tag = "*.wav" });
        }

        private void DeleteFileTypeComboBoxItems(object sender, RoutedEventArgs e)
        {
            FileTypeComboBox.Items.Clear();
        }

        private void FileTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FileTypeComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is not null)
            {
                _watcher.Filter = selectedItem.Tag.ToString() ?? string.Empty;
            }
            else
            {
                MessageBox.Show("Please select a file type from the list.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Cannot add watcher. The specified path does not exist.\n", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (!HasAccessRights(WatchFolderTextBox.Text))
            {
                MessageBox.Show("Cannot add watcher. Insufficient access rights.\n", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (_watcherManager.Watchers.Any(w => w.Path.Equals(WatchFolderTextBox.Text, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Cannot add watcher. A watcher for this path already exists.\n", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    Path = _watcher.Path,
                    IncludeSubdirectories = _watcher.IncludeSubdirectories,
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite,
                    Filter = _watcher.Filter
                };

                _watcherManager.AddWatcher(_watcher.Name, newWatcher);

                MessageBox.Show($"Watcher '{_watcher.Name}' added successfully.\n", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
