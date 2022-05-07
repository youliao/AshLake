namespace AshLake.Services.Yande.Application.Posts.Commands;

public record DeletePostCommand
{
    public int PostId { get; init; }
}
