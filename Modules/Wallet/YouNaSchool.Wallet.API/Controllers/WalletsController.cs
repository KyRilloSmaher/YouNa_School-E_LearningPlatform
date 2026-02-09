using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Shared.Application.RESULT_PATTERN;
using YouNaSchool.Wallet.API.Extensions;
using YouNaSchool.Wallet.Application.Commands.DeactivateWallet;
using YouNaSchool.Wallet.Application.Commands.PayLecture;
using YouNaSchool.Wallet.Application.Commands.ReActivateWallet;
using YouNaSchool.Wallet.Application.Commands.RechargeWallet;
using YouNaSchool.Wallet.Application.DTOs;
using YouNaSchool.Wallet.Application.Queries.GetAllWallets;
using YouNaSchool.Wallet.Application.Queries.GetWalletByStudentId;
using YouNaSchool.Wallet.Application.Queries.GetWalletLedgerEntry;

namespace YouNaSchool.Wallet.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class WalletsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<WalletsController> _logger;

        public WalletsController(IMediator mediator, ILogger<WalletsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{studentId}", Name = nameof(GetWalletByStudentIdAsync))]
        [ProducesResponseType(typeof(Result<StudentWalletResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWalletByStudentIdAsync([FromRoute] string studentId,CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting wallet for student ID: {StudentId}", studentId);

            var query = new GetWalletByStudentIdQuery(studentId);
            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet(Name = nameof(GetAllWalletsAsync))]
        [ProducesResponseType(typeof(Result<PaginatedResult<StudentWalletResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllWalletsAsync([FromQuery] GetAllWalletsQuery query,CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting all wallets - Page: {PageNumber}, Size: {PageSize}",query.pageNumber, query.pageSize);

            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{studentId}/ledger-entries", Name = nameof(GetWalletLedgerEntriesAsync))]
        [ProducesResponseType(typeof(Result<List<WalletLedgerEntryResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWalletLedgerEntriesAsync(
            [FromRoute] string studentId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting ledger entries for student ID: {StudentId}", studentId);

            var query = new GetWalletLedgerEntryQuery(studentId);
            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPatch("{walletId:guid}/deactivate", Name = nameof(DeactivateWalletAsync))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> DeactivateWalletAsync([FromRoute] Guid walletId,CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deactivating wallet with ID: {WalletId}", walletId);

            var command = new DeactivateWalletCommand(walletId);
            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPatch("{walletId:guid}/reactivate", Name = nameof(ReactivateWalletAsync))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> ReactivateWalletAsync(
            [FromRoute] Guid walletId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Reactivating wallet with ID: {WalletId}", walletId);

            var command = new ReActivateWalletCommand(walletId);
            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost("{walletId:guid}/recharge", Name = nameof(RechargeWalletAsync))]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> RechargeWalletAsync([FromRoute] Guid walletId,[FromBody] RechargeWalletCommand request,CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Recharging wallet {WalletId} with amount {Amount}",
                walletId, request.Amount);
            var result = await _mediator.Send(request, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost("lectures/{lectureId:guid}/purchases/wallet", Name = nameof(PurchaseLectureWithWalletAsync))]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> PurchaseLectureWithWalletAsync([FromRoute] Guid lectureId,[FromBody] PayLectureCommand request,CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Purchasing lecture {LectureId} for student {StudentId}",lectureId, request.StudentId);
            var result = await _mediator.Send(request, cancellationToken);
            return result.ToActionResult();
        }
    }
}