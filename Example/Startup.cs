// Register the Missing Properties tool in your Startup.cs / Program.cs

// 1. Add the service registration
services.AddMissingProperties();

// 2. Make sure Blazor Server is enabled
services.AddServerSideBlazor();

// 3. Map the Blazor hub (in app configuration)
app.MapBlazorHub();
