using Quartz;
using Quartz.Impl.Matchers;

namespace BlazorTaskScheduler.Quartz
{
    public class QuartzService : IQuartzService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;

        public QuartzService (ISchedulerFactory schedulerFactory) 
        {
            _schedulerFactory = schedulerFactory;
            
        }
        private async Task<IScheduler> GetSchedulerAsync()
        {
            if (_scheduler == null)
            {
                _scheduler = await _schedulerFactory.GetScheduler();
            }
            return _scheduler;
        }
        async Task<List<JobDetailData>> IQuartzService.GetJobsAsync()
        {

            var scheduler = await GetSchedulerAsync();
            var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

            var jobList = new List<JobDetailData>();

            foreach (var jobKey in jobKeys)
            {
                var detail = await this.GetJobDetail(jobName:jobKey.Name,group:jobKey.Group);
                if(detail != null)
                {
                    jobList.Add(detail); 
                }
            }

            return jobList;
        }

        public async Task RunJobNow(string jobName, string group = "DEFAULT")
        {
            var scheduler = await GetSchedulerAsync();
            await scheduler.TriggerJob(new JobKey(jobName, group));
        }

        public async Task PauseJob(string jobName, string group = "DEFAULT")
        {
            var scheduler = await GetSchedulerAsync();
            await scheduler.PauseJob(new JobKey(jobName, group));
        }

        public async Task ResumeJob(string jobName, string group = "DEFAULT")
        {
            var scheduler = await GetSchedulerAsync();
            await scheduler.ResumeJob(new JobKey(jobName, group));
        }

        public async Task<JobDetailData?> GetJobDetail(string jobName, string group)
        {
           var key = new JobKey(jobName, group);
           var scheduler = await GetSchedulerAsync();
           var detail = await scheduler.GetJobDetail(key);
           var triggers = await GetJobTriggers(jobName, group);

           var triggerStates = await Task.WhenAll(triggers.Select(t => scheduler.GetTriggerState(t.Key)));

           if (detail == null)
            {
                return null;
            }

            var jobDetail = new JobDetailData
            {
                Name = detail.Key.Name,
                Group = detail.Key.Group,
                Description = detail.Description,
                JobType = detail.JobType.FullName ?? "",
                Durable = detail.Durable,
                RequestsRecovery = detail.RequestsRecovery,
                Concurrent = detail.ConcurrentExecutionDisallowed,
                PersistData = detail.PersistJobDataAfterExecution,
                DataMap = detail.JobDataMap.ToDictionary(k => k.Key, v => v.Value),
                Status = triggerStates.Any(s => s == TriggerState.Error) ? "Error" :
                         triggerStates.Any(s => s == TriggerState.Paused) ? "Paused" :
                         triggerStates.All(s => s == TriggerState.Complete) ? "Complete" :
                         "Active"
            };

            jobDetail.Triggers = triggers.Select(t => new TriggerInfoData 
            { 
                Name = t.Key.Name,
                Group = t.Key.Group,
                NextFireTime = t.GetNextFireTimeUtc()?.UtcDateTime,
                PreviousFireTime = t.GetPreviousFireTimeUtc()?.UtcDateTime,
                CronExpression = (t as ICronTrigger)?.CronExpressionString,
                TriggerType = t.GetType().Name
            }).ToList();

            jobDetail.NextRun = jobDetail.Triggers
                                   .Where(t => t.NextFireTime.HasValue)
                                   .Select(t => t.NextFireTime!.Value)
                                   .OrderBy(t => t)
                                   .FirstOrDefault();

            jobDetail.LastRun = jobDetail.Triggers
                                    .Where(t => t.PreviousFireTime.HasValue)
                                    .Select(t => t.PreviousFireTime!.Value)
                                    .OrderByDescending(t => t)
                                    .FirstOrDefault();

            return jobDetail;
        }

        public async Task<IReadOnlyCollection<ITrigger>> GetJobTriggers(string jobName, string group)
        {
            var key = new JobKey(jobName, group);
            var scheduler = await GetSchedulerAsync();
            return await scheduler.GetTriggersOfJob(key);
        }
    }
}
