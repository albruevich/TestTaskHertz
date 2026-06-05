using TestTaskHertz.Mobile.Services;

namespace TestTaskHertz.Mobile;

public partial class MainPage : ContentPage
{
    private readonly JobsApiClient jobsApiClient = new();

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnTestButtonClicked(object? sender, EventArgs e)
    {
        StartJobButton.IsEnabled = false;
        StatusLabel.IsVisible = true;
        StatusLabel.Text = "Відправляю запит...";

        try
        {
            // Створюємо задачу і одразу читаємо її стан
            var jobId = await jobsApiClient.PostJobAsync();
            var job = await jobsApiClient.GetJobAsync(jobId);

            StatusLabel.Text = job is null
                ? $"Job created: {jobId}"
                : $"ID: {job.Id}\nStatus: {job.Status}\nCreatedAt: {job.CreatedAt:HH:mm:ss}";
        }
        catch (Exception exception)
        {
            StatusLabel.Text = $"Request failed: {exception.Message}";
        }
        finally
        {
            StartJobButton.IsEnabled = true;
        }
    }
}
