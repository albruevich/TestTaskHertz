using TestTaskHertz.Mobile.ViewModels;

namespace TestTaskHertz.Mobile;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainViewModel();
    }
}
