
using Shared.Application.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Notifications.Application.IntegrationEvents
{
    public sealed record LecturePaidIntegrationEvent(
    Guid UserId,
    Guid LectureId,
    decimal Amount,
    DateTime PaidAt,
    string? phone, string? Email
) : IntegrationEvent;
    
}
