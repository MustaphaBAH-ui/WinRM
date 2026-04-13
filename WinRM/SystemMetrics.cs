using System.Diagnostics;
using System.Net.NetworkInformation;

namespace ResourceMonitor;

public class SystemMetrics
{
    private readonly PerformanceCounter _cpu =
        new("Processor", "% Processor Time", "_Total");

    private readonly PerformanceCounter _diskRead =
        new("PhysicalDisk", "Disk Read Bytes/sec", "_Total");

    private readonly PerformanceCounter _diskWrite =
        new("PhysicalDisk", "Disk Write Bytes/sec", "_Total");

    private long _lastBytesSent;
    private long _lastBytesReceived;

    // Call once on startup so the first real read is accurate
    public void Initialise()
    {
        _cpu.NextValue();
        _diskRead.NextValue();
        _diskWrite.NextValue();
        SnapshotNetwork();
    }

    public float GetCpuPercent() => _cpu.NextValue();

    public (float UsedGb, float TotalGb, float Percent) GetRam()
    {
        var info = GC.GetGCMemoryInfo();
        float total = info.TotalAvailableMemoryBytes / (1024f * 1024f * 1024f);

        // Read committed memory via PerformanceCounter
        using var counter = new PerformanceCounter("Memory", "Committed Bytes");
        float used = counter.NextValue() / (1024f * 1024f * 1024f);
        float percent = total > 0 ? (used / total) * 100f : 0f;
        return (used, total, Math.Min(percent, 100f));
    }

    public (float ReadMbs, float WriteMbs) GetDisk()
    {
        float read = _diskRead.NextValue() / (1024f * 1024f);
        float write = _diskWrite.NextValue() / (1024f * 1024f);
        return (read, write);
    }

    public (float SentKbs, float ReceivedKbs) GetNetwork()
    {
        long sent = 0, received = 0;
        foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (nic.OperationalStatus != OperationalStatus.Up) continue;
            var stats = nic.GetIPv4Statistics();
            sent += stats.BytesSent;
            received += stats.BytesReceived;
        }

        float sentKbs = (sent - _lastBytesSent) / 1024f;
        float recvKbs = (received - _lastBytesReceived) / 1024f;

        _lastBytesSent = sent;
        _lastBytesReceived = received;

        return (Math.Max(0, sentKbs), Math.Max(0, recvKbs));
    }

    private void SnapshotNetwork()
    {
        foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (nic.OperationalStatus != OperationalStatus.Up) continue;
            var stats = nic.GetIPv4Statistics();
            _lastBytesSent += stats.BytesSent;
            _lastBytesReceived += stats.BytesReceived;
        }
    }
}