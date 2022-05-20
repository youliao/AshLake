namespace AshLake.Contracts.Seedwork.Extensions;
public static class CustomConvert
{
    public static string ToBase64(this Stream stream)
    {
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        var bytes = memoryStream.ToArray();

        return Convert.ToBase64String(bytes);
    }
}
