namespace AshLake.Services.Collector.Domain.Entities;

public interface IS3Object
{
    public Stream Data { get; }

    public string ObjectKey { get; }
}
