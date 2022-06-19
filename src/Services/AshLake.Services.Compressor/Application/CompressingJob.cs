using AshLake.Contracts.Seedwork;
using AshLake.Services.Compressor.Infrastructure.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace AshLake.Services.Compressor.Application;

public class CompressingJob
{
    private readonly IS3ObjectRepositoty<PostPreview> _previewRepositoty;
    private readonly ICollectorService _collectorService;
    private readonly IDownloader _downloader;

    public CompressingJob(IS3ObjectRepositoty<PostPreview> previewRepositoty, ICollectorService collectorService, IDownloader downloader)
    {
        _previewRepositoty = previewRepositoty ?? throw new ArgumentNullException(nameof(previewRepositoty));
        _collectorService = collectorService ?? throw new ArgumentNullException(nameof(collectorService));
        _downloader = downloader ?? throw new ArgumentNullException(nameof(downloader));
    }

    public async Task<string> AddOrUpdatePreview(string objectKey)
    {
        var fileUrl = await _collectorService.GetPostFileUrlAsync(objectKey);
        if (fileUrl is null) return EntityState.None.ToString();

        var data = await _downloader.DownloadFileTaskAsync(fileUrl) ?? throw new InvalidDataException();

        using var image = Image.Load(data);

        using var resizedData = new MemoryStream();
        ResizeTo(image, 300);
        image.SaveAsJpeg(resizedData);

        var preview = new PostPreview(objectKey, resizedData.ToArray());
        await _previewRepositoty.PutAsync(preview);
        var isExists = await _previewRepositoty.ExistsAsync(objectKey);
        return isExists ? EntityState.Modified.ToString() : EntityState.Added.ToString();
    }

    public async Task<string> AddPreview(string objectKey)
    {
        if (objectKey is null) return EntityState.None.ToString();

        var isExists = await _previewRepositoty.ExistsAsync(objectKey);
        if (isExists) return EntityState.Unchanged.ToString();

        var fileUrl = await _collectorService.GetPostFileUrlAsync(objectKey);
        if (fileUrl is null) return EntityState.None.ToString();

        var data = await _downloader.DownloadFileTaskAsync(fileUrl) ?? throw new InvalidDataException();
        using var image = Image.Load(data);

        using var resizedData = new MemoryStream();
        ResizeTo(image, 300);
        image.SaveAsJpeg(resizedData);

        var preview = new PostPreview(objectKey, resizedData.ToArray());
        await _previewRepositoty.PutAsync(preview);
        return isExists ? EntityState.Modified.ToString() : EntityState.Added.ToString();
    }

    private void ResizeTo(Image image, int longSide)
    {
        var imageLongside = Math.Max(image.Width, image.Height);
        var ratio = (double)imageLongside / longSide;

        var newWidth = (int)Math.Round(image.Width / ratio, 0);
        var newHeight = (int)Math.Round(image.Height / ratio, 0);

        var resizeOptions = new ResizeOptions()
        {
            Size = new Size(newWidth, newHeight),
            Sampler = KnownResamplers.Lanczos3
        };
        image.Mutate(x => x.Resize(resizeOptions));
    }
}
