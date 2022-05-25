namespace AshLake.Services.Archiver.Application.Commands.CreateJobsForAddOrUpdateMetadata;

public record CreatePostMetadataJobsCommand
{
    public int StartId { get; init; }
    public int EndId { get; init; }
    public int Step { get; init; }
}
