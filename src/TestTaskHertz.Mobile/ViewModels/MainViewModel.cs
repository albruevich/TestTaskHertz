using System.ComponentModel;
using System.Runtime.CompilerServices;
using TestTaskHertz.Mobile.Models;
using TestTaskHertz.Mobile.Services;

namespace TestTaskHertz.Mobile.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly JobsApiClient jobsApiClient = new();
    private readonly JobsSignalRClient jobsSignalRClient = new();
    private bool isJobRunning;
    private string statusText = "";
    private bool isStatusVisible;
    private bool isActivityIndicatorRunning;
    private double activityIndicatorOpacity;
    private bool isStartButtonEnabled = true;
    private double startButtonOpacity = 1.0;
    private bool isResultVisible;
    private string jobIdText = "";
    private string resultText = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    public MainViewModel()
    {
        StartJobCommand = new Command(async () => await StartJobAsync());

        // Підписуємо ViewModel на оновлення задачі з SignalR
        jobsSignalRClient.JobChanged += OnJobChangedAsync;
    }

    public Command StartJobCommand { get; }

    public string StatusText
    {
        get => statusText;
        private set => SetProperty(ref statusText, value);
    }

    public bool IsStatusVisible
    {
        get => isStatusVisible;
        private set => SetProperty(ref isStatusVisible, value);
    }

    public bool IsActivityIndicatorRunning
    {
        get => isActivityIndicatorRunning;
        private set => SetProperty(ref isActivityIndicatorRunning, value);
    }

    public double ActivityIndicatorOpacity
    {
        get => activityIndicatorOpacity;
        private set => SetProperty(ref activityIndicatorOpacity, value);
    }

    public bool IsStartButtonEnabled
    {
        get => isStartButtonEnabled;
        private set => SetProperty(ref isStartButtonEnabled, value);
    }

    public double StartButtonOpacity
    {
        get => startButtonOpacity;
        private set => SetProperty(ref startButtonOpacity, value);
    }

    public bool IsResultVisible
    {
        get => isResultVisible;
        private set => SetProperty(ref isResultVisible, value);
    }

    public string JobIdText
    {
        get => jobIdText;
        private set => SetProperty(ref jobIdText, value);
    }

    public string ResultText
    {
        get => resultText;
        private set => SetProperty(ref resultText, value);
    }

    private async Task StartJobAsync()
    {
        if (isJobRunning)
        {
            return;
        }

        SetJobRunningState(true);
        IsStatusVisible = true;
        IsResultVisible = false;
        ActivityIndicatorOpacity = 1;
        IsActivityIndicatorRunning = true;
        StatusText = "Очікування...";

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
            StatusText = $"Request failed: {exception.Message}";
            IsActivityIndicatorRunning = false;
            ActivityIndicatorOpacity = 0;
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
        StatusText = GetStatusText(job.Status);
        JobIdText = $"ID: {job.Id}";
        IsResultVisible = true;
        ResultText = BuildResultText(job);

        var isCompleted = job.Status == JobStatus.Completed;
        IsActivityIndicatorRunning = !isCompleted;
        ActivityIndicatorOpacity = isCompleted ? 0 : 1;
        SetJobRunningState(!isCompleted);
    }

    private void SetJobRunningState(bool isRunning)
    {
        isJobRunning = isRunning;
        IsStartButtonEnabled = !isRunning;
        StartButtonOpacity = isRunning ? 0.6 : 1.0;
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
        var totalTime = job.FinishedAt.HasValue ? (job.FinishedAt.Value - job.CreatedAt).TotalSeconds.ToString("0") + " sec" : "-";

        return $"Total time: {totalTime}\nCreatedAt: {job.CreatedAt:HH:mm:ss}\nStartedAt: {startedAt}\nFinishedAt: {finishedAt}";
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        OnPropertyChanged(propertyName);
    }
}
