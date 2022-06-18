﻿using Hellang.Middleware.ProblemDetails;
using AshLake.Services.Collector.Domain.Repositories;
using Microsoft.OpenApi.Models;
using Dapr.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using HealthChecks.UI.Client;

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
            Authorization = new[] { new HangfireAuthorizationFilter() }
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

        builder.Services.AddSingleton<IDownloadService, DownloadService>(_ =>
        {
            var downloadOpt = new DownloadConfiguration()
            {
                ChunkCount = 8, // file parts to download, default value is 1
                OnTheFlyDownload = true, // caching in-memory or not? default values is true
                ParallelDownload = true // download parts of file as parallel or not. Default value is false
            };

            return new DownloadService(downloadOpt);
        });

        builder.Services.AddSingleton<IGrabberService, GrabberService>(_ =>
            new GrabberService(DaprClient.CreateInvokeHttpClient("grabber")));

        builder.Services.AddSingleton<IDanbooruService, DanbooruGrabberService>(_ => 
            new DanbooruGrabberService(DaprClient.CreateInvokeHttpClient("grabber")));
        #endregion

        #region BackgroundJobs

        builder.Services.AddScoped(typeof(YandeJob));

        #endregion
    }
}