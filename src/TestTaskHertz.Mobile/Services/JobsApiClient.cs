using System.Net.Http.Json;
using TestTaskHertz.Mobile.Models;

namespace TestTaskHertz.Mobile.Services;

public class JobsApiClient
{
    private readonly HttpClient httpClient = new() { BaseAddress = new Uri("http://localhost:5090") };

    public async Task<Guid> PostJobAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync("/jobs", content: null, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<CreateJobResponse>(cancellationToken);

        return result?.JobId ?? throw new InvalidOperationException("The create job response did not contain a job id.");
    }

    public async Task<JobDto?> GetJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"/jobs/{jobId}", cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<JobDto>(cancellationToken);
    }

    private record CreateJobResponse(Guid JobId);
}
