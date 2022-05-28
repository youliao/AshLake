namespace AshLake.Services.YandeStore.Application.Posts.Queries;

public record GetPostsQuery(List<string>? Tags, List<PostRating>? Ratings,List<PostStatus>? Statuses) : IRequest<IEnumerable<object>>;

public class GetPostsByTagsQueryHandler : IRequestHandler<GetPostsQuery, IEnumerable<object>>
{
    private readonly IPostRepository _repository;

    public GetPostsByTagsQueryHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<object>> Handle(GetPostsQuery query, CancellationToken cancellationToken)
    {
        return await _repository.FindAsync(query.Tags, query.Ratings, query.Statuses);
    }
}
