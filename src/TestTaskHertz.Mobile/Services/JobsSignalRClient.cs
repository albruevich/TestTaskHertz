using Microsoft.AspNetCore.SignalR.Client;
using TestTaskHertz.Mobile.Models;

namespace TestTaskHertz.Mobile.Services;

public class JobsSignalRClient
{
    private readonly HubConnection hubConnection;

    public event Func<JobDto, Task>? JobChanged;

    public JobsSignalRClient()
    {
        // Створюємо підключення до SignalR hub на backend
        hubConnection = new HubConnectionBuilder()
            .WithUrl($"{AppConfig.ApiBaseUrl}/jobsHub")
            .WithAutomaticReconnect()
            .Build();

        // Обробляємо повідомлення JobChanged від backend
        hubConnection.On<JobDto>("JobChanged", async job =>
        {
            if (JobChanged != null)
            {
                await JobChanged(job);
            }
        });
    }

    public async Task ConnectToJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        if (hubConnection.State == HubConnectionState.Disconnected)
        {
            // Відкриваємо real-time підключення
            await hubConnection.StartAsync(cancellationToken);
        }

        // Підписуємося на оновлення конкретної задачі
        await hubConnection.InvokeAsync("SubscribeToJob", jobId, cancellationToken);
    }
}
