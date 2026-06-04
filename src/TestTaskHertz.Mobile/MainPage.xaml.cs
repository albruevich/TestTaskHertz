using System.Diagnostics;

namespace TestTaskHertz.Mobile;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void OnTestButtonClicked(object? sender, EventArgs e)
    {
        var message = $"Test button clicked at {DateTimeOffset.Now:HH:mm:ss}";

        Console.WriteLine(message);
        Debug.WriteLine(message);
    }
}
