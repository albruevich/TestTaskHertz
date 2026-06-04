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

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/jobs", async (IDocumentSession session, CancellationToken cancellationToken) =>
{
    var job = new Job
    {
        Id = Guid.NewGuid(),
        Status = JobStatus.Created,
        CreatedAt = DateTimeOffset.UtcNow
    };

    session.Store(job);
    await session.SaveChangesAsync(cancellationToken);

    return Results.Created($"/jobs/{job.Id}", new { jobId = job.Id });
});

app.Run();
