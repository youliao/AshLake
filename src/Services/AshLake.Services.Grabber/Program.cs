using Hellang.Middleware.ProblemDetails;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaprClient();

builder.AddCustomSerilog();
builder.AddCustomProblemDetails();
builder.AddCustomControllers();
builder.AddCustomSwagger();
builder.AddCustomHealthChecks();
builder.AddCustomEasyCaching();
builder.AddCustomApplicationServices();

var app = builder.Build();

app.UseProblemDetails();
app.UseCustomSwagger();
app.UseCloudEvents();
app.MapControllers();
app.MapSubscribeHandler();
app.UseCustomHealthChecks();

app.MapGet("/", () => Results.Redirect("/swagger"));

try
{
    app.Logger.LogInformation("Starting web host ({ApplicationName})...", ProgramExtensions.AppName);
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Host terminated unexpectedly ({ApplicationName})...", ProgramExtensions.AppName);
}
finally
{
    Serilog.Log.CloseAndFlush();
}