using Carter.ModelBinding;

namespace AshLake.Services.Yande.Modules;

public class PostModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/post/create", CreateAsync);
    }

    public async Task<IResult> CreateAsync(HttpRequest req,
                                           CreatePostCommand command,
                                           IMediator mediator)
    {
        var result = req.Validate(command);
        if (!result.IsValid)
        {
            return Results.ValidationProblem(result.GetValidationProblems(), statusCode: 422);
        }

        await mediator.Send(command);

        return Results.Accepted();
    }
}
