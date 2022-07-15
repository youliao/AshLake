using Hellang.Middleware.ProblemDetails;
using MongoDB.Driver;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Hangfire.Dashboard;
using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Autofac;

namespace AshLake.Services.Archiver;

internal static class ProgramExtensions
{
    public const string AppName = "Archiver API";

    public static void UseSerilog(this WebApplicationBuilder builder)
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

    public static void AddProblemDetails(this WebApplicationBuilder builder)
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

    public static void AddControllers(this WebApplicationBuilder builder)
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

    public static void AddMediatR(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
    }

    public static void AddMassTransit(this WebApplicationBuilder builder)
    {
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

    public static void AddHangfire(this WebApplicationBuilder builder)
    {
        builder.Services.AddHangfire(c =>
        {
            c.UseRedisStorage(builder.Configuration["RedisConnectionString"]);
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
            .AddMongoDb(builder.Configuration["MongoConnectionString"],
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

        #endregion

        #region BackgroundJobs

        builder.Services.AddScoped(typeof(PostMetadataJob<>));
        builder.Services.AddScoped(typeof(TagMetadataJob<>));

        #endregion
    }

    public static void UseAutofac(this WebApplicationBuilder builder)
    {
        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
        {
            builder.RegisterGeneric(typeof(CreateAddPostMetadataJobsCommandHandler<>)).AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(BooruApiService<>)).AsImplementedInterfaces();
        });
    }
}