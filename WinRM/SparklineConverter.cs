using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ResourceMonitor;

public class SparklineConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not ObservableCollection<float> history || history.Count == 0)
            return new PointCollection();

        const double width = 200;
        const double height = 50;

        float max = history.Max();
        float min = history.Min();
        float range = max - min;
        if (range < 1f) range = 1f;

        var points = new PointCollection();
        int count = history.Count;

        for (int i = 0; i < count; i++)
        {
            double x = (i / (double)(count - 1)) * width;
            double y = height - ((history[i] - min) / range * height);
            points.Add(new Point(x, y));
        }

        return points;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}