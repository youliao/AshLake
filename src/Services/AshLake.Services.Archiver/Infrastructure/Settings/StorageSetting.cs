namespace AshLake.Services.Archiver.Infrastructure.Settings;

public abstract class StorageSetting
{
    public string Endpoint { get; set; } = null!;
    public string AccessKey { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
}

public class PostFileStorageSetting : StorageSetting { }
public class PostPreviewStorageSetting : StorageSetting { }

