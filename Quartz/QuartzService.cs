using Quartz;
using Quartz.Impl.Matchers;

namespace BlazorTaskScheduler.Quartz
{
    public class QuartzService : IQuartzService
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public QuartzService (ISchedulerFactory schedulerFactory) {
            _schedulerFactory = schedulerFactory;
        }
        async Task<List<JobInfoData>> IQuartzService.GetJobsAsync()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

            var list = new List<JobInfoData>();

            foreach (var jobKey in jobKeys)
            {
                var detail = await scheduler.GetJobDetail(jobKey);
                var triggers = await scheduler.GetTriggersOfJob(jobKey);

                var trigger = triggers.FirstOrDefault();

                var nextRun = trigger?.GetNextFireTimeUtc();
                var lastRun = trigger?.GetPreviousFireTimeUtc();

                var state = trigger != null
                    ? await scheduler.GetTriggerState(trigger.Key)
                    : TriggerState.None;

                list.Add(new JobInfoData
                {
                    Name = jobKey.Name,
                    Group = jobKey.Group,
                    Description = detail.Description,
                    NextRun = nextRun,
                    LastRun = lastRun,
                    IsPaused = state == TriggerState.Paused
                });
            }

            return list;
        }

        public async Task RunJobNow(string jobName, string group = "DEFAULT")
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.TriggerJob(new JobKey(jobName, group));
        }

        public async Task PauseJob(string jobName, string group = "DEFAULT")
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.PauseJob(new JobKey(jobName, group));
        }

        public async Task ResumeJob(string jobName, string group = "DEFAULT")
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.ResumeJob(new JobKey(jobName, group));
        }
    }
}
