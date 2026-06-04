using TestTaskHertz.Mobile.Models;

namespace TestTaskHertz.Mobile.Services;

public class JobsApiClient
{
    public Task<Guid> CreateJobAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<JobDto?> GetJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
