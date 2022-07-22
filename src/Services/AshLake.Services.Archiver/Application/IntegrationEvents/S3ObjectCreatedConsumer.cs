//namespace AshLake.Services.Archiver.Application.EventConsumers;

//public record S3ObjectCreatedEvent(string ObjectKey);
//public class S3ObjectCreatedConsumer : IConsumer<S3ObjectCreatedEvent>
//{
//    public async Task Consume(ConsumeContext<S3ObjectCreatedEvent> context)
//    {
//        var message = context.Message;

//        Console.WriteLine(message.ObjectKey);
//    }
//}

//public class S3ObjectCreatedConsumerDefinition :
//    ConsumerDefinition<S3ObjectCreatedConsumer>
//{

//    public S3ObjectCreatedConsumerDefinition()
//    {
//        EndpointName = "s3:ObjectCreated";
//    }
//}
