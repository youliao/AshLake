namespace AshLake.Services.Collector.Application.Commands;

public record AddDownloadTaskCommand(IList<string> Urls,string ObjectKey,string Md5);
