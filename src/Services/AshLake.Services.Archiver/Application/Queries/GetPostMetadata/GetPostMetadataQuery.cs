using System.Text.Json.Nodes;

namespace AshLake.Services.Archiver.Application.Queries.GetPostMetadata;

public record GetPostMetadataQuery : IRequest<JsonNode?>
{
    public int Id { get; init; }
}

