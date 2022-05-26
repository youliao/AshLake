namespace AshLake.Services.Yande.Application.Posts.Commands;

public class CreatePostCommandValidator : AbstractValidator<AddPostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(v => v.Width)
            .GreaterThan(100)
            .NotEmpty();

        RuleFor(v => v.Height)
            .GreaterThan(100)
            .NotEmpty();
    }
}

