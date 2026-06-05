using System.Threading.Channels;

namespace TestTaskHertz.Api.Jobs;

public class JobQueue : IJobQueue
{
    // Зберігаємо id задач у внутрішній асинхронній черзі
    private readonly Channel<Guid> channel = Channel.CreateUnbounded<Guid>();

    public ValueTask EnqueueAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return channel.Writer.WriteAsync(jobId, cancellationToken);
    }

    public ValueTask<Guid> DequeueAsync(CancellationToken cancellationToken)
    {
        return channel.Reader.ReadAsync(cancellationToken);
    }
}
