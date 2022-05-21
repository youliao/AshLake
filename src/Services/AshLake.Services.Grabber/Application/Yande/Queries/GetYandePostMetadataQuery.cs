﻿using System.Text.Json.Nodes;

namespace AshLake.Services.Grabber.Application.Yande.Queries;

public record GetYandePostMetadataQuery : IRequest<JsonObject?>
{
    public int Id { get; init; }
}

public class GetYandePostMetadataQueryHandler : IRequestHandler<GetYandePostMetadataQuery, JsonObject?>
{
    private readonly YandeSourceSiteRepository _repository;

    public GetYandePostMetadataQueryHandler(YandeSourceSiteRepository repository)
    {
        _repository = repository;
    }

    public async Task<JsonObject?> Handle(GetYandePostMetadataQuery query, CancellationToken cancellationToken)
    {
        return await _repository.GetMetadataAsync(query.Id);
    }
}