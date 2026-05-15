namespace BlazorTaskScheduler.Quartz
{
    public class TriggerInfoData
    {
        public string Name { get; set; }
        public string Group { get; set; } = "";

        public DateTimeOffset? NextFireTime { get; set; }
        public DateTimeOffset? PreviousFireTime { get; set; }

        public string? CronExpression { get; set; }
        public string TriggerType { get; set; } = "";
    }
}
