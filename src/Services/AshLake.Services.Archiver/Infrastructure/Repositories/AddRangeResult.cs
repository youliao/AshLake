namespace AshLake.Services.Archiver.Infrastructure.Repositories;

public record AddRangeResult(IReadOnlyList<int> AddedIds, IReadOnlyList<int> ModifiedIds, IReadOnlyList<int> UnchangedIds);
