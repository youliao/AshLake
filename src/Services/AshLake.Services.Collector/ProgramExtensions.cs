using Hellang.Middleware.ProblemDetails;
using Microsoft.OpenApi.Models;
using Serilog;

namespace AshLake.Services.Collector;

internal static class ProgramExtensions
{
    public const string AppName = "Collector API";

    public static void AddCustomSerilog(this WebApplicationBuilder builder)
    {
        var seqServerUrl = builder.Configuration["SeqServerUrl"];

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console(Serilog.Events.LogEventLevel.Warning)
            .WriteTo.Seq(seqServerUrl, Serilog.Events.LogEventLevel.Warning)
            .Enrich.WithProperty("ApplicationName", AppName)
            .CreateLogger();

        builder.Host.UseSerilog();
    }

    public static void AddCustomProblemDetails(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(c =>
        {
            // You can configure the middleware to re-throw certain types of exceptions, all exceptions or based on a predicate.
            // This is useful if you have upstream middleware that needs to do additional handling of exceptions.
            c.Rethrow<NotSupportedException>();
            c.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);
            c.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);
            c.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
        });
    }

    public static void AddCustomSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = $"Ash Lake - {AppName}", Version = "v1" });
        });
    }

    public static void UseCustomSwagger(this WebApplication app)
    {
        app.UseSwagger().UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{AppName} V1");
        });
    }

    public static void AddAria2Client(this WebApplicationBuilder builder)
    {
        var url = builder.Configuration["Arai2RpcUrl"];
        var secret = builder.Configuration["Arai2RpcSecret"];


        builder.Services.AddSingleton(_ => new Aria2NetClient(url, secret));
    }

    public static void AddMinioClient(this WebApplicationBuilder builder)
    {
        var endpoint = builder.Configuration["MinioEndpoint"];
        var accessKey = builder.Configuration["MinioAccessKey"];
        var secretKey = builder.Configuration["MinioSecretKey"];

        builder.Services.AddSingleton(_ => new Minio.MinioClient().WithEndpoint(endpoint).WithCredentials(accessKey, secretKey).Build());
    }
}
