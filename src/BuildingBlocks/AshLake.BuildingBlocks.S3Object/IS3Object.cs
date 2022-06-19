namespace AshLake.BuildingBlocks.S3Object;

public interface IS3Object
{
    public string ObjectKey { get; }

    public byte[] Data { get; }
}
