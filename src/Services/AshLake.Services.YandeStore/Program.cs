using Hellang.Middleware.ProblemDetails;

const string appName = "YandeStore API";

var builder = WebApplication.CreateBuilder(args);
builder.AddCustomSerilog();
builder.AddCustomSwagger();
builder.AddCustomProblemDetails();
builder.AddCustomControllers();
builder.AddCustomTypeAdapterConfigs();
builder.AddCustomApplicationServices();
builder.AddCustomHealthChecks();

builder.Services.AddMediatR(typeof(Program));

var app = builder.Build();

app.UseCustomSwagger();
app.UseProblemDetails();
app.UseCloudEvents();
app.MapControllers();
app.MapSubscribeHandler();
app.UseCustomHealthChecks();

try
{
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
