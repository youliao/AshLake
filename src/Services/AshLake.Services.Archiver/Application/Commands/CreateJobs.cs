namespace AshLake.Services.Archiver.Application.Commands;

public record CreateAddPostMetadataJobsCommand(int StartId, int EndId, int Step);
public record CreateUpdatePostMetadataJobsCommand(int StartId, int EndId, int Step);
public record CreateTagMetadataJobsCommand(IEnumerable<int>? TagTypes);