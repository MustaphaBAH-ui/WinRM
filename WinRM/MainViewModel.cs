using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace ResourceMonitor;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly SystemMetrics _metrics = new();
    private readonly DispatcherTimer _timer;
    private const int HistorySize = 60;

    // --- CPU ---
    public ObservableCollection<float> CpuHistory { get; } = [];
    private float _cpuPercent;
    public float CpuPercent { get => _cpuPercent; set => Set(ref _cpuPercent, value); }

    // --- RAM ---
    public ObservableCollection<float> RamHistory { get; } = [];
    private float _ramPercent;
    private float _ramUsedGb;
    private float _ramTotalGb;
    public float RamPercent { get => _ramPercent; set => Set(ref _ramPercent, value); }
    public float RamUsedGb { get => _ramUsedGb; set => Set(ref _ramUsedGb, value); }
    public float RamTotalGb { get => _ramTotalGb; set => Set(ref _ramTotalGb, value); }

    // --- Disk ---
    public ObservableCollection<float> DiskReadHistory { get; } = [];
    public ObservableCollection<float> DiskWriteHistory { get; } = [];
    private float _diskReadMbs;
    private float _diskWriteMbs;
    public float DiskReadMbs { get => _diskReadMbs; set => Set(ref _diskReadMbs, value); }
    public float DiskWriteMbs { get => _diskWriteMbs; set => Set(ref _diskWriteMbs, value); }

    // --- Network ---
    public ObservableCollection<float> NetSentHistory { get; } = [];
    public ObservableCollection<float> NetRecvHistory { get; } = [];
    private float _netSentKbs;
    private float _netRecvKbs;
    public float NetSentKbs { get => _netSentKbs; set => Set(ref _netSentKbs, value); }
    public float NetRecvKbs { get => _netRecvKbs; set => Set(ref _netRecvKbs, value); }

    public MainViewModel()
    {
        _metrics.Initialise();

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += OnTick;
        _timer.Start();
    }

    private void OnTick(object? sender, EventArgs e)
    {
        // CPU
        CpuPercent = _metrics.GetCpuPercent();
        Push(CpuHistory, CpuPercent);

        // RAM
        var (usedGb, totalGb, ramPct) = _metrics.GetRam();
        RamUsedGb = usedGb;
        RamTotalGb = totalGb;
        RamPercent = ramPct;
        Push(RamHistory, ramPct);

        // Disk
        var (readMbs, writeMbs) = _metrics.GetDisk();
        DiskReadMbs = readMbs;
        DiskWriteMbs = writeMbs;
        Push(DiskReadHistory, readMbs);
        Push(DiskWriteHistory, writeMbs);

        // Network
        var (sentKbs, recvKbs) = _metrics.GetNetwork();
        NetSentKbs = sentKbs;
        NetRecvKbs = recvKbs;
        Push(NetSentHistory, sentKbs);
        Push(NetRecvHistory, recvKbs);
    }

    private static void Push(ObservableCollection<float> col, float value)
    {
        col.Add(value);
        if (col.Count > HistorySize)
            col.RemoveAt(0);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}