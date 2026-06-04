using TestTaskHertz.Mobile.Models;

namespace TestTaskHertz.Mobile.Services;

public sealed class JobsSignalRClient
{
    public event Func<JobDto, Task>? JobChanged
    {
        add { }
        remove { }
    }

    public Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SubscribeToJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
