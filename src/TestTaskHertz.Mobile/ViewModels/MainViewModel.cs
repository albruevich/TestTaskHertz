using TestTaskHertz.Mobile.Models;

namespace TestTaskHertz.Mobile.ViewModels;

public class MainViewModel
{
    public string StatusText { get; private set; } = "Очікування запуску";
    public bool IsRunning { get; private set; }
    public JobDto? CurrentJob { get; private set; }
}
