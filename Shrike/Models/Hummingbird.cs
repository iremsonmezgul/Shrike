namespace Shrike.Models
{
    public class Hummingbird
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public int ActiveCallCount { get; set; }
        public int TotalRamMb { get; set; }
        public int UsedRamMb { get; set; }
        public int CpuUsage { get; set; }

    }
}
