using AshLake.Services.YandeStore.Application.Posts;
using AshLake.Services.YandeStore.Application.Services;
using AshLake.Services.YandeStore.Infrastructure.Services;
using Dapr.Client;
using HealthChecks.UI.Client;
using Hellang.Middleware.ProblemDetails;
using Newtonsoft.Json.Converters;
using Npgsql;
using Serilog;

namespace AshLake.Services.YandeStore;

internal static class ProgramExtensions
{
    public const string AppName = "YandeStore API";

    public static void AddCustomConfiguration(this WebApplicationBuilder builder)
    {
        // Disabled temporarily until https://github.com/dapr/dotnet-sdk/issues/779 is resolved.
        //builder.Configuration.AddDaprSecretStore(
        //    "eshop-secretstore",
        //    new DaprClientBuilder().Build());
    }

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

            // This will map NotImplementedException to the 501 Not Implemented status code.
            c.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);

            // This will map HttpRequestException to the 503 Service Unavailable status code.
            c.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);

            // Because exceptions are handled polymorphically, this will act as a "catch all" mapping, which is why it's added last.
            // If an exception other than NotImplementedException and HttpRequestException is thrown, this will handle it.
            c.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
        });
    }

    public static void AddCustomSerilog(this WebApplicationBuilder builder)
    {
        var seqServerUrl = builder.Configuration["SeqServerUrl"];

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console()
            .WriteTo.Seq(seqServerUrl)
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
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<YandeDbContext>>();
        var context = dbContextFactory.CreateDbContext();
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

    public static void AddCustomApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IYandeArchiverService>(_ =>
            new YandeArchiverService(DaprClient.CreateInvokeHttpClient("archiver")));

        builder.Services.AddScoped<IPostRepository, PostRepository>();

        builder.Services.AddPooledDbContextFactory<YandeDbContext>(
            options => options.UseNpgsql(builder.Configuration["DBConnectionString"])
                              .EnableServiceProviderCaching(false), 100);
    }

    public static void AddCustomTypeAdapterConfigs(this WebApplicationBuilder builder)
    {
        PostTypeAdapterConfigs.AddPostCommandTypeAdapterConfig();
        PostTypeAdapterConfigs.PostMetadataDtoTypeAdapterConfig();
    }
}