using SharedKernel.Application.RESULT_PATTERN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Application.RESULT_PATTERN
{
    public class Result<T> : Result
    {
        public HttpStatusCode StatusCode { get; set; }
        public T? Value { get; }

        protected Result(T? value, bool isSuccess, string? error, HttpStatusCode code) : base(isSuccess, error)
        {
            Value = value;
            StatusCode = code;
        }

        public static Result<T> Success(T value) => new(value, true, null, HttpStatusCode.OK);

        public static Result<T> Failure(string error , HttpStatusCode code ) => new(default, false, error, code);
    }
}
