using AshLake.Services.YandeStore.Infrastructure.Repositories.Posts;

namespace AshLake.Services.YandeStore.Application.Posts.Queries;

public record GetPostsByKeysetPaginationQuery(List<string>? Tags,
                            List<PostRating>? Ratings,
                            List<PostStatus>? Statuses,
                            PostSortColumn? OrderColumn,
                            int? Limit,
                            int? referenceId,
                            KeysetPaginationDirection? Direction) : IRequest<KeysetPaginationResult<PostListItemDto>>;

public class GetPostsByKeysetPaginationQueryHandler : IRequestHandler<GetPostsByKeysetPaginationQuery, KeysetPaginationResult<PostListItemDto>>
{
    private readonly IPostRepository _repository;

    public GetPostsByKeysetPaginationQueryHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<KeysetPaginationResult<PostListItemDto>> Handle(GetPostsByKeysetPaginationQuery query, CancellationToken cancellationToken)
    {
        var paginationResult = await _repository.KeysetPaginateAsync(query.Tags ?? new List<string>(),
            query.Ratings ?? new List<PostRating>(),
            query.Statuses ?? new List<PostStatus>(),
            query.OrderColumn ?? PostSortColumn.ID,
            query.Direction ?? KeysetPaginationDirection.Forward,
            query.Limit ?? 100,
            query.referenceId);

        return paginationResult;
    }
}
