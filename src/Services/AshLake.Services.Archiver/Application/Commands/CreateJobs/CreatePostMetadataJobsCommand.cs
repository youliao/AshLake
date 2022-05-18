namespace AshLake.Services.Archiver.Application.Commands.CreateJobsForAddOrUpdateMetadata;

public record CreatePostMetadataJobsCommand
{
    public int StartPostId { get; init; }
    public int EndPostId { get; init; }
    public int Step { get; init; } = 100;
}
