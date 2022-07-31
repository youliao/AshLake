namespace AshLake.Services.Archiver.Infrastructure.Repositories;

public record AddRangeResult(IList<int> AddedIds, IList<int> ModifiedIds, IList<int> UnchangedIds);
public record ReplaceRangeResult(IList<int> AddedIds, IList<int> ModifiedIds);
