//using Microsoft.Extensions.Logging;
//using SharedKernel.Application.UNIT_OF_WORK;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using YounaSchool.Authuntication.Application.IntegrationEvents;
//using YouNaSchool.Users.Application.Abstractions.Persistence;
//using YouNaSchool.Users.Domain.Entities;

//namespace YouNaSchool.Users.Infrastructure.IntegrationEvents.IntegrationEventsHandlers
//{
//    public class UserRegisteredIntegrationEventHandler : IIntegrationEventHandler<UserRegisteredIntegrationEvent>
//    {
//        private readonly IStudentRepository _StudentRepository;
//        private readonly IAssistantRepository _assistantRepository;
//        private readonly IUnitOfWork _unitOfWork;
//        public readonly ILogger<UserRegisteredIntegrationEventHandler> _logger;
//        public UserRegisteredIntegrationEventHandler(IStudentRepository StudentRepository, IUnitOfWork unitOfWork, ILogger<UserRegisteredIntegrationEventHandler> logger, IAssistantRepository assistantRepository)
//        {
//            _StudentRepository = StudentRepository;
//            _unitOfWork = unitOfWork;
//            _logger = logger;
//            _assistantRepository = assistantRepository;
//        }
//        public async Task HandleAsync(UserRegisteredIntegrationEvent @event, CancellationToken cancellationToken = default)
//        {

//            var student = Student.Create(@event.UserId, @event.Level);
//            var  assistant = await _assistantRepository.GetAvailableAssistantAsync(@event.TeacherId, false,cancellationToken);
//            if (assistant is not null) 
//                student.AssignAssistant(assistant.Id);
//            await _StudentRepository.AddAsync(student, cancellationToken);
//            await _unitOfWork.SaveChangesAsync(cancellationToken);
//            _logger.LogInformation("Handled {IntegrationEvent} for StudentId: {StudentId}", nameof(StudentRegisteredIntegrationEvent), @event.StudentId);
//        }
//    }
//}
