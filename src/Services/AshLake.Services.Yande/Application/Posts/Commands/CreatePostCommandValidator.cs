namespace AshLake.Services.Yande.Application.Posts.Commands;

public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(v => v.Rating)
            .Length(1)
            .NotEmpty();
    }
}

