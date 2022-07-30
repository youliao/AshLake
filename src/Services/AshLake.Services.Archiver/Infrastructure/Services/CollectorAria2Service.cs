using Aria2NET;

namespace AshLake.Services.Archiver.Infrastructure.Services;

public interface ICollectorAria2Service
{
    Task<string> AddDownloadTask(IList<string> urls, string filename, string md5);

    Task<GlobalStatResult> GetAria2GlobalStat();
}

public class CollectorAria2Service : ICollectorAria2Service
{
    private readonly Aria2NetClient _aria2Client;

    public CollectorAria2Service(Aria2NetClient aria2Client)
    {
        _aria2Client = aria2Client ?? throw new ArgumentNullException(nameof(aria2Client));
    }

    public async Task<string> AddDownloadTask(IList<string> urls, string filename, string md5)
    {
        var options = new Dictionary<string, object>()
        {
            { "out",filename},
            { "checksum",$"md5={md5}"}
        };

        var taskId = await _aria2Client.AddUriAsync(urls, options);

        return taskId;
    }

    public async Task<GlobalStatResult> GetAria2GlobalStat()
    {
        return await _aria2Client.GetGlobalStatAsync();
    }
}
