using SharedKernel.Application.MESSAGING;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Application.OUTBOX_PATTERN
{
    public interface IOutboxService
    {
        Task AddAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
    }
}
