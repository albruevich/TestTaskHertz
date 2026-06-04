namespace TestTaskHertz.Mobile.Models;

public sealed record JobDto(
    Guid Id,
    JobStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? FinishedAt);
