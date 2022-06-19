namespace AshLake.Services.Compressor.Domain.Entities;

public class PostPreview : IS3Object, IDisposable
{
    private bool disposed = false;
    public string ObjectKey { get; private set; } = null!;

    public Stream Data { get; private set; }

    public PostPreview(string objectKey, Stream data)
    {
        ObjectKey = objectKey ?? throw new ArgumentNullException(nameof(objectKey));
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        if (disposing)
        {
            Data.Dispose();
        }

        disposed = true;
    }
}
