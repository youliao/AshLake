using System.Text.Json.Nodes;

namespace AshLake.Services.ArchiveBox.Application.Queries;

public record GetPostMetadataQuery : IRequest<JsonNode?>
{
    public int Id { get; init; }
}

