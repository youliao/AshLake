using Hellang.Middleware.ProblemDetails;
using MongoDB.Driver;
using System.Text.Json.Serialization;

namespace AshLake.Services.Archiver;

public static class ProgramExtensions
{
    private const string AppName = "Archiver API";

    public static void AddCustomConfiguration(this WebApplicationBuilder builder)
    {
        // Disabled temporarily until https://github.com/dapr/dotnet-sdk/issues/779 is resolved.
        //builder.Configuration.AddDaprSecretStore(
        //    "eshop-secretstore",
        //    new DaprClientBuilder().Build());
    }

    public static void AddCustomGrabberServices(this WebApplicationBuilder builder)
    {
    }

    public static void AddCustomBackgroundJobs(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped(typeof(YandeJob));
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

    public static void AddCustomRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(_ =>
        {
            return new MongoClient(builder.Configuration["MongoDatabaseConnectionString"]);
        });
        builder.Services.AddSingleton(typeof(IMetadataRepository<,>), typeof(MetadataRepository<,>));

        builder.Services.AddSingleton(_ =>
        {
            return new Minio.MinioClient()
                            .WithEndpoint(builder.Configuration["ImageStorageEndpoint"])
                            .WithCredentials(builder.Configuration["ImageStorageAccessKey"],
                                             builder.Configuration["ImageStorageSecretKey"])
                            .Build();
        });
        builder.Services.AddSingleton(typeof(IPostImageRepositoty<>), typeof(PostImageRepositoty<>));
    }

    public static void AddCustomAddHangfire(this WebApplicationBuilder builder)
    {
        builder.Services.AddHangfire(c =>
        {
            c.UseRedisStorage(builder.Configuration["HangfireConnectionString"]);
        });
        builder.Services.AddHangfireServer(opt =>
        {
            opt.ShutdownTimeout = TimeSpan.FromMinutes(30);
            opt.WorkerCount = 10;
            opt.Queues = new[] { "metadata", "file", "preview" };
        });
    }

    public static void AddCustomApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IYandeGrabberService>(_ =>
            new YandeGrabberService(DaprClient.CreateInvokeHttpClient("grabber")));

        builder.Services.AddScoped<IEventBus, DaprEventBus>();
        builder.Services.AddScoped<PostMetadataAddedIntegrationEventHandler>();
    }
}