namespace BlazorTaskScheduler.Quartz
{
    public interface IQuartzService
    {
        Task<List<JobInfoData>> GetJobsAsync();
        Task RunJobNow(string jobName, string group = "DEFAULT");
        Task PauseJob(string jobName, string group = "DEFAULT");
        Task ResumeJob(string jobName, string group = "DEFAULT");
    }
}
