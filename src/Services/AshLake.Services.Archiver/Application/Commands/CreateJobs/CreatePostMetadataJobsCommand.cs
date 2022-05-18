namespace AshLake.Services.Archiver.Application.Commands.CreateJobsForAddOrUpdateMetadata;

public abstract record CreatePostMetadataJobsCommand
{
    public int StartPostId { get; init; }
    public int EndPostId { get; init; }
}
