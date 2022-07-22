namespace AshLake.Services.Archiver.Application.EventConsumers;

[EntityName("s3:ObjectCreated")]
public record S3ObjectCreatedEvent(string ObjectKey);
public class S3ObjectCreatedConsumer : IConsumer<S3ObjectCreatedEvent>
{
    public async Task Consume(ConsumeContext<S3ObjectCreatedEvent> context)
    {
        var message = context.Message;

        Console.WriteLine(message.ObjectKey);
    }
}
