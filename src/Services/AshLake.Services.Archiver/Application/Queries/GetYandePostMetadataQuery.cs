using System.Text.Json.Nodes;

namespace AshLake.Services.ArchiveBox.Application.Queries;

public record GetYandePostMetadataQuery : IRequest<JsonNode?>
{
    public int Id { get; init; }
}

