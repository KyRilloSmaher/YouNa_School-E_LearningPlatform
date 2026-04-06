

using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YouNaSchool.API.Extensions;
using YouNaSchool.Notifications.Application.Features.Queries;
using YouNaSchool.Wallet.API.Extensions;

namespace YouNaSchool.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Module("Notification")]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public NotificationController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserNotificationsAsync() { 
         var userId = User.Claims.FirstOrDefault(uc=>uc.Type == ClaimTypes.NameIdentifier).Value;
         var result = await _mediator.Send(new GetUserNotificationsQuery(Guid.Parse(userId)));
            return result.ToActionResult();
        }
    }
}
