namespace AshLake.Services.Collector.Domain.Entities;

public record class ImageLink(string Url, string ObjectKey);

public static class ImageLinkExtensions
{
    public static string GetMd5(this ImageLink link) => Path.GetFileNameWithoutExtension(link.ObjectKey);
}