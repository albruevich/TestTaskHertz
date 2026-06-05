using TestTaskHertz.Mobile.Models;
using TestTaskHertz.Mobile.Services;

namespace TestTaskHertz.Mobile;

public partial class MainPage : ContentPage
{
    private readonly JobsApiClient jobsApiClient = new();
    private readonly JobsSignalRClient jobsSignalRClient = new();
    private bool isJobRunning;

    public MainPage()
    {
        InitializeComponent();

        // Підписуємо UI на оновлення задачі з SignalR
        jobsSignalRClient.JobChanged += OnJobChangedAsync;
    }

    private async void OnTestButtonClicked(object? sender, EventArgs e)
    {
        if (isJobRunning)
        {
            return;
        }

        SetJobRunningState(true);
        StatusLabel.IsVisible = true;
        ResultLabel.IsVisible = false;
        JobActivityIndicator.IsVisible = true;
        JobActivityIndicator.IsRunning = true;
        StatusLabel.Text = "Очікування...";

        try
        {
            // Створюємо задачу, підписуємося на SignalR і читаємо початковий стан
            var jobId = await jobsApiClient.PostJobAsync();
            await jobsSignalRClient.ConnectToJobAsync(jobId);

            var job = await jobsApiClient.GetJobAsync(jobId);

            if (job != null)
            {
                UpdateJobUi(job);
            }
        }
        catch (Exception exception)
        {
            StatusLabel.Text = $"Request failed: {exception.Message}";
            JobActivityIndicator.IsRunning = false;
            JobActivityIndicator.IsVisible = false;
            SetJobRunningState(false);
        }
    }

    private Task OnJobChangedAsync(JobDto job)
    {
        return MainThread.InvokeOnMainThreadAsync(() =>
        {
            UpdateJobUi(job);
        });
    }

    private void UpdateJobUi(JobDto job)
    {
        StatusLabel.Text = GetStatusText(job.Status);
        ResultLabel.IsVisible = true;
        ResultLabel.Text = BuildResultText(job);

        var isCompleted = job.Status == JobStatus.Completed;
        JobActivityIndicator.IsRunning = !isCompleted;
        JobActivityIndicator.IsVisible = !isCompleted;
        SetJobRunningState(!isCompleted);
    }

    private void SetJobRunningState(bool isRunning)
    {
        isJobRunning = isRunning;
        StartJobButton.IsEnabled = !isRunning;
        StartJobButton.Opacity = isRunning ? 0.6 : 1.0;
    }

    private static string GetStatusText(JobStatus status)
    {
        return status switch
        {
            JobStatus.Created => "Очікування...",
            JobStatus.InProgress => "У роботі...",
            JobStatus.Completed => "Готово!",
            _ => "Невідомий статус"
        };
    }

    private static string BuildResultText(JobDto job)
    {
        var startedAt = job.StartedAt?.ToString("HH:mm:ss") ?? "-";
        var finishedAt = job.FinishedAt?.ToString("HH:mm:ss") ?? "-";

        return $"ID: {job.Id}\n\nCreatedAt: {job.CreatedAt:HH:mm:ss}\nStartedAt: {startedAt}\nFinishedAt: {finishedAt}";
    }
}
