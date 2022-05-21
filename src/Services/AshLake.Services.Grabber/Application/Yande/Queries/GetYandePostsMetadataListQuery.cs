﻿using System.Text.Json.Nodes;

namespace AshLake.Services.Grabber.Application.Yande.Queries;

public record GetYandePostsMetadataListQuery : IRequest<IReadOnlyList<JsonObject>>
{
    public int StartId { get; init; }
    public int Page { get; init; }
    public int Limit { get; init; }
}

public class GetYandePostsMetadataArrayQueryHandler : IRequestHandler<GetYandePostsMetadataListQuery, IReadOnlyList<JsonObject>>
{
    private readonly YandeSourceSiteRepository _repository;

    public GetYandePostsMetadataArrayQueryHandler(YandeSourceSiteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<JsonObject>> Handle(GetYandePostsMetadataListQuery query, CancellationToken cancellationToken)
    {
        string tags = $"id:>={query.StartId} order:id";

        return await _repository.GetMetadataListAsync(tags, query.Limit, query.Page);
    }
}