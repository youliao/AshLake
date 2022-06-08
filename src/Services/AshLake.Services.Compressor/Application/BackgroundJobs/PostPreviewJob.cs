using AshLake.Contracts.Seedwork;
using AshLake.Services.Compressor.Application.Services;
using AshLake.Services.Compressor.Domain.Repositories;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace AshLake.Services.Compressor.Application.BackgroundJobs;

public class PostPreviewJob
{
    private readonly IS3ObjectRepositoty<PostPreview> _previewRepositoty;
    private readonly ICollectorService _collectorService;

    public PostPreviewJob(IS3ObjectRepositoty<PostPreview> previewRepositoty, ICollectorService collectorService)
    {
        _previewRepositoty = previewRepositoty ?? throw new ArgumentNullException(nameof(previewRepositoty));
        _collectorService = collectorService ?? throw new ArgumentNullException(nameof(collectorService));
    }

    public async Task<string> AddOrUpdatePreview(string objectKey)
    {
        var fileData = await _collectorService.GetPostFile(objectKey);
        if (fileData is null) return EntityState.None.ToString();

        var isExists = await _previewRepositoty.ExistsAsync(objectKey);

        var image = await Image.LoadAsync(fileData);

        var resizedData = default(Stream)!;
        ResizeTo(ref image, 300);
        image.SaveAsJpeg(resizedData);

        var preview = new PostPreview(Path.GetFileNameWithoutExtension(objectKey), resizedData);
        await _previewRepositoty.PutAsync(preview);
        return isExists ? EntityState.Modified.ToString() : EntityState.Added.ToString();
    }

    public async Task<string> AddPreview(string objectKey)
    {
        var postMd5 = Path.GetFileNameWithoutExtension(objectKey);

        if (objectKey is null) return EntityState.None.ToString();

        var isExists = await _previewRepositoty.ExistsAsync(objectKey);
        if (isExists) return EntityState.Unchanged.ToString();

        var fileData = await _collectorService.GetPostFile(objectKey);
        if (fileData is null) return EntityState.None.ToString();

        var image = await Image.LoadAsync(fileData);

        var resizedData = default(Stream)!;
        ResizeTo(ref image, 300);
        image.SaveAsJpeg(resizedData);

        var preview = new PostPreview(Path.GetFileNameWithoutExtension(objectKey), resizedData);
        await _previewRepositoty.PutAsync(preview);
        return isExists ? EntityState.Modified.ToString() : EntityState.Added.ToString();
    }

    private void ResizeTo(ref Image image,int longSide)
    {
        var imageLongside = Math.Max(image.Width, image.Height);
        var ratio = (double)imageLongside / longSide;

        var newWidth = (int)Math.Round(image.Width / ratio, 0);
        var newHeight = (int)Math.Round(image.Height / ratio, 0);

        var resizeOptions = new ResizeOptions()
        {
            Size = new Size(newWidth,newHeight),
            Sampler = KnownResamplers.Lanczos3
        };
        image.Mutate(x => x.Resize(resizeOptions));
    }
}
