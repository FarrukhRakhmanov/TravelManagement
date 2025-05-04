namespace Domain.ViewModels
{
    /// <summary>
    /// View Model for Line Chart
    /// </summary>
    public class LineChartVM
    {
        public required List<ChartData> Series { get; set; }
        public required string[] Categories { get; set; }

    }

    /// <summary>
    /// Chart Data
    /// </summary>
    public class ChartData
    {
        public required string Name { get; set; }
        public required int[] Data { get; set; }
    }
}
