namespace AshLake.Services.Yande.Modules;

public class PostModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/posts", AddAsync)
            .Accepts<AddPostCommand>("application/json")
            .Produces(StatusCodes.Status202Accepted)
            .Produces<IEnumerable<ModelError>>(StatusCodes.Status422UnprocessableEntity)
            .WithTags("Posts")
            .WithName("AddPost")
            .IncludeInOpenApi();

        app.MapGet("/posts/{id:int}", GetAsync)
            .Produces<Post>()
            .Produces(404)
            .WithTags("Posts")
            .WithName("GetPostById")
            .IncludeInOpenApi();
    }

    private async Task<IResult> AddAsync(HttpRequest req,
                                           AddPostCommand command,
                                           IMediator mediator)
    {
        var result = req.Validate(command);
        if (!result.IsValid)
        {
            return Results.ValidationProblem(result.GetValidationProblems(), statusCode: 422);
        }

        await mediator.Send(command);

        return Results.CreatedAtRoute();
    }

    private async Task<IResult> GetAsync(int id,
                                       IMediator mediator)
    {
        var query = new GetPostQuery() { PostId = id };

        var post = await mediator.Send(query);
        if (post is null)
            return Results.NotFound();

        return Results.Ok(post);
    }
}
