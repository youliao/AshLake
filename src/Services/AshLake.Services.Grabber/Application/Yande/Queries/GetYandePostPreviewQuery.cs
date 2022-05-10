using System.Text.Json.Nodes;

namespace AshLake.Services.Grabber.Application.Yande.Queries;

public record GetYandePostPreviewQuery : IRequest<Stream>
{
    public int Id { get; init; }
}

public class GetYandePostPreviewQueryHandler : IRequestHandler<GetYandePostPreviewQuery, Stream>
{
    private readonly YandeSourceSiteRepository _repository;

    public GetYandePostPreviewQueryHandler(YandeSourceSiteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Stream> Handle(GetYandePostPreviewQuery query, CancellationToken cancellationToken)
    {
        return await _repository.GetPreviewAsync(query.Id);
    }
}