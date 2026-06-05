using Microsoft.AspNetCore.SignalR;
using Marten;

namespace TestTaskHertz.Api.Jobs;

public class JobBackgroundService : BackgroundService
{
    private readonly IJobQueue jobQueue;
    private readonly IDocumentStore documentStore;
    private readonly IHubContext<JobsHub> hubContext;
    private readonly ILogger<JobBackgroundService> logger;

    // Конструктор викликається автоматично через DI
    public JobBackgroundService(IJobQueue jobQueue, IDocumentStore documentStore, IHubContext<JobsHub> hubContext, ILogger<JobBackgroundService> logger)
    {
        this.jobQueue = jobQueue;
        this.documentStore = documentStore;
        this.hubContext = hubContext;
        this.logger = logger;
    }

    // Метод запускається автоматично як фоновий сервіс
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Чекаємо наступну задачу з черги
            var jobId = await jobQueue.DequeueAsync(stoppingToken);

            try
            {
                await ProcessJobAsync(jobId, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to process job {JobId}", jobId);
            }
        }
    }

    private async Task ProcessJobAsync(Guid jobId, CancellationToken cancellationToken)
    {
        // Імітуємо очікування перед стартом задачі
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

        await UpdateJobAsync(jobId, JobStatus.InProgress, job => job.StartedAt = DateTimeOffset.UtcNow, cancellationToken);

        // Імітуємо виконання задачі
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

        await UpdateJobAsync(jobId, JobStatus.Completed, job => job.FinishedAt = DateTimeOffset.UtcNow, cancellationToken);
    }

    private async Task UpdateJobAsync(Guid jobId, JobStatus status, Action<Job> updateTimestamps, CancellationToken cancellationToken)
    {
        // Відкриваємо сесію, щоб прочитати і зберегти Job у PostgreSQL
        await using var session = documentStore.LightweightSession();
        var job = await session.LoadAsync<Job>(jobId, cancellationToken);

        if (job == null)
        {
            logger.LogWarning("Job {JobId} was not found", jobId);
            return;
        }

        // Оновлюємо статус і часову мітку
        job.Status = status;
        updateTimestamps(job);

        session.Store(job);
        await session.SaveChangesAsync(cancellationToken);

        // Надсилаємо клієнтам актуальний стан задачі через SignalR
        await hubContext.Clients.Group($"job-{job.Id}").SendAsync("JobChanged", job, cancellationToken);
    }
}
