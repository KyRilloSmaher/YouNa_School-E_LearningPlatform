using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchhol.Users.Application.Abstractions.Messaging.RabbitMq
{
    public interface IRabbitMqPublisher
    {
        Task PublishAsync(
            string routingKey,
            string messageType,
            string messageBody,
            CancellationToken cancellationToken = default);
    }
}
