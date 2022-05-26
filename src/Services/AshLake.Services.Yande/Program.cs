using Hellang.Middleware.ProblemDetails;

const string appName = "Yande API";

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomProblemDetails();
builder.AddCustomControllers();
builder.AddCustomRepositories();
builder.AddCustomSwagger();

builder.Services.AddMediatR(typeof(Program));
var app = builder.Build();

app.UseCustomSwagger();
app.UseProblemDetails();
app.UseCloudEvents();
app.MapControllers();
app.MapSubscribeHandler();

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
