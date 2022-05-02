namespace AshLake.Services.Yande.Services;

public class PostService : ServiceBase
{
    public PostService(IServiceCollection services)
    : base(services)
    {
        App.MapPost("/api/v1/post/create", CreateAsync);
    }

    public async Task<IResult> CreateAsync(
    CreatePostCommand command,
    [FromServices] IEventBus eventBus)
    {
        await eventBus.PublishAsync(command);
        return Results.Accepted();
    }
}
