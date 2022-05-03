const string appName = "Yande API";

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomDatabase();
builder.AddCustomRepositories();
builder.AddCustomSwagger();

var app = builder.Services
    .AddEndpointsApiExplorer()
    .AddServices(builder);

app.UseCustomSwagger();
app.UseRouting();
app.UseCloudEvents();
app.UseEndpoints(endpoint =>
{
    endpoint.MapSubscribeHandler();
});

try
{
    app.Logger.LogInformation("Applying database migration ({ApplicationName})...", appName);
    app.ApplyDatabaseMigration();

    app.Logger.LogInformation("Starting web host ({ApplicationName})...", appName);
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Host terminated unexpectedly ({ApplicationName})...", appName);
}
finally
{
    Serilog.Log.CloseAndFlush();
}
