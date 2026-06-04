namespace TestTaskHertz.Api.Jobs;

public class Job
{
    public Guid Id { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Created;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }
}
