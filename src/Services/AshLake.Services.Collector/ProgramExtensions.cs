using Hellang.Middleware.ProblemDetails;
using Microsoft.OpenApi.Models;
using Dapr.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using HealthChecks.UI.Client;
using Hangfire.Dashboard;

namespace AshLake.Services.Collector;

internal static class ProgramExtensions
{
    public const string AppName = "Collector API";

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

    public static void AddCustomControllers(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddControllers()
            .AddDapr();

        builder.Services.AddEndpointsApiExplorer();
    }

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

    public static void AddCustomHealthChecks(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddDapr()
            .AddUrlGroup(new Uri($"http://{builder.Configuration["ImageStorageEndpoint"]}/minio/health/live"),
                         "imagestorage",
                         null,
                         new string[] { "minio" });
    }

    public static void UseCustomHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        app.MapHealthChecks("/liveness", new HealthCheckOptions
        {
            Predicate = r => r.Name.Contains("self")
        });
    }

    public static void AddCustomHangfire(this WebApplicationBuilder builder)
    {
        builder.Services.AddHangfire(c =>
        {
            c.UseRedisStorage(builder.Configuration["HangfireConnectionString"]);
        });

        builder.Services.AddHangfireServer(opt =>
        {
            opt.ShutdownTimeout = TimeSpan.FromMinutes(30);
            opt.WorkerCount = 5;
        });
    }

    public static void UseCustomHangfireDashboard(this WebApplication app)
    {
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new IDashboardAuthorizationFilter[0]
        });
    }

    public static void AddCustomApplicationServices(this WebApplicationBuilder builder)
    {
        #region Repositories

        builder.Services.AddSingleton(x => new Minio.MinioClient()
                .WithEndpoint(builder.Configuration["ImageStorageEndpoint"])
                .WithCredentials(builder.Configuration["ImageStorageAccessKey"],
                    builder.Configuration["ImageStorageSecretKey"])
                .Build()
        );
        builder.Services.AddSingleton(typeof(IS3ObjectRepositoty<>), typeof(S3ObjectRepositoty<>));

        #endregion

        #region Integration

        builder.Services.AddScoped<IEventBus, DaprEventBus>();

        builder.Services.AddSingleton<IGrabberService<Yandere>>(_ =>
            new GrabberService<Yandere>(DaprClient.CreateInvokeHttpClient("grabber")));
        builder.Services.AddSingleton<IGrabberService<Danbooru>>(_ =>
            new GrabberService<Danbooru>(DaprClient.CreateInvokeHttpClient("grabber")));
        builder.Services.AddSingleton<IGrabberService<Konachan>>(_ =>
            new GrabberService<Konachan>(DaprClient.CreateInvokeHttpClient("grabber")));

        builder.Services.AddSingleton<IArchiverService<Yandere>>(_ =>
            new ArchiverService<Yandere>(DaprClient.CreateInvokeHttpClient("archiver")));
        builder.Services.AddSingleton<IArchiverService<Danbooru>>(_ =>
            new ArchiverService<Danbooru>(DaprClient.CreateInvokeHttpClient("archiver")));
        builder.Services.AddSingleton<IArchiverService<Konachan>>(_ =>
            new ArchiverService<Konachan>(DaprClient.CreateInvokeHttpClient("archiver")));
        #endregion

        #region BackgroundJobs

        builder.Services.AddHttpClient<IDownloader, Downloader>(config =>
        {
            config.Timeout = TimeSpan.FromMinutes(5);
        });

        builder.Services.AddScoped(typeof(CollectingJob<>));

        #endregion
    }
}