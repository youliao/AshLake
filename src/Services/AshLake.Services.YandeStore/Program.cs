using Hellang.Middleware.ProblemDetails;

const string appName = "YandeStore API";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.AddCustomSerilog();
builder.AddCustomSwagger();
builder.AddCustomProblemDetails();
builder.AddCustomControllers();
//builder.AddCustomHealthChecks();
builder.AddCustomApplicationServices();
builder.AddCustomTypeAdapterConfigs();

builder.Services.AddMediatR(typeof(Program));

var app = builder.Build();

app.UseCustomSwagger();
app.UseProblemDetails();
app.UseCloudEvents();
app.MapControllers();
app.MapSubscribeHandler();

//app.MapHealthChecks("/hc", new HealthCheckOptions()
//{
//    Predicate = _ => true,
//    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
//});
//app.MapHealthChecks("/liveness", new HealthCheckOptions
//{
//    Predicate = r => r.Name.Contains("self")
//});

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
