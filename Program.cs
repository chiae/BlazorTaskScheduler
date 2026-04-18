using BlazorTaskScheduler.Components;
using BlazorTaskScheduler.Quartz;
using BlazorTaskScheduler.QuartzJobs;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddQuartz(static q =>
{
    // Add a test job
    q.AddJob<SampleJob>(opts => opts.WithIdentity("SampleJob").WithDescription("Test job only."));
    // Trigger job every 10 seconds
    q.AddTrigger(opts => opts
        .ForJob("SampleJob")
        .WithIdentity("SampleJob-trigger")
        .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever()));
});

// Start Quartz as a background service
builder.Services.AddQuartzHostedService();
// Add QuartzService
builder.Services.AddSingleton<IQuartzService, QuartzService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
