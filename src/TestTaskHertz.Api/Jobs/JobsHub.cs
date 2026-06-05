using Microsoft.AspNetCore.SignalR;

namespace TestTaskHertz.Api.Jobs;

public class JobsHub : Hub
{
    // Клієнт підписується тільки на оновлення конкретної задачі
    public Task SubscribeToJob(Guid jobId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, $"job-{jobId}");
    }
}
