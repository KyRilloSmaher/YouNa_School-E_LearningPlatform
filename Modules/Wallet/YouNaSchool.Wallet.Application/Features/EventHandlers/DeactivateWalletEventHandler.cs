//using MediatR;
//using Microsoft.Extensions.Logging;
//using SharedKernel.Application.ClockandUserContext;
//using SharedKernel.Application.OUTBOX_PATTERN;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;
//using YouNaSchool.Wallet.Application.Abstractions.ExternalService;
//using YouNaSchool.Wallet.Application.Abstractions.Messaging;
//using YouNaSchool.Wallet.Application.Abstractions.OtherModules;
//using YouNaSchool.Wallet.Domain.Events;

//namespace YouNaSchool.Wallet.Application.Features.EventHandlers
//{
//    public sealed class DeactivateWalletEventHandler : INotificationHandler<WalletDeactivatedEvent>
//    {
//        private readonly ILogger<DeactivateWalletEventHandler> _logger;
//        private readonly IOutBoxMessageRepository _boxMessageRepository;
//        private readonly ISystemClock _systemClock;
//        private readonly IAuthUserProvider _userEmailQuery;

//        public DeactivateWalletEventHandler(ILogger<DeactivateWalletEventHandler> logger, IOutBoxMessageRepository boxMessageRepository, ISystemClock systemClock, IEmailService emailService, IAuthUserProvider userEmailQuery)
//        {
//            _logger = logger;
//            _boxMessageRepository = boxMessageRepository;
//            _systemClock = systemClock;
//            _emailService = emailService;
//            _userEmailQuery = userEmailQuery;
//        }

//        public async Task Handle(WalletDeactivatedEvent notification, CancellationToken cancellationToken)
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
//            var email = await _userEmailQuery.GetUserEmailAsync(Guid.Parse(@message.StudentId), cancellationToken);
//            if (!string.IsNullOrEmpty(email))
//            {
//                var result = await _emailService.SendEmailAsync(email, "Wallet Deactivated", "Your wallet has been deactivated.");
//                if (result)
//                {
//                    _logger.LogInformation("Wallet deactivation notification email sent successfully to {Email}.", email);
//                }
//                else
//                {
//                    _logger.LogError("Failed to send wallet deactivation notification email to {Email}.", email);
//                }
//            }
//            else
//            {
//                _logger.LogWarning("No email found for user with ID {UserId}. Unable to send wallet deactivation notification.", message.StudentId);
//            }
//        }
//    }
//}
