using Quartz;

namespace BlazorTaskScheduler.QuartzJobs
{
    public class SampleJob : IJob
    {
        private readonly ILogger<SampleJob> _logger;

        public SampleJob(ILogger<SampleJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Sample job executed at {time}",DateTime.Now);
            return Task.CompletedTask;
        }
    }
}
