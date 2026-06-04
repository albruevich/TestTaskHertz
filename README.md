# TestTaskHertz

Learning/prototype project for a MAUI mobile client and ASP.NET Core backend.

Current state:

- `b` = backend project: `src/TestTaskHertz.Api`
- `f` = frontend/mobile project: `src/TestTaskHertz.Mobile`

## Requirements

- .NET SDK 10
- .NET MAUI workload
- Xcode
- iOS Simulator runtime installed in Xcode

Check installed workloads:

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

## Restore

```bash
dotnet restore TestTaskHertz.sln
```

## Run Backend

From the repository root:

```bash
make b
```

The backend starts on:

```text
http://localhost:5090
```

Current test endpoint:

```text
GET /
```

Expected response:

```text
Hello World!
```

## Run Frontend

From the repository root:

```bash
make f
```

`make f` will:

1. Check whether an iOS Simulator is already running.
2. Open Simulator automatically if needed.
3. Wait for a booted simulator.
4. Build the MAUI app.
5. Install it into the booted simulator.
6. Launch the app.

Current expected screen:

```text
TestTaskHertz
Hello from f
```

## Notes

The mobile project currently uses iOS simulator builds:

```text
net10.0-ios
iossimulator-arm64
```

The project includes temporary iOS build settings because the installed .NET iOS workload expects a newer Xcode minor version:

```xml
<ValidateXcodeVersion>false</ValidateXcodeVersion>
<MtouchLink>SdkOnly</MtouchLink>
```

These can be revisited after aligning Xcode and .NET iOS workload versions.

## Reviewer Friendliness Checklist

Backend should be easy to verify from terminal:

```bash
make b
curl http://localhost:5090/
```

Mobile verification can require more local setup because MAUI/iOS depends on:

- installed .NET SDK;
- installed MAUI workload;
- installed Xcode;
- installed iOS Simulator runtime;
- compatible .NET iOS workload and Xcode versions.

Before final submission, update this README with the exact tested environment:

```text
macOS:
.NET SDK:
Xcode:
iOS Simulator runtime:
```

Before final submission, make the project friendly for review:

1. Keep backend runnable with one command.
2. Keep mobile runnable with one command.
3. Include clear setup commands.
4. Include clear troubleshooting notes for MAUI/Xcode version mismatch.
5. Include a screenshot of the working iOS Simulator app.
6. Re-check whether temporary iOS settings can be removed:

```xml
<ValidateXcodeVersion>false</ValidateXcodeVersion>
<MtouchLink>SdkOnly</MtouchLink>
```
