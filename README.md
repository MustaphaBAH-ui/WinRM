# WinRM

A lightweight real-time system resource monitor desktop app built with C# and WPF. Displays live CPU, memory, disk, and network usage with animated sparkline charts — all updating every second.

![.NET 8](https://img.shields.io/badge/.NET-8.0--windows-512BD4?style=flat-square)
![WPF](https://img.shields.io/badge/UI-WPF-blue?style=flat-square)
![Platform](https://img.shields.io/badge/platform-Windows-blue?style=flat-square)
![License](https://img.shields.io/badge/license-MIT-green?style=flat-square)
[![Latest Release](https://img.shields.io/github/v/release/your-username/ResourceMonitor?style=flat-square)](https://github.com/your-username/ResourceMonitor/releases/latest)

---

## Features

- Live **CPU utilisation** percentage
- Live **RAM usage** in GB and percentage
- Live **Disk read/write** speed in MB/s
- Live **Network sent/received** speed in KB/s
- **Sparkline charts** showing the last 60 seconds of history for every metric
- Updates every **1 second** using a `DispatcherTimer`
- Clean dark UI with no external dependencies

---

## Screenshot

> *(Add a screenshot here — press Win + Shift + S, capture the app window, and upload it to the repo as `screenshot.png`)*

---

## Installation

### Download (Windows x64)

No .NET installation required — self-contained executable.

1. Go to the [Releases page](https://github.com/your-username/ResourceMonitor/releases/latest)
2. Download `WinRM.exe`
3. Double-click to run — no installation needed

### Build from source

**Requirements**: [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) and Windows

```bash
git clone https://github.com/your-username/ResourceMonitor.git
cd ResourceMonitor
dotnet build
```

To run directly:
```bash
dotnet run
```

To publish as a self-contained single `.exe`:
```bash
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```
Output will be at `bin\Release\net8.0-windows\win-x64\publish\ResourceMonitor.exe`.

---

## Project Structure

```
ResourceMonitor/
├── MainWindow.xaml          # UI layout — four metric cards with sparklines
├── MainWindow.xaml.cs       # Code-behind (minimal)
├── MainViewModel.cs         # DispatcherTimer, ObservableCollections, data binding
├── SystemMetrics.cs         # Windows API calls — PerformanceCounter, NetworkInterface
├── SparklineConverter.cs    # IValueConverter — history queue → Polyline points
├── RelayCommand.cs          # ICommand helper for WPF bindings
└── ResourceMonitor.csproj
```

### How it works

| Layer | File | Responsibility |
|---|---|---|
| Data | `SystemMetrics.cs` | Reads raw values from `PerformanceCounter`, `NetworkInterface`, `DriveInfo` |
| ViewModel | `MainViewModel.cs` | Polls every 1s, maintains 60-point history queues, notifies UI via `INotifyPropertyChanged` |
| Converter | `SparklineConverter.cs` | Converts `ObservableCollection<float>` to a WPF `PointCollection` for `Polyline` |
| UI | `MainWindow.xaml` | Four cards bound to the ViewModel, sparklines bound through the converter |

---

## Requirements

- Windows 10 or later
- .NET 8.0 SDK (build only — not needed if using the precompiled release)

---

## License

MIT — see [LICENSE](LICENSE) for details.
