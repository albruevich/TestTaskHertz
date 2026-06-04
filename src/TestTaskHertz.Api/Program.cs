using JasperFx;
using Marten;
using TestTaskHertz.Api.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMarten(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Postgres")
        ?? throw new InvalidOperationException("Connection string 'Postgres' is missing.");

    options.Connection(connectionString);
    options.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
    options.Schema.For<Job>().Identity(job => job.Id);
});

builder.Services.AddSingleton<IJobQueue, JobQueue>();
builder.Services.AddHostedService<JobBackgroundService>();

var app = builder.Build();

app.MapPost("/jobs", async (IDocumentSession session, IJobQueue jobQueue, CancellationToken cancellationToken) =>
{
    var job = new Job
    {
        Id = Guid.NewGuid(),
        Status = JobStatus.Created,
        CreatedAt = DateTimeOffset.UtcNow
    };

    session.Store(job);

    await session.SaveChangesAsync(cancellationToken);
    await jobQueue.EnqueueAsync(job.Id, cancellationToken);

    return Results.Created($"/jobs/{job.Id}", new { jobId = job.Id });
});

app.MapGet("/jobs/{id:guid}", async (Guid id, IQuerySession session, CancellationToken cancellationToken) =>
{
    var job = await session.LoadAsync<Job>(id, cancellationToken);

    return job is null
        ? Results.NotFound()
        : Results.Ok(job);
});

app.Run();
