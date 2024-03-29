﻿using AshLake.Services.YandeStore.Application.Posts;
using AshLake.Services.YandeStore.Infrastructure.Repositories.Posts;
using AshLake.Services.YandeStore.Infrastructure.Services;
using Dapr.Client;
using Hangfire.Dashboard;
using HealthChecks.UI.Client;
using Hellang.Middleware.ProblemDetails;
using Newtonsoft.Json.Converters;
using Npgsql;
using Serilog;

namespace AshLake.Services.YandeStore;

internal static class ProgramExtensions
{
    public const string AppName = "YandeStore API";

    public static void AddCustomControllers(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddControllers()
            .AddDapr()
            .AddNewtonsoftJson(options => { options.SerializerSettings.Converters.Add(new StringEnumConverter()); });

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

    public static void AddCustomSerilog(this WebApplicationBuilder builder)
    {
        var seqServerUrl = builder.Configuration["SeqServerUrl"];

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console( Serilog.Events.LogEventLevel.Warning)
            .WriteTo.Seq(seqServerUrl,  Serilog.Events.LogEventLevel.Warning)
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

        builder.Services.AddSwaggerGenNewtonsoftSupport();
    }

    public static void UseCustomSwagger(this WebApplication app)
    {
        app.UseSwagger().UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{AppName} V1"); });
    }

    public static void AddCustomHealthChecks(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddDapr()
            .AddNpgSql(
                builder.Configuration["DBConnectionString"],
                name: "database",
                tags: new string[] { "postgresql" });
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

    public static void ApplyDatabaseMigration(this WebApplication app)
    {
        // Apply database migration automatically. Note that this approach is not
        // recommended for production scenarios. Consider generating SQL scripts from
        // migrations instead.
        using var scope = app.Services.CreateScope();

        //var retryPolicy = CreateRetryPolicy(app.Configuration, Log.Logger);
        var context = scope.ServiceProvider.GetRequiredService<YandeDbContext>();
        //retryPolicy.Execute(context.Database.Migrate);
        context.Database.Migrate();
        using var conn = (NpgsqlConnection)context.Database.GetDbConnection();
        conn.Open();
        conn.ReloadTypes();
    }

    private static Policy CreateRetryPolicy(IConfiguration configuration, Serilog.ILogger logger)
    {
        // Only use a retry policy if configured to do so.
        // When running in an orchestrator/K8s, it will take care of restarting failed services.
        if (bool.TryParse(configuration["RetryMigrations"], out bool retryMigrations))
        {
            return Policy.Handle<Exception>().WaitAndRetryForever(
                sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                onRetry: (exception, retry, timeSpan) =>
                {
                    logger.Warning(
                        exception,
                        "Exception {ExceptionType} with message {Message} detected during database migration (retry attempt {retry}, connection {connection})",
                        exception.GetType().Name,
                        exception.Message,
                        retry,
                        configuration["DBConnectionString"]);
                }
            );
        }

        return Policy.NoOp();
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
            opt.WorkerCount = 3;
            opt.Queues = new[] { nameof(Post).ToLower() };
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
        builder.Services.AddScoped<IYandeArchiverService>(_ =>
            new YandeArchiverService(DaprClient.CreateInvokeHttpClient("archiver")));

        builder.Services.AddScoped<IPostRepository, PostRepository>();

        builder.Services.AddDbContext<YandeDbContext>(
            options => options.UseNpgsql(builder.Configuration["DBConnectionString"]));
    }

    public static void AddCustomTypeAdapterConfigs(this WebApplicationBuilder builder)
    {
        PostTypeAdapterConfigs.Load();
    }
}