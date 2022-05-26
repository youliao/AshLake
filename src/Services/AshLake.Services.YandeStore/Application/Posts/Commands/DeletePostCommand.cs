namespace AshLake.Services.YandeStore.Application.Posts.Commands;

public record DeletePostCommand
{
    public int PostId { get; init; }
}
