namespace BlazorTaskScheduler.Quartz
{
    public class JobInfoData
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? NextRun { get; set; }
        public DateTimeOffset? LastRun { get; set; }
        public bool IsPaused { get; set; }
    }
}
