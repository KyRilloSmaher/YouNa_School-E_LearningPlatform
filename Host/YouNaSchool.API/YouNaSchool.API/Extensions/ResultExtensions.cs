using Microsoft.AspNetCore.Mvc;
using Shared.Application.RESULT_PATTERN;
using SharedKernel.Application.RESULT_PATTERN;
using System.Net;

namespace YouNaSchool.Wallet.API.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult(this Result response)
        {
            return response.IsSuccess
                ? (IActionResult)new OkResult()
                : new BadRequestObjectResult(new { error = response.Error });
        }

        public static IActionResult ToActionResult<T>(this Result<T> response)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.OK => new OkObjectResult(response),
                HttpStatusCode.Created => new CreatedResult(string.Empty, response),
                HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(response),
                HttpStatusCode.BadRequest=> new BadRequestObjectResult(response),
                HttpStatusCode.NotFound=>new NotFoundObjectResult(response),
                HttpStatusCode.Accepted=> new AcceptedResult(string.Empty, response),
                HttpStatusCode.UnprocessableEntity=> new UnprocessableEntityObjectResult(response),
                _ => new ObjectResult(CreateProblemDetails(response))
                {
                    StatusCode = (int)response.StatusCode
                }
            };
        }

        private static ProblemDetails CreateProblemDetails<T>(Result<T> result)
        {
            return new ProblemDetails
            {
                Title = result.StatusCode.ToString(),
                Detail = result.Error,
                Status = (int)result.StatusCode
            };
        }
    }
}
