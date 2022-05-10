using System.Text.Json.Nodes;

namespace AshLake.Services.Grabber.Application.Yande.Queries;

public record GetYandePostFileQuery : IRequest<Stream>
{
    public int Id { get; init; }
}

public class GetYandePostFileQueryHandler : IRequestHandler<GetYandePostFileQuery, Stream>
{
    private readonly YandeSourceSiteRepository _repository;

    public GetYandePostFileQueryHandler(YandeSourceSiteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Stream> Handle(GetYandePostFileQuery query, CancellationToken cancellationToken)
    {
        return await _repository.GetFileAsync(query.Id);
    }
}