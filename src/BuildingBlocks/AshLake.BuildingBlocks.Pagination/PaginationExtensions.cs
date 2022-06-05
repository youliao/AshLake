using System.Text.Json;

namespace AshLake.BuildingBlocks.Pagination;
public static class PaginationExtensions
{
    public static PaginationMetadata GetMetadata<T>(this KeysetPaginationResult<T> paginationResult) 
    {
        return new PaginationMetadata(paginationResult.TotalCount,
                                      paginationResult.PageSize,
                                      paginationResult.HasPrevious,
                                      paginationResult.HasNext);
    }

    public static string GetJson(this PaginationMetadata metadata)
    {
        return JsonSerializer.Serialize(metadata,new JsonSerializerOptions(JsonSerializerDefaults.Web));
    }
}
