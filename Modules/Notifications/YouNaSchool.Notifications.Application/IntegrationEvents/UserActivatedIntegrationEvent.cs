using Shared.Application.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Notifications.Application.IntegrationEvents
{
    public sealed record UserActivatedIntegrationEvent(
    Guid UserId,
    string Email,
    DateTime ActivatedAt,
    string? phone
) : IntegrationEvent;
}
