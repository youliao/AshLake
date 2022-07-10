using Hellang.Middleware.ProblemDetails;
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

    public static void AddCustomControllers(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddControllers()
            .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new BsonDocumentJsonConverter());
        });

        builder.Services.AddEndpointsApiExplorer();
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

    public static void AddCustomMassTransit(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediator(cfg =>
        {
            cfg.AddConsumer<CreatePostFileDownloadTaskConsumer>();

            cfg.AddConsumer<CreateAddPostMetadataJobsCommandConsumer<Yandere>>();
            cfg.AddConsumer<CreateAddPostMetadataJobsCommandConsumer<Danbooru>>();
            cfg.AddConsumer<CreateAddPostMetadataJobsCommandConsumer<Konachan>>();

            cfg.AddConsumer<CreateReplacePostMetadataJobsCommandConsumer<Yandere>>();
            cfg.AddConsumer<CreateReplacePostMetadataJobsCommandConsumer<Danbooru>>();
            cfg.AddConsumer<CreateReplacePostMetadataJobsCommandConsumer<Konachan>>();
        });

        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumers(Assembly.GetEntryAssembly());

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(builder.Configuration["RabbitMqHost"]);
                cfg.ConfigureEndpoints(context);
            });
        });
    }

    public static void AddCustomHangfire(this WebApplicationBuilder builder)
    {
        builder.Services.AddHangfire(c =>
        {
            c.UseRedisStorage(builder.Configuration["RedisConnectionString"]);
        });

        builder.Services.AddHangfireServer(opt =>
        {
            opt.ServerName = $"localhost-{nameof(Yandere).ToLower()}";
            opt.WorkerCount = 1;
            opt.Queues = new[] { nameof(Yandere).ToLower()};
        });

        builder.Services.AddHangfireServer(opt =>
        {
            opt.ServerName = $"localhost-{nameof(Danbooru).ToLower()}";
            opt.WorkerCount = 1;
            opt.Queues = new[] { nameof(Danbooru).ToLower() };
        });

        builder.Services.AddHangfireServer(opt =>
        {
            opt.ServerName = $"localhost-{nameof(Konachan).ToLower()}";
            opt.WorkerCount = 1;
            opt.Queues = new[] { nameof(Konachan).ToLower() };
        });
    }

    public static void UseCustomHangfireDashboard(this WebApplication app)
    {
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new IDashboardAuthorizationFilter[0]
        });
    }

    public static void AddCustomHealthChecks(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddDapr()
            .AddMongoDb(builder.Configuration["MongoConnectionString"],
                        "database",
                        null,
                        new string[] { "mongodb" });
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

    public static void AddCustomApplicationServices(this WebApplicationBuilder builder)
    {
        #region Repositories

        builder.Services.AddSingleton(_ =>
        {
            return new MongoClient(builder.Configuration["MongoConnectionString"]);
        });
        builder.Services.AddSingleton(typeof(IMetadataRepository<,>), typeof(MetadataRepository<,>));
        builder.Services.AddSingleton(typeof(IPostRelationRepository), typeof(PostRelationRepository));

        #endregion

        #region Integration

        builder.Services.AddHttpClient<IBooruApiService, BooruApiService>(config =>
        {
            config.BaseAddress = new Uri(builder.Configuration["BooruApiHost"]);
        });

        builder.Services.AddHttpClient<IBooruApiService<Yandere>, BooruApiService<Yandere>>(config =>
        {
            config.BaseAddress = new Uri(builder.Configuration["BooruApiHost"]);
        });

        builder.Services.AddHttpClient<IBooruApiService<Danbooru>, BooruApiService<Danbooru>>(config =>
        {
            config.BaseAddress = new Uri(builder.Configuration["BooruApiHost"]);
        });

        builder.Services.AddHttpClient<IBooruApiService<Konachan>, BooruApiService<Konachan>>(config =>
        {
            config.BaseAddress = new Uri(builder.Configuration["BooruApiHost"]);
        });

        builder.Services.AddHttpClient<ICollectorService, CollectorService>(config =>
        {
            config.BaseAddress = new Uri(builder.Configuration["CollectorHost"]);
        });

        builder.Services.AddHttpClient<IImgProxyService, ImgProxyService>(config =>
        {
            config.BaseAddress = new Uri(builder.Configuration["ImgProxyHost"]);
        });

        #endregion

        #region BackgroundJobs

        builder.Services.AddScoped(typeof(PostMetadataJob<>));
        builder.Services.AddScoped(typeof(TagMetadataJob<>));

        #endregion
    }
}