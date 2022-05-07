namespace AshLake.Services.Yande.Application.Posts.Commands;

public class AddPostCommandValidator : AbstractValidator<AddPostCommand>
{
    public AddPostCommandValidator()
    {
        RuleFor(v => v.Rating)
            .Length(1)
            .NotEmpty();

        RuleFor(v => v.Width)
            .GreaterThan(100)
            .NotEmpty();

        RuleFor(v => v.Height)
            .GreaterThan(100)
            .NotEmpty();
    }
}

