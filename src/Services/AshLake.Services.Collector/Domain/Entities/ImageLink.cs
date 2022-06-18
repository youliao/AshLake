namespace AshLake.Services.Collector.Domain.Entities;

public record class ImageLink(string Url, string Md5);

public static class ImageLinkExtensions
{
    public static string GetObjectId(this ImageLink link)
    {
        var fileExt = Path.GetExtension(link.Url);

        return link.Md5 + fileExt;
    }
}