namespace AshLake.Services.Archiver.Domain.Entities;

public interface IStoragble
{
    public byte[] Data { get; }

    public string ObjectKey { get; }
}
