namespace BlazorTaskScheduler.Quartz
{
    public class JobDetailData
    {
        public string Name { get; set; } = "";
        public string Group { get; set; } = "";
        public string? Description { get; set; }
        public string JobType { get; set; } = "";
        public bool Durable { get; set; }
        public bool RequestsRecovery { get; set; }
        public bool Concurrent { get; set; }
        public bool PersistData { get; set; }
        public string Status { get; set; } = "Unknown";
        public DateTimeOffset? NextRun { get; set; }
        public DateTimeOffset? LastRun { get; set; }
        public Dictionary<string, object> DataMap { get; set; } = new();
        public List<TriggerInfoData> Triggers { get; set; } = new();
    }

}
