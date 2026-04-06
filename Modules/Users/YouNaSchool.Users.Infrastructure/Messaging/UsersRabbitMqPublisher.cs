using SharedPublisher = SharedKernel.Infrastructure.Messaging.RabbitMQ.IRabbitMqPublisher;
using UsersPublisher = YouNaSchhol.Users.Application.Abstractions.Messaging.RabbitMq.IRabbitMqPublisher;

namespace YouNaSchool.Users.Infrastructure.Messaging;

internal sealed class UsersRabbitMqPublisher : UsersPublisher
{
    private const string ExchangeName = "users.events";
    private readonly SharedPublisher _publisher;

    public UsersRabbitMqPublisher(SharedPublisher publisher)
    {
        _publisher = publisher;
    }

    public Task PublishAsync(
        string routingKey,
        string messageType,
        string messageBody,
        CancellationToken cancellationToken = default)
    {
        return _publisher.PublishAsync(
            exchange: ExchangeName,
            routingKey: routingKey,
            messageType: messageType,
            payload: messageBody,
            cancellationToken: cancellationToken);
    }
}
