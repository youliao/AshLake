namespace AshLake.Services.Archiver.Application.Commands;

public record CreateTagMetadataJobsCommand(IEnumerable<int>? TagTypes);
