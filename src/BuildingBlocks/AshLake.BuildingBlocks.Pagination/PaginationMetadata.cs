namespace AshLake.BuildingBlocks.Pagination;
public record PaginationMetadata(int TotalCount, int PageSize, bool HasPrevious, bool HasNext);
