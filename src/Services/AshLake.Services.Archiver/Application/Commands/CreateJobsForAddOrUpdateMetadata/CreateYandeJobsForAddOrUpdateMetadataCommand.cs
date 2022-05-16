namespace AshLake.Services.Archiver.Application.Commands.CreateJobsForAddOrUpdateMetadata;

public record CreateYandeJobsForAddOrUpdateMetadataCommand : CreateJobsForAddOrUpdateMetadataCommand,
    IRequest<IEnumerable<string>>
{
}

public class CreateYandeJobsForAddOrUpdateMetadataCommandHandler : CreateJobsForAddOrUpdateMetadataCommandHandler<CreateYandeJobsForAddOrUpdateMetadataCommand>,
    IRequestHandler<CreateYandeJobsForAddOrUpdateMetadataCommand, IEnumerable<string>>
{
}
