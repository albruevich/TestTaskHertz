# TestTaskHertz

Prototype client-server system for tracking long-running background jobs.

The project contains:

- `b` / backend: ASP.NET Core API, Marten, PostgreSQL, SignalR
- `f` / frontend: .NET MAUI mobile app for iOS Simulator

## What It Does

The mobile app creates a background job on the backend. The backend saves the job in PostgreSQL through Marten, puts it into an in-memory queue, processes it in a `BackgroundService`, and sends status updates to the mobile app through SignalR.

Job lifecycle:

```text
Created
  -> wait 5 seconds
InProgress
  -> wait 5 seconds
Completed
```

The mobile app shows:

- current status: `Очікування...`, `У роботі...`, `Готово!`
- active `ActivityIndicator` while the job is running
- job id
- total execution time
- timestamps for `CreatedAt`, `StartedAt`, `FinishedAt`

![iOS Simulator](docs/ios_simu.webp)

## Requirements

- Docker Desktop
- .NET SDK 10
- .NET MAUI workload
- Xcode
- iOS Simulator runtime
- `make`

Check .NET workloads:

```bash
dotnet workload list
```

Expected workloads include:

```text
maui
ios
android
```

Check Xcode:

```bash
xcodebuild -version
xcode-select -p
```

The active developer directory should point to:

```text
/Applications/Xcode.app/Contents/Developer
```

## Project Structure

```text
src/TestTaskHertz.Api      backend API
src/TestTaskHertz.Mobile   MAUI mobile app
docker-compose.yml         local PostgreSQL
Makefile                   helper commands
docs                       screenshots for README
```

## Quick Start

From the repository root:

```bash
docker compose up -d
dotnet restore TestTaskHertz.sln
make b
```

In a second terminal:

```bash
make f
```

`make f` will:

1. Find a booted iOS Simulator
2. Open Simulator if needed
3. Build the MAUI app
4. Install the app into the simulator
5. Launch the app

## Run PostgreSQL

PostgreSQL is started through Docker Compose:

```bash
docker compose up -d
```

The database settings are:

```text
Host: localhost
Port: 5432
Database: task_hertz
User: task_hertz
Password: task_hertz
```

Check that the container is running:

```bash
docker ps
```

Expected container:

```text
task-hertz-postgres
```

![Docker Desktop](docs/docker.webp)

## Run Backend

Start the backend:

```bash
make b
```

Backend URL:

```text
http://localhost:5090
```

Available endpoints:

```text
POST /jobs
GET  /jobs/{id}
SignalR /jobsHub
```

## Test Backend From Terminal

Create a job:

```bash
curl -i -X POST http://localhost:5090/jobs
```

Example response:

```json
{"jobId":"511bbf48-376d-4db5-9f26-4ff78b0722c5"}
```

Read a job:

```bash
curl http://localhost:5090/jobs/511bbf48-376d-4db5-9f26-4ff78b0722c5
```

The `status` value is an enum:

```text
0 = Created
1 = InProgress
2 = Completed
```

Wait a few seconds and call `GET /jobs/{id}` again to see the status and timestamps change.

## Run Mobile App

Start the iOS Simulator app:

```bash
make f
```

Tap:

```text
Запустити задачу
```

Expected flow:

```text
Очікування...
У роботі...
Готово!
```

The app uses normal HTTP requests for creating/reading the job and SignalR for real-time status updates.

## Inspect Database Optional

You can inspect saved Marten documents in pgAdmin.

Connection settings:

```text
Host: localhost
Port: 5432
Maintenance database: task_hertz
Username: task_hertz
Password: task_hertz
```

Marten creates a document table:

```text
task_hertz
  -> Schemas
    -> public
      -> Tables
        -> mt_doc_job
```

The `data` column stores the serialized `Job` document as `jsonb`.

![pgAdmin](docs/pg_admin.webp)

## Helper Commands

```bash
make b          # run backend
make f          # build, install and launch iOS app
make f-logs     # stream mobile app logs from simulator
make restore    # restore NuGet packages
make build-api  # build backend only
```

## Notes

The project currently targets `.NET 10` because it was implemented and tested in a local .NET 10 SDK environment.

The mobile project includes temporary iOS build settings:

```xml
<ValidateXcodeVersion>false</ValidateXcodeVersion>
<MtouchLink>SdkOnly</MtouchLink>
```

They are used to keep local iOS Simulator builds working when the installed .NET iOS workload expects a slightly newer Xcode minor version.

## Troubleshooting

If backend port `5090` is already in use:

```bash
lsof -nP -iTCP:5090 -sTCP:LISTEN
kill <PID>
```

If PostgreSQL is not available, make sure Docker Desktop is running and restart the database:

```bash
docker compose up -d
```

If VS Code shows stale MAUI/XAML errors but terminal build succeeds:

```bash
dotnet build src/TestTaskHertz.Mobile/TestTaskHertz.Mobile.csproj -f net10.0-ios -r iossimulator-arm64 --tl:off
```

Then reload VS Code:

```text
Cmd+Shift+P -> Developer: Reload Window
```
