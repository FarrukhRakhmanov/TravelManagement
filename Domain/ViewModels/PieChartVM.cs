namespace Domain.ViewModels
{
    /// <summary>
    /// View model for Pie chart
    /// </summary>
    public class PieChartVM
    {
        public required int[] Series { get; set; }
        public required string[] Labels { get; set; }
    }
}
