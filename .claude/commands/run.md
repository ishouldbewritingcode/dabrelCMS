Launch the dabrelCMS app and confirm it started successfully.

1. Run `dotnet run --launch-profile https` in the background.
2. Watch the output for the line `APPLICATION STARTED - Serilog is working!` — this confirms Serilog initialized and the app is ready.
3. If that line appears, report success and that the app is available at https://localhost:7272.
4. If the process exits or an exception appears before that line, report the error output and stop.
5. Do not open a browser — just confirm the app is running and show the URL.
