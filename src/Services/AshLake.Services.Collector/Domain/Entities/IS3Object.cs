namespace AshLake.Services.Collector.Domain.Entities;

public interface IS3Object
{
    public byte[] Data { get; }

    public string ObjectKey { get; }
}
