namespace AshLake.Services.YandeStore.Application.Posts.Queries;

public record GetPostsQuery(List<string>? Tags, List<PostRating>? Ratings,List<PostStatus>? Statuses,int? Limit) : IRequest<IEnumerable<PostListItemDto>>;

public class GetPostsByTagsQueryHandler : IRequestHandler<GetPostsQuery, IEnumerable<PostListItemDto>>
{
    private readonly IPostRepository _repository;

    public GetPostsByTagsQueryHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PostListItemDto>> Handle(GetPostsQuery query, CancellationToken cancellationToken)
    {
        var objectList = await _repository.FindAsync(query.Tags ?? new List<string>(),
            query.Ratings ?? new List<PostRating>(),
            query.Statuses ?? new List<PostStatus>(),
            query.Limit ?? 100);

        var dtoList = objectList.Adapt<IEnumerable<PostListItemDto>>();
        return dtoList;
    }
}
