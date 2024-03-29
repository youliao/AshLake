﻿using Hellang.Middleware.ProblemDetails;
using MongoDB.Driver;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Hangfire.Dashboard;
using System.Reflection;

namespace AshLake.Services.Archiver;

internal static class ProgramExtensions
{
    public const string AppName = "Archiver API";

    public static void UseSerilog(this WebApplicationBuilder builder)
    {
        var SeqServerHost = builder.Configuration["SEQ_SERVER_HOST"];

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console(Serilog.Events.LogEventLevel.Warning)
            .WriteTo.Seq(SeqServerHost, Serilog.Events.LogEventLevel.Warning)
            .Enrich.WithProperty("ApplicationName", AppName)
            .CreateLogger();

        builder.Host.UseSerilog();
    }

    public static void AddProblemDetails(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(c =>
        {
            // You can configure the middleware to re-throw certain types of exceptions, all exceptions or based on a predicate.
            // This is useful if you have upstream middleware that needs to do additional handling of exceptions.
            c.Rethrow<NotSupportedException>();
            c.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);
            c.MapToStatusCode<RequestFaultException>(StatusCodes.Status400BadRequest);
            c.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);
            c.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
        });
    }

    public static void AddControllers(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddControllers()
            .AddDapr()
            .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new BsonDocumentJsonConverter());
        });

        builder.Services.AddEndpointsApiExplorer();
    }

    public static void AddDapr(this WebApplicationBuilder builder)
    {
        builder.Services.AddDaprClient();
    }

    public static void UseDapr(this WebApplication app)
    {
        app.UseCloudEvents();
        app.MapSubscribeHandler();
    }

    public static void AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = $"Ash Lake - {AppName}", Version = "v1" });
        });
    }

    public static void UseSwagger(this WebApplication app)
    {
        SwaggerBuilderExtensions.UseSwagger(app).UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{AppName} V1");
        });
    }

    public static void AddMassTransit(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediator(cfg =>
        {
            cfg.AddConsumer<CreateAddPostMetadataJobsHandler<Yandere>>();
            cfg.AddConsumer<CreateAddPostMetadataJobsHandler<Danbooru>>();
            cfg.AddConsumer<CreateAddPostMetadataJobsHandler<Konachan>>();

            cfg.AddConsumer<CreateReplacePostMetadataJobsHandler<Yandere>>();
            cfg.AddConsumer<CreateReplacePostMetadataJobsHandler<Danbooru>>();
            cfg.AddConsumer<CreateReplacePostMetadataJobsHandler<Konachan>>();

            cfg.AddConsumers(Assembly.GetEntryAssembly());
        });
    }

    public static void AddHangfire(this WebApplicationBuilder builder)
    {
        builder.Services.AddHangfire(c =>
        {
            c.UseRedisStorage(builder.Configuration["REDIS_CONNECTION_STRING"]);
        });

        builder.Services.AddHangfireServer(opt =>
        {
            opt.ServerName = $"localhost-common";
            opt.WorkerCount = 10;
            opt.Queues = new[] { "common" };
        });

        builder.Services.AddHangfireServer(opt =>
        {
            opt.ServerName = $"localhost-{Yandere.Alias}";
            opt.WorkerCount = 1;
            opt.Queues = new[] { nameof(Yandere).ToLower()};
        });

        builder.Services.AddHangfireServer(opt =>
        {
            opt.ServerName = $"localhost-{Danbooru.Alias}";
            opt.WorkerCount = 1;
            opt.Queues = new[] { nameof(Danbooru).ToLower() };
        });

        builder.Services.AddHangfireServer(opt =>
        {
            opt.ServerName = $"localhost-{Konachan.Alias}";
            opt.WorkerCount = 1;
            opt.Queues = new[] { nameof(Konachan).ToLower() };
        });
    }

    public static void UseHangfireDashboard(this WebApplication app)
    {
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new IDashboardAuthorizationFilter[0]
        });
    }

    public static void AddHealthChecks(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddMongoDb(builder.Configuration["MONGO_CONNECTION_STRING"],
                        "database",
                        null,
                        new string[] { "mongodb" });
    }

    public static void UseHealthChecks(this WebApplication app)
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

    public static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        #region Repositories

        builder.Services.AddSingleton(_ =>
        {
            return new MongoClient(builder.Configuration["MONGO_CONNECTION_STRING"]);
        });
        builder.Services.AddSingleton(typeof(IMetadataRepository<,>), typeof(MetadataRepository<,>));
        builder.Services.AddSingleton(typeof(IPostRelationRepository), typeof(PostRelationRepository));

        #endregion

        #region Integration

        builder.Services.AddHttpClient<IBooruApiService, BooruApiService>(config =>
        {
            config.BaseAddress = new Uri(builder.Configuration["BOORUAPI_HOST"]);
        });

        builder.Services.AddHttpClient<IBooruApiService<Yandere>, BooruApiService<Yandere>>(config =>
        {
            config.BaseAddress = new Uri(builder.Configuration["BOORUAPI_HOST"]);
        });

        builder.Services.AddHttpClient<IBooruApiService<Danbooru>, BooruApiService<Danbooru>>(config =>
        {
            config.BaseAddress = new Uri(builder.Configuration["BOORUAPI_HOST"]);
        });

        builder.Services.AddHttpClient<IBooruApiService<Konachan>, BooruApiService<Konachan>>(config =>
        {
            config.BaseAddress = new Uri(builder.Configuration["BOORUAPI_HOST"]);
        });

        builder.Services.AddHttpClient<ICollectorRCloneService, CollectorRCloneService>(config =>
        {
            config.BaseAddress = new Uri(builder.Configuration["COLLECTOR_RCLONE_HOST"]);
        });

        builder.Services.AddSingleton<ICollectorAria2Service, CollectorAria2Service>(_ =>
        {
            var aria2NetClient = new Aria2NET.Aria2NetClient(builder.Configuration["COLLECTOR_ARIA2_HOST"] + "/jsonrpc",
            builder.Configuration["COLLECTOR_ARIA2_SECRET"]);

            return new CollectorAria2Service(aria2NetClient);
        });

        #endregion

        #region BackgroundJobs

        builder.Services.AddScoped(typeof(PostMetadataJob<>));
        builder.Services.AddScoped(typeof(TagMetadataJob<>));

        #endregion
    }
}