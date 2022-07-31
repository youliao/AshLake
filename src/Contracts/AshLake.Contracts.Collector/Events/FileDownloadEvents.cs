namespace AshLake.Contracts.Collector.Events;
public record FileDownloadCompleted(string gid, string fileName);
public record FileDownloadError(string gid, string fileName);
