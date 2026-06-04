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
            var jobId = await jobsApiClient.PostJobAsync();

            StatusLabel.Text = $"Job created: {jobId}";
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
