using AshLake.Services.Grabber.Infrastructure;
using System.Text.Json.Serialization;

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

    public static void AddCustomDatabase(this WebApplicationBuilder builder)
    {

    }

    public static void AddCustomEasyCaching(this WebApplicationBuilder builder)
    {
        Newtonsoft.Json.JsonConvert.DefaultSettings = new Func<Newtonsoft.Json.JsonSerializerSettings>(() =>
        {
            Newtonsoft.Json.JsonSerializerSettings setting = new Newtonsoft.Json.JsonSerializerSettings();
            setting.Converters.Add(new BsonDocumentNewtonsoftConverter());
            return setting;
        });

        builder.Services.AddEasyCaching(options =>
        {
            options.UseLiteDB(config =>
            {
                config.DBConfig = new EasyCaching.LiteDB.LiteDBDBOptions { FileName = "yande.ldb"};
            });
        });
    }

    public static void AddCustomRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<YandeSourceSiteRepository>();
        builder.Services.AddHttpClient<IYandeSourceSiteRepository, YandeSourceSiteRepository>(config =>
        {
            //config.BaseAddress = new Uri(builder.Configuration["YandeUrl"]);
            config.BaseAddress = new Uri("https://yande.re/");
            config.Timeout = TimeSpan.FromSeconds(30);
            config.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));

        }).ConfigurePrimaryHttpMessageHandler(provider => new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip
        });
    }

}
