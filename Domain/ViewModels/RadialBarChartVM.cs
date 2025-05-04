namespace Domain.ViewModels
{
    /// <summary>
    /// View Model for Radial Bar Chart
    /// </summary>
    public class RadialBarChartVM
    {
        public decimal TotalCount { get; set; }
        public decimal CurrentMonthCount { get; set; }
        public bool HasRatioIncreased { get; set; }
        public int[] Series { get; set; }

    }
}
