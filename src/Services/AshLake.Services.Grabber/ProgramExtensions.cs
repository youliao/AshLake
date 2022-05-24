using EasyCaching.Core.Configurations;
using Hellang.Middleware.ProblemDetails;
using Newtonsoft.Json.Converters;

namespace AshLake.Services.Grabber;

public static class ProgramExtensions
{
    private const string AppName = "Grabber API";

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
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

        //.AddJsonOptions(options =>
        //{
        //    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        //    options.JsonSerializerOptions.Converters.Add(new BsonDocumentJsonConverter());
        //});

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

    public static void AddCustomDatabase(this WebApplicationBuilder builder)
    {

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

    public static void AddCustomRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<YandeSourceSiteRepository>();
        builder.Services.AddHttpClient<IYandeSourceSiteRepository, YandeSourceSiteRepository>(config =>
        {
            config.BaseAddress = new Uri(builder.Configuration["YandeUrl"]);
            //config.BaseAddress = new Uri("https://yande.re/");
            config.Timeout = TimeSpan.FromSeconds(30);
            config.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));

        }).ConfigurePrimaryHttpMessageHandler(provider => new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip
        });
    }

}
