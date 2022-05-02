namespace AshLake.Services.Yande.Application.Posts.Commands;

public record DeletePostCommand : Command
{
    public int PostId { get; set; }
}
