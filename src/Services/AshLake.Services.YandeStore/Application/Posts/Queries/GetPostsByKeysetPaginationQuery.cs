namespace AshLake.Services.YandeStore.Application.Posts.Queries;

public record GetPostsByKeysetPaginationQuery(List<string>? Tags,
                            List<PostRating>? Ratings,
                            List<PostStatus>? Statuses,
                            PostSortColumn? OrderColumn,
                            int? Limit,
                            int? referenceId,
                            KeysetPaginationDirection? Direction) : IRequest<IEnumerable<PostListItemDto>>;

public class GetPostsByKeysetPaginationQueryHandler : IRequestHandler<GetPostsByKeysetPaginationQuery, IEnumerable<PostListItemDto>>
{
    private readonly IPostRepository _repository;

    public GetPostsByKeysetPaginationQueryHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PostListItemDto>> Handle(GetPostsByKeysetPaginationQuery query, CancellationToken cancellationToken)
    {
        var objectList = await _repository.KeysetPaginateAsync(query.Tags ?? new List<string>(),
            query.Ratings ?? new List<PostRating>(),
            query.Statuses ?? new List<PostStatus>(),
            query.OrderColumn ?? PostSortColumn.ID,
            query.Direction ?? KeysetPaginationDirection.Forward,
            query.Limit ?? 100,
            query.referenceId);

        var dtoList = objectList.Adapt<IEnumerable<PostListItemDto>>();
        return dtoList;
    }
}
