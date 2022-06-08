namespace AshLake.Services.Compressor.Application.Services;

public interface ICollectorService
{
    Task<Stream> GetPostFile(string objectKey);

}