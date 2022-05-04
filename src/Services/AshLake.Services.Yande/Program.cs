const string appName = "Yande API";

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomJsonOptions();
builder.AddCustomDatabase();
builder.AddCustomRepositories();
builder.AddCustomSwagger();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCarter();
builder.Services.AddMediatR(typeof(Program));
var app = builder.Build();
app.UseCustomSwagger();
//app.UseRouting();
//app.UseCloudEvents();
//app.UseEndpoints(endpoint =>
//{
//    endpoint.MapSubscribeHandler();
//});


app.MapCarter();

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
