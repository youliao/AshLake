using AshLake.Services.YandeStore.Infrastructure.Repositories.Posts;

namespace AshLake.Services.YandeStore.Application.Posts.Queries;

public record GetPostByIdQuery(int PostId) : IRequest<PostMetadataDto?>;

public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, PostMetadataDto?>
{
    private readonly IPostRepository _repository;

    public GetPostByIdQueryHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<PostMetadataDto?> Handle(GetPostByIdQuery query, CancellationToken cancellationToken)
    {
        var post = await _repository.GetAsync(query.PostId);
        if (post is null) return null;

        var childIds = post.HasChildren ? await _repository.GetChildIdsAsync(query.PostId) : null;

        var dto = childIds is null ? post.Adapt<PostMetadataDto>() : (post, childIds).Adapt<PostMetadataDto>();
        return dto;

    }
}
