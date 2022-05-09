namespace AshLake.Services.Grabber.Modules;

public class YandeModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/yande/grabpostsmetadata", GrabYandePostsMetadataAsync)
            .Accepts<GrabYandePostsMetadataCommand>("application/json")
            .Produces(StatusCodes.Status202Accepted)
            .Produces<IEnumerable<ModelError>>(StatusCodes.Status422UnprocessableEntity)
            .WithTags("Yande")
            .WithName("GrabPostsMetadata")
            .IncludeInOpenApi();
    }

    private async Task<IResult> GrabYandePostsMetadataAsync(HttpRequest req,
                                       GrabYandePostsMetadataCommand command,
                                       IMediator mediator)
    {
        //var result = req.Validate(command);
        //if (!result.IsValid)
        //{
        //    return Results.ValidationProblem(result.GetValidationProblems(), statusCode: 422);
        //}

        await mediator.Send(command);

        return Results.CreatedAtRoute();
    }
}
