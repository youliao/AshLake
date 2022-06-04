namespace AshLake.Services.Archiver.Infrastructure.Repositories;

public record AddRangeResult(IReadOnlyList<string> AddedIds, IReadOnlyList<string> ModifiedIds, IReadOnlyList<string> UnchangedIds);
