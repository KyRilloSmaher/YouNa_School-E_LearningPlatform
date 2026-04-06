using Microsoft.AspNetCore.Mvc;
using Shared.Application.RESULT_PATTERN;
using System.Net;

namespace YouNaSchool.Wallet.API.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            return result.StatusCode switch
            {
                HttpStatusCode.OK => new OkObjectResult(result.Value),
                HttpStatusCode.Created => new CreatedResult(string.Empty, result.Value),
                HttpStatusCode.NoContent => new NoContentResult(),
                HttpStatusCode.Accepted => new AcceptedResult(string.Empty, result.Value),
                HttpStatusCode.BadRequest => new BadRequestObjectResult(CreateProblemDetails(result)),
                HttpStatusCode.NotFound => new NotFoundObjectResult(CreateProblemDetails(result)),
                HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(CreateProblemDetails(result)),
                HttpStatusCode.UnprocessableEntity => new UnprocessableEntityObjectResult(CreateProblemDetails(result)),
                _ => new ObjectResult(CreateProblemDetails(result))
                {
                    StatusCode = (int)result.StatusCode
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
