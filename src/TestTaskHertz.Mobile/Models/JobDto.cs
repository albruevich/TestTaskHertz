namespace TestTaskHertz.Mobile.Models;

public record JobDto(
    Guid Id,
    JobStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? FinishedAt);
