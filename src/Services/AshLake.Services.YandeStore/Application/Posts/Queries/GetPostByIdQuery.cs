namespace AshLake.Services.YandeStore.Application.Posts.Queries;

public record GetPostByIdQuery(int PostId) : IRequest<Post?>;

public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, Post?>
{
    private readonly IPostRepository _repository;

    public GetPostByIdQueryHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<Post?> Handle(GetPostByIdQuery query, CancellationToken cancellationToken)
    {
        return await _repository.GetAsync(query.PostId);
    }
}
