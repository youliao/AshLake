namespace AshLake.Services.Yande.Application.Posts.Queries;

public record GetPostQuery : IRequest<Post?>
{
    public int PostId { get; set; }
}

public class GetPostQueryHandler : IRequestHandler<GetPostQuery, Post?>
{
    private readonly IPostRepository _repository;

    public GetPostQueryHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<Post?> Handle(GetPostQuery query, CancellationToken cancellationToken)
    {
        return await _repository.GetAsync(query.PostId);
    }
}
