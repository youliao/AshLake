namespace AshLake.Services.Archiver.Application.Commands;

public record CreatePostMetadataJobsCommand(int StartId, int EndId, int Step);

public record CreateTagMetadataJobsCommand(IEnumerable<int>? TagTypes);