namespace AshLake.Services.Grabber.Domain.Entities;

public class ImageLink
{
    public string Url { get; private set; }
    public string PostMD5 { get; private set; }

    public ImageType Type { get; private set; }

    public ImageLink(string url, string postMD5, ImageType type)
    {
        Url = url ?? throw new ArgumentNullException(nameof(url));
        PostMD5 = postMD5 ?? throw new ArgumentNullException(nameof(postMD5));
        Type = type;
    }
}
