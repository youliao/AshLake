using Hellang.Middleware.ProblemDetails;

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomSerilog();
builder.AddCustomProblemDetails();
builder.AddCustomControllers();
builder.AddCustomSwagger();
builder.AddCustomMediatR();
builder.AddCustomMassTransit();
builder.AddCustomHangfire();
builder.AddCustomHealthChecks();
builder.AddCustomApplicationServices();

var app = builder.Build();

app.UseProblemDetails();
app.UseCustomSwagger();
app.MapControllers();
app.UseCustomHealthChecks();
app.UseCustomHangfireDashboard();

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