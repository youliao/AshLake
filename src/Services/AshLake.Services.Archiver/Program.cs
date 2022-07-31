using Hellang.Middleware.ProblemDetails;

var builder = WebApplication.CreateBuilder(args);

builder.UseSerilog();

builder.AddProblemDetails();
builder.AddControllers();
builder.AddDapr();
builder.AddSwagger();
builder.AddMassTransit();
builder.AddHangfire();
builder.AddHealthChecks();
builder.AddApplicationServices();

var app = builder.Build();

app.UseProblemDetails();
app.UseSwagger();
app.UseDapr();
app.MapControllers();
app.UseHealthChecks();
app.UseHangfireDashboard();

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