using AshLake.Services.Grabber.Infrastructure.Services;
using EasyCaching.Core.Configurations;
using HealthChecks.UI.Client;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json.Converters;
using Serilog;

namespace AshLake.Services.Grabber;

internal static class ProgramExtensions
{
    public const string AppName = "Grabber API";

    public static void AddCustomSerilog(this WebApplicationBuilder builder)
    {
        var seqServerUrl = builder.Configuration["SeqServerUrl"];

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console(Serilog.Events.LogEventLevel.Warning)
            .WriteTo.Seq(seqServerUrl, Serilog.Events.LogEventLevel.Information)
            .Enrich.WithProperty("ApplicationName", AppName)
            .CreateLogger();

        builder.Host.UseSerilog();
    }

    public static void AddCustomControllers(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddControllers()
            .AddDapr()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

        builder.Services.AddEndpointsApiExplorer();
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

    public static void AddCustomEasyCaching(this WebApplicationBuilder builder)
    {
        builder.Services.AddEasyCaching(options =>
        {
            options.UseRedis(config =>
            {
                config.DBConfig.Endpoints.Add(new ServerEndPoint(builder.Configuration["EasycachingRedisHost"], 6379));
                config.SerializerName = "json";
                config.DBConfig.Database = 0;
            },nameof(Yande));

            options.UseRedis(config =>
            {
                config.DBConfig.Endpoints.Add(new ServerEndPoint(builder.Configuration["EasycachingRedisHost"], 6379));
                config.SerializerName = "json";
                config.DBConfig.Database = 1;
            }, nameof(Danbooru));

            options.WithJson("json");
        });
    }

    public static void AddCustomHealthChecks(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddDapr()
            .AddRedis(builder.Configuration["EasycachingRedisHost"], "easycaching", null, new string[] { "redis" })
            .AddUrlGroup(new Uri("https://yande.re/post.json?limit=1"), "yande", null, new string[] { "soucesites" })
            .AddUrlGroup(new Uri("https://danbooru.donmai.us/posts.json?limit=1"), "danbooru", null, new string[] { "soucesites" });
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

        builder.Services.AddScoped<YandeSourceSiteService>();
        builder.Services.AddHttpClient<IYandeSourceSiteService, YandeSourceSiteService>(config =>
        {
            config.BaseAddress = new Uri(builder.Configuration["YandeUrl"]);
            config.Timeout = TimeSpan.FromSeconds(30);
            config.DefaultRequestHeaders.AcceptEncoding.Add(
                new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));

        }).ConfigurePrimaryHttpMessageHandler(provider => new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip
        });

        #endregion
    }
}
