using System.Data.SqlClient;

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

//if (!app.Environment.IsDevelopment())
if (true)
{
    app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            // using static System.Net.Mime.MediaTypeNames;
            context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;

            var exceptionHandlerPathFeature =
                context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature?.Error is DbUpdateException ex)
            {     
                await context.Response.WriteAsJsonAsync(new { SqlState = (ex.InnerException as System.Data.Common.DbException)?.SqlState ?? string.Empty });
            }
        });
    });

    app.UseHsts();
}

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
