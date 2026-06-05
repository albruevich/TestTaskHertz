using TestTaskHertz.Mobile.Models;
using TestTaskHertz.Mobile.Services;

namespace TestTaskHertz.Mobile;

public partial class MainPage : ContentPage
{
    private readonly JobsApiClient jobsApiClient = new();
    private readonly JobsSignalRClient jobsSignalRClient = new();

    public MainPage()
    {
        InitializeComponent();

        // Підписуємо UI на оновлення задачі з SignalR
        jobsSignalRClient.JobChanged += OnJobChangedAsync;
    }

    private async void OnTestButtonClicked(object? sender, EventArgs e)
    {
        StartJobButton.IsEnabled = false;
        StatusLabel.IsVisible = true;
        StatusLabel.Text = "Відправляю запит...";

        try
        {
            // Створюємо задачу, підписуємося на SignalR і читаємо початковий стан
            var jobId = await jobsApiClient.PostJobAsync();
            await jobsSignalRClient.ConnectToJobAsync(jobId);

            var job = await jobsApiClient.GetJobAsync(jobId);

            StatusLabel.Text = job == null ? $"Job created: {jobId}" : BuildJobText(job);
        }
        catch (Exception exception)
        {
            StatusLabel.Text = $"Request failed: {exception.Message}";
            StartJobButton.IsEnabled = true;
        }
    }

    private Task OnJobChangedAsync(JobDto job)
    {
        return MainThread.InvokeOnMainThreadAsync(() =>
        {
            StatusLabel.Text = BuildJobText(job);
            StartJobButton.IsEnabled = job.Status == JobStatus.Completed;
        });
    }

    private static string BuildJobText(JobDto job)
    {
        var startedAt = job.StartedAt?.ToString("HH:mm:ss") ?? "-";
        var finishedAt = job.FinishedAt?.ToString("HH:mm:ss") ?? "-";

        return job.Status == JobStatus.Completed
            ? $"ID: {job.Id}\nStatus: {job.Status}\nCreatedAt: {job.CreatedAt:HH:mm:ss}\nStartedAt: {startedAt}\nFinishedAt: {finishedAt}"
            : $"ID: {job.Id}\nStatus: {job.Status}\nCreatedAt: {job.CreatedAt:HH:mm:ss}";
    }
}
