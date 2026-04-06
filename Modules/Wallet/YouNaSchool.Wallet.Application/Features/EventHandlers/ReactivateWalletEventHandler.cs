//using MediatR;

//using Microsoft.Extensions.Logging;
//using SharedKernel.Application.ClockandUserContext;
//using SharedKernel.Application.OUTBOX_PATTERN;
//using System.Text.Json;
//using YouNaSchool.Wallet.Application.Abstractions.ExternalService;
//using YouNaSchool.Wallet.Application.Abstractions.Messaging;
//using YouNaSchool.Wallet.Application.Abstractions.OtherModules;
//using YouNaSchool.Wallet.Domain.Events;

//namespace YouNaSchool.Wallet.Application.Features.EventHandlers
//{
//    public class ReactivateWalletEventHandler : INotificationHandler<WalletReactivatedEvent>
//    {
//        private readonly ILogger<ReactivateWalletEventHandler> _logger;
//        private readonly IOutBoxMessageRepository _boxMessageRepository;
//        private readonly ISystemClock _systemClock;
//        private readonly IAuthUserProvider _userEmailQuery;
//        private readonly IEmailService _emailService;

//        public ReactivateWalletEventHandler(ILogger<ReactivateWalletEventHandler> logger, IOutBoxMessageRepository boxMessageRepository, ISystemClock systemClock, IAuthUserProvider userEmailQuery, IEmailService emailService)
//        {
//            _logger = logger;
//            _boxMessageRepository = boxMessageRepository;
//            _systemClock = systemClock;
//            _userEmailQuery = userEmailQuery;
//            _emailService = emailService;
//        }

//        public async Task Handle(WalletReactivatedEvent notification, CancellationToken cancellationToken)
//        {
//            if (notification == null)
//            {
//                _logger.LogWarning("Received null notification for WalletDeactivatedEvent.");
//                return;
//            }
//            var message = notification;

//            //await _boxMessageRepository.AddAsync(new OutboxMessage
//            //{
//            //    Id = Guid.NewGuid(),
//            //    Type = nameof(notification),
//            //    Payload = JsonSerializer.Serialize(message),
//            //    OccurredOn = _systemClock.UtcNow
//            //});
//            var email = await _userEmailQuery.GetUserEmailAsync(Guid.Parse(message.StudentId),cancellationToken);
//            if (!string.IsNullOrEmpty(email))
//            {
//                await _emailService.SendEmailAsync(email, "Wallet Reactivated", $"Your wallet with ID {message.WalletId} has been reactivated.");
//            }
//            else
//            {
//                _logger.LogWarning("No email found for user with ID {StudentId}. Cannot send wallet reactivation notification.", message.StudentId);
//            }

//        }
//    }
//}
