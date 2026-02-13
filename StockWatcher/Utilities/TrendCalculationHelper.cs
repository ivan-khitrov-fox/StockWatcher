using MathNet.Numerics;
using StockWatcher.Models;

namespace StockWatcher.Utilities;

public class Trend
{
    /// <summary>
    /// b in  f(x) = ax + b
    /// </summary>
    public double Intersect { get; set; }
    /// <summary>
    /// a in  f(x) = ax + b
    /// </summary>
    public double Slope { get; set; }
    /// <summary>
    /// Diapason start
    /// </summary>
    public double x0 { get; set; }
    /// <summary>
    /// Diapason end
    /// </summary>
    public double x1 { get; set; }
}

public static class TrendCalculationHelper
{
    public static Trend CalculateTrendByHoursHistory(List<HourHistory> hourHistory)
    {
        if (!hourHistory.Any()) return null;

        double[] xAxis;
        double[] yAxis;
        ConvertHourHistoryToAxis(hourHistory, out xAxis, out yAxis);
        var trend = CalculateTrend(xAxis, yAxis);
        return trend;
    }

    /// <summary>
    /// Prepares HourHistory list to Trend calculation axis arrays.
    /// </summary>
    private static void ConvertHourHistoryToAxis(List<HourHistory> hourHistory, out double[] xAxis, out double[] yAxis)
    {
        var xAxisList = new List<double>();
        var yAxisList = new List<double>();
        var dict = new Dictionary<long, double>();

        foreach (var item in hourHistory)
        {
            var start = item.StartTime.Ticks;
            var end = item.EndTime.Ticks;
            AddTimePriceValue(dict, item.StartPrice, start);
            AddTimePriceValue(dict, item.EndPrice, end);
        }
        foreach (var time in dict.Keys)
        {
            xAxisList.Add(Convert.ToDouble(time));
            yAxisList.Add(dict[time]);
        }

        xAxis = xAxisList.ToArray();
        yAxis = yAxisList.ToArray();
    }

    private static void AddTimePriceValue(Dictionary<long, double> dict, double price, long timeTicks)
    {
        if (dict.ContainsKey(timeTicks))
        {
            dict[timeTicks] = new double[] { dict[timeTicks], price }.Average();
        }
        else
        {
            dict.Add(timeTicks, price);
        }
    }

    /// <summary>
    /// Calculates trend function f(x) = ax + b by double axis arrays {x, y}
    /// </summary>
    /// <param name="xAxis">x axis</param>
    /// <param name="yAxis">y axis</param>
    /// <returns>Trend instance</returns>
    public static Trend CalculateTrend(double[] xAxis, double[] yAxis)
    {
        if (!xAxis.Any() || !yAxis.Any()) return null;

        var p = Fit.Line(xAxis, yAxis);
        // f(x) = ax + b
        double intercept = p.Item1; // b
        double slope = p.Item2; // a
        return new Trend
        {
            Intersect = intercept,
            Slope = slope,
            x0 = xAxis.Min(),
            x1 = xAxis.Max()
        };
    }
}
