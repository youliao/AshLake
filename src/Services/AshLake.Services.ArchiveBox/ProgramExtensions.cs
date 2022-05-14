namespace AshLake.Services.ArchiveBox;

public static class ProgramExtensions
{
    private const string AppName = "ArchiveBox API";

    public static void AddCustomConfiguration(this WebApplicationBuilder builder)
    {
        // Disabled temporarily until https://github.com/dapr/dotnet-sdk/issues/779 is resolved.
        //builder.Configuration.AddDaprSecretStore(
        //    "eshop-secretstore",
        //    new DaprClientBuilder().Build());
    }


    public static void AddCustomJsonOptions(this WebApplicationBuilder builder)
    {

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
        builder.Services.AddSingleton(s =>
        {
            var settings = MongoClientSettings.FromConnectionString(builder.Configuration["MongoDBConnectionString"]);
            return new MongoClient(settings);
        });
    }

    public static void AddCustomEasyCaching(this WebApplicationBuilder builder)
    {
    }

    public static void AddCustomRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped(typeof(IYandeMetadataRepository<>), typeof(YandeMetadataRepository<>));
    }

}
