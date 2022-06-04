namespace AshLake.Services.YandeStore.Application.Posts.Commands;

public class AddOrUpdatePostCommandValidator : AbstractValidator<AddOrUpdatePostCommand>
{
    public AddOrUpdatePostCommandValidator()
    {
        RuleFor(v => v.Width)
            .GreaterThan(100)
            .NotEmpty();

        RuleFor(v => v.Height)
            .GreaterThan(100)
            .NotEmpty();
    }
}

