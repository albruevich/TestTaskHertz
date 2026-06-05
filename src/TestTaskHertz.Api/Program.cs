using JasperFx;
using Marten;
using TestTaskHertz.Api.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Налаштовуємо Marten для роботи з PostgreSQL
builder.Services.AddMarten(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Postgres")
        ?? throw new InvalidOperationException("Connection string 'Postgres' is missing.");

    options.Connection(connectionString);

    // Дозволяємо Marten автоматично створити таблиці в локальній базі через JasperFx
    options.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;

    // Вказуємо, що Job.Id буде ключем документа
    options.Schema.For<Job>().Identity(job => job.Id);
});

// Реєструємо чергу задач і фоновий сервіс
builder.Services.AddSingleton<IJobQueue, JobQueue>();
builder.Services.AddHostedService<JobBackgroundService>();

// Додаємо SignalR для real-time оновлень статусу
builder.Services.AddSignalR();

var app = builder.Build();

// Реєструємо endpoint для створення задачі
app.MapPost("/jobs", async (IDocumentSession session, IJobQueue jobQueue, CancellationToken cancellationToken) =>
{
    // Створюємо нову задачу зі статусом Created
    var job = new Job
    {
        Id = Guid.NewGuid(),
        Status = JobStatus.Created,
        CreatedAt = DateTimeOffset.UtcNow
    };

    session.Store(job);

    // Зберігаємо задачу в базу і додаємо її в чергу
    await session.SaveChangesAsync(cancellationToken);
    await jobQueue.EnqueueAsync(job.Id, cancellationToken);

    return Results.Created($"/jobs/{job.Id}", new { jobId = job.Id });
});

// Реєструємо endpoint для читання задачі по id
app.MapGet("/jobs/{id:guid}", async (Guid id, IQuerySession session, CancellationToken cancellationToken) =>
{
    // Читаємо актуальний стан задачі з бази
    var job = await session.LoadAsync<Job>(id, cancellationToken);

    return job is null ? Results.NotFound() : Results.Ok(job);
});

// Реєструємо SignalR hub для підключення клієнтів
app.MapHub<JobsHub>("/jobsHub");

app.Run();
