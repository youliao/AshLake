namespace AshLake.Services.Archiver.Infrastructure;

public abstract class MongoDatabaseSetting
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}

public class YandeMongoDatabaseSetting : MongoDatabaseSetting
{
}