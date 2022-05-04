namespace AshLake.Services.Yande.Services;

public class PostService : ServiceBase
{
    private readonly IPostRepository _repository;
    public PostService(IServiceCollection services, IPostRepository repository)
    : base(services)
    {
        _repository = repository;
        App.MapPost("/api/v1/post/create", CreateAsync);
    }

    public async Task<IResult> CreateAsync(
    CreatePostCommand command)
    {
        var post = new Post(command.Author,
                    command.CreatedAt,
                    command.FileExt,
                    command.FileSize,
                    command.FileUrl,
                    command.HasChildren,
                    command.Height,
                    command.PostId,
                    command.Md5,
                    command.ParentId,
                    command.Rating,
                    command.Score,
                    command.Source,
                    command.Status,
                    command.Tags,
                    command.UpdatedAt,
                    command.Width);

        await _repository.AddPostAsync(post);

        return Results.Accepted();
    }
}
