using Hellang.Middleware.ProblemDetails;
using MongoDB.Driver;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Hangfire.Dashboard;
using EasyCaching.Core.Configurations;

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
            .AddDapr()
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

    public static void AddCustomEasyCaching(this WebApplicationBuilder builder)
    {
        var redisConnArr = builder.Configuration["RedisConnectionString"].Split(':');
        var redisHost = redisConnArr.First();
        var redisPort = int.Parse(redisConnArr.Last());

        builder.Services.AddEasyCaching(options =>
        {
            options.UseRedis(config =>
            {
                config.DBConfig.Endpoints.Add(new ServerEndPoint(redisHost, redisPort));
                config.SerializerName = "json";
                config.DBConfig.Database = 0;
            });
        });
    }

    public static void AddCustomHangfire(this WebApplicationBuilder builder)
    {
        builder.Services.AddHangfire(c =>
        {
            c.UseRedisStorage(builder.Configuration["RedisConnectionString"],new Hangfire.Redis.RedisStorageOptions
            {
                Db = 1
            });
        });

        builder.Services.AddHangfireServer(opt =>
        {
            opt.ShutdownTimeout = TimeSpan.FromMinutes(30);
            opt.WorkerCount = 5;
            opt.Queues = new[] { "postmetadata", "tagmetadata" };
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

        #endregion

        #region Integration

        builder.Services.AddScoped<IEventBus, DaprEventBus>();
        builder.Services.AddSingleton<IBooruApiService<Yandere>>(_ =>
            new BooruApiService<Yandere>(DaprClient.CreateInvokeHttpClient("booru-api")));

        builder.Services.AddHttpClient<IBooruApiService<Danbooru>, BooruApiService<Danbooru>>(config =>
        {
            config.BaseAddress = new Uri(builder.Configuration["BooruApiHost"]);
        });

        //builder.Services.AddSingleton<IBooruApiService<Danbooru>>(_ =>
        //    new BooruApiService<Danbooru>(DaprClient.CreateInvokeHttpClient("booru-api")));

        builder.Services.AddSingleton<IBooruApiService<Konachan>>(_ =>
            new BooruApiService<Konachan>(DaprClient.CreateInvokeHttpClient("booru-api")));

        builder.Services.AddSingleton<ICollectorService>(_ =>
            new CollectorService(DaprClient.CreateInvokeHttpClient("collector")));

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