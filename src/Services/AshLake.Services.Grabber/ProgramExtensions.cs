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
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
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
        builder.Services.AddEasyCaching(options =>
        {
            options.UseInMemory(BooruSites.Yande);
        });

        builder.Services.AddEasyCaching(options =>
        {
            options.UseInMemory(BooruSites.Danbooru);
        });
    }

    public static void AddCustomRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<YandeSourceSiteRepository>();
        builder.Services.AddHttpClient<YandeSourceSiteRepository>(config =>
        {
            config.BaseAddress = new Uri(builder.Configuration["YandeUrl"]);
            config.Timeout = TimeSpan.FromSeconds(30);
        });
    }

}
