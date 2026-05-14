## Short description

A lightweight and efficient .NET and WPF application that monitors file system changes in real time. FileWatcher automatically detects created, modified, deleted, and renamed files within a selected directory and logs all events in a clean and structured way.

## ✨ Features

- Real-time File Monitoring – Automatically detects changes in the file system
- Create / Update / Delete / Rename Detection – Logs all event types instantly
- Clean Logging Output – View detailed, timestamped system events
- Custom Directory Selection – Monitor any folder of your choice
- Error Handling – Graceful handling of unexpected file system exceptions

## 🛠️ Technologies Used

- .NET / C# – Core application logic
- Visual Studio / .NET CLI – Development and build tools
- WPF and XAML

## 📋 Prerequisites

- .NET SDK 8.0 or higher
- Windows, macOS, or Linux environment (FileSystemWatcher works cross-platform)

## 📦 Installation

1. Clone the repository:

```bash
git clone https://github.com/andrejkoller/FileWatcher.git
cd FileWatcher
```

2. Restore dependencies:

```bash
dotnet restore
```

3. Build the project:

```bash
dotnet build
```

4. Run the application:

```env
dotnet run
```
