using Quartz;

namespace BlazorTaskScheduler.Quartz
{
    public interface IQuartzService
    {
        Task<List<JobDetailData>> GetJobsAsync();
        Task<JobDetailData?> GetJobDetail(string jobName, string group = "DEFAULT");
        Task<IReadOnlyCollection<ITrigger>> GetJobTriggers(string jobName, string group = "DEFAULT");
        Task RunJobNow(string jobName, string group = "DEFAULT");
        Task PauseJob(string jobName, string group = "DEFAULT");
        Task ResumeJob(string jobName, string group = "DEFAULT");
    }
}
